using System.Collections.Generic;
using UnityEngine;

namespace NinjaController
{

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class NinjaController : MonoBehaviour
    {

        public Animator myPlayerAnimator;

        private Rigidbody2D RBody { get; set; }

        [SerializeField]
        private PhysicsParams physicsParams;

        public Vector2 Velocity { get { return (RBody.velocity); } }

        public Vector2 VelocityRelativeGround { get { return (Velocity / PhysicsParams.onGroundMaxVelHorizontal); } }

        private float timeRealLastGroundCollision = 0;
        private float timeRealLastWallLeftCollision = 0;
        private float timeRealLastWallRightCollision = 0;

        public bool IsOnGround
        {
            get
            {
                return GetIsColliding(timeRealLastGroundCollision);
            }
        }

        public bool IsOnWallLeft
        {
            get
            {
                return GetIsColliding(timeRealLastWallLeftCollision);
            }
        }

        public bool IsOnWallRight
        {
            get
            {
                return GetIsColliding(timeRealLastWallRightCollision);
            }
        }

        public bool IsInAir { get { return isPlayerInAir; } }

        private bool GetIsColliding(float timeLastCollision)
        {
            return (Time.realtimeSinceStartup < timeLastCollision + 0.05f);
        }

        private Vector2 currentVelocity = Vector2.zero;
        private Vector2 currentForce = Vector2.zero;

        private float EntityMass { get { return (PhysicsParams.playerMass); } }

        private bool isPlayerInAir = false;
        private bool keyJumpRetrigger = false;
        private bool keyJumpPressed = false;
        private bool isPlayerOnWall = false;

        public PhysicsParams PhysicsParams
        {
            get { return physicsParams; }
            set { physicsParams = value; }
        }

        public Vector2 CurrentForce { get { return currentForce; } }

        public bool IsOnWall { get { return isPlayerOnWall; } }

        private List<Renderer> allRenderers;

        public List<Renderer> AllRenderers { get { return allRenderers; } }

        private bool isCollidingLadder = false;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Vector2 Position2D
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public void Awake()
        {
            RBody = GetComponent<Rigidbody2D>();
            allRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
        }

        public void Update()
        {

            //let's reset forces to 0 and then add regular gravitation
            SimResetForce();
            SimAddForce(new Vector2(0, PhysicsParams.gameGravity) * EntityMass);

            //process key input (like jumping key pressed, etc...)
            ProcessInput();

            //simulate position and velocity based on all acting forces
            ComputeVelocity(Time.deltaTime);

            //collision detection with static world
            isPlayerOnWall = IsOnWallLeft || IsOnWallRight;
            isPlayerInAir = IsOnGround == false;


            // animation
            float inputAxisX = Input.GetAxisRaw("Horizontal");
            bool isKeyDownLeft = inputAxisX < -0.5f;
            bool isKeyDownRight = inputAxisX > 0.5f;
            myPlayerAnimator.SetBool("Running", isKeyDownLeft || isKeyDownRight);

            if (isKeyDownLeft) GetComponent<SpriteRenderer>().flipX = true;
            if (isKeyDownRight) GetComponent<SpriteRenderer>().flipX = false;

            myPlayerAnimator.SetBool("InAir", isPlayerInAir);
            myPlayerAnimator.SetBool("Climbing", isCollidingLadder && currentVelocity.y > 0.001);

            myPlayerAnimator.SetBool("GoingUp", currentVelocity.y > 0.001);
            myPlayerAnimator.SetBool("GoingDown", currentVelocity.y < -0.001);

            myPlayerAnimator.SetBool("Crouching", Input.GetAxisRaw("Vertical") < -0.5f);
        }

        private void SimResetForce()
        {
            currentForce = Vector2.zero;
        }

        private void SimAddForce(Vector2 force)
        {
            currentForce += force;
        }

        private void ComputeVelocity(float dt)
        {

            currentVelocity += (currentForce / EntityMass) * dt;

            //let's cap the speed in case its higher than the max
            if (isPlayerInAir)
            {
                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -PhysicsParams.inAirMaxVelHorizontal, PhysicsParams.inAirMaxVelHorizontal);
            }
            else
            {
                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -PhysicsParams.onGroundMaxVelHorizontal, PhysicsParams.onGroundMaxVelHorizontal);
            }

            RBody.velocity = currentVelocity;
        }

        private void ProcessInput()
        {

            bool isKeyDownJump = Input.GetButton("Jump");
            float inputAxisX = Input.GetAxisRaw("Horizontal");
            bool isKeyDownLeft = inputAxisX < -0.5f;
            bool isKeyDownRight = inputAxisX > 0.5f;

            if (isCollidingLadder && Input.GetAxisRaw("Vertical") > 0.5f)
            {
                currentVelocity = new Vector2(currentVelocity.x, PhysicsParams.jumpUpVel);
            }

            //-----------------
            //JUMPING LOGIC:
            //player is on ground
            else if (isPlayerInAir == false)
            {
                //in case the player is on ground and does not press the jump key, he
                //should be allowed to jump
                if (isKeyDownJump == false)
                {
                    keyJumpRetrigger = true;
                }

                //did player press down the jump button?
                if (isKeyDownJump == true && keyJumpRetrigger == true)
                {
                    keyJumpPressed = true;
                    keyJumpRetrigger = false;

                    //when pressing jump on ground we set the upwards velocity directly
                    currentVelocity = new Vector2(currentVelocity.x, PhysicsParams.jumpUpVel);
                }
            }
            else if (isPlayerOnWall == true)
            {
                //let's allow jumping again in case of being on the wall
                if (isKeyDownJump == false)
                {
                    keyJumpRetrigger = true;
                }
                if (currentVelocity.y < 0)
                {//apply friction when moving downwards
                    SimAddForce(new Vector2(0, PhysicsParams.wallFriction) * EntityMass);
                }
                if (currentVelocity.y < PhysicsParams.wallFrictionStrongVelThreshold)
                {//apply even more friction when moving downwards fast
                    SimAddForce(new Vector2(0, PhysicsParams.wallFrictionStrong) * EntityMass);
                }
                if (isKeyDownJump == true && keyJumpRetrigger == true)
                {
                    keyJumpPressed = true;
                    keyJumpRetrigger = false;

                    //in case we are moving down -> let's set the velocity directly
                    //in case we are moving up -> sum up velocity
                    if (IsOnWallLeft == true)
                    {
                        if (currentVelocity.y <= 0)
                        {
                            currentVelocity = new Vector2(PhysicsParams.jumpWallVelHorizontal, PhysicsParams.jumpWallVelVertical);
                        }
                        else
                        {
                            currentVelocity = new Vector2(PhysicsParams.jumpWallVelHorizontal, currentVelocity.y + PhysicsParams.jumpWallVelVertical);
                        }
                    }
                    else if (IsOnWallRight == true)
                    {
                        if (currentVelocity.y <= 0)
                        {
                            currentVelocity = new Vector2(-PhysicsParams.jumpWallVelHorizontal, PhysicsParams.jumpWallVelVertical);
                        }
                        else
                        {
                            currentVelocity = new Vector2(-PhysicsParams.jumpWallVelHorizontal, currentVelocity.y + PhysicsParams.jumpWallVelVertical);
                        }
                    }
                }
            }
            //did player lift the jump button?
            if (isKeyDownJump == false)
            {
                keyJumpPressed = false;
            }

            //let's apply force in case we are holding the jump key during a jump.
            if (keyJumpPressed == true)
            {
                SimAddForce(new Vector2(0, PhysicsParams.jumpUpForce) * EntityMass);
            }
            //however let's stop doing that as soon as we fall down after the up-phase.
            if (keyJumpPressed == true && currentVelocity.y <= 0)
            {
                keyJumpPressed = false;
            }

            //let's apply additional gravity in case we're in air moving up but not holding the jump button
            if (keyJumpPressed == false && isPlayerInAir == true && currentVelocity.y > 0)
            {
                SimAddForce(new Vector2(0, PhysicsParams.jumpGravity) * EntityMass);
            }

            //-----------------
            //IN AIR SIDEWAYS:
            if (isPlayerInAir == true)
            {
                //steering into moving direction (slow accel)
                if (isKeyDownLeft == true && currentVelocity.x <= 0)
                {
                    SimAddForce(new Vector2(-PhysicsParams.inAirMoveHorizontalForce, 0) * EntityMass);
                }
                else if (isKeyDownRight == true && currentVelocity.x >= 0)
                {
                    SimAddForce(new Vector2(PhysicsParams.inAirMoveHorizontalForce, 0) * EntityMass);
                }
                //steering against moving direction (fast reverse accel)
                else if (isKeyDownLeft == true && currentVelocity.x >= 0)
                {
                    SimAddForce(new Vector2(-PhysicsParams.inAirMoveHorizontalForceReverse, 0) * EntityMass);
                }
                else if (isKeyDownRight == true && currentVelocity.x <= 0)
                {
                    SimAddForce(new Vector2(PhysicsParams.inAirMoveHorizontalForceReverse, 0) * EntityMass);
                }
            }

            //-----------------
            //ON GROUND SIDEWAYS:
            if (isPlayerInAir == false)
            {
                //steering into moving direction (slow accel)
                if (isKeyDownLeft == true && currentVelocity.x <= 0)
                {
                    SimAddForce(new Vector2(-PhysicsParams.onGroundMoveHorizontalForce, 0) * EntityMass);
                }
                else if (isKeyDownRight == true && currentVelocity.x >= 0)
                {
                    SimAddForce(new Vector2(PhysicsParams.onGroundMoveHorizontalForce, 0) * EntityMass);
                }
                //steering against moving direction (fast reverse accel)
                else if (isKeyDownLeft == true && currentVelocity.x >= 0)
                {
                    SimAddForce(new Vector2(-PhysicsParams.onGroundMoveHorizontalForceReverse, 0) * EntityMass);
                }
                else if (isKeyDownRight == true && currentVelocity.x <= 0)
                {
                    SimAddForce(new Vector2(PhysicsParams.onGroundMoveHorizontalForceReverse, 0) * EntityMass);
                }
                //not steering -> brake due to friction.
                else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x > 0)
                {
                    SimAddForce(new Vector2(-PhysicsParams.groundFriction, 0) * EntityMass);
                }
                else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x < 0)
                {
                    SimAddForce(new Vector2(PhysicsParams.groundFriction, 0) * EntityMass);
                }

                //in case the velocity is close to 0 and no keys are pressed we should make the the player stop.
                //to do this let's first undo the prior friction force, and then set the velocity to 0.
                if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x > 0 && currentVelocity.x < PhysicsParams.groundFrictionEpsilon)
                {
                    SimAddForce(new Vector2(PhysicsParams.groundFriction, 0) * EntityMass);
                    currentVelocity.x = 0;
                }
                else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x < 0 && currentVelocity.x > -PhysicsParams.groundFrictionEpsilon)
                {
                    SimAddForce(new Vector2(-PhysicsParams.groundFriction, 0) * EntityMass);
                    currentVelocity.x = 0;
                }
            }
        }

        public void ResetVelocity()
        {
            currentVelocity = Vector2.zero;
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag != "ladder")
            {
                foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                if (GetIsVectorClose(new Vector2(0, 1f), contactPoint.normal))
                {
                    timeRealLastGroundCollision = Time.realtimeSinceStartup;
                    currentVelocity.y = Mathf.Clamp(currentVelocity.y, 0, Mathf.Abs(currentVelocity.y));
                }
                if (GetIsVectorClose(new Vector2(1, 0), contactPoint.normal))
                {
                    timeRealLastWallLeftCollision = Time.realtimeSinceStartup;
                    currentVelocity.x = Mathf.Clamp(currentVelocity.x, 0, Mathf.Abs(currentVelocity.x));
                }
                if (GetIsVectorClose(new Vector2(-1, 0), contactPoint.normal))
                {
                    timeRealLastWallRightCollision = Time.realtimeSinceStartup;
                    currentVelocity.x = Mathf.Clamp(currentVelocity.x, -Mathf.Abs(currentVelocity.x), 0);
                }
                if (GetIsVectorClose(Vector2.down, contactPoint.normal))
                {
                    currentVelocity.y = Mathf.Clamp(currentVelocity.y, -Mathf.Abs(currentVelocity.y), 0);
                }
            }
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "ladder")
            {
                isCollidingLadder = true;
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "ladder")
            {
                isCollidingLadder = false;
            }
        }

        private bool GetIsVectorClose(Vector2 vectorA, Vector2 vectorB)
        {
            return Vector2.Distance(vectorA, vectorB) < 0.1f;
        }

        public void OnLifeChanged(int life, Vector2 contactVector)
        {
            const float forceEnemyCollision = 15.0f;
            currentVelocity = contactVector.normalized * forceEnemyCollision;
        }

        public void ResetPlayer()
        {
            currentVelocity = Vector2.zero;
        }

    }
}
