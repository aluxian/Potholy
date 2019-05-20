using System.Collections;
using UnityEngine;

public class RigidbodyCharacter : MonoBehaviour
{

    public float HorizontalMoveThrust = 100f;
    public float JumpThrust = 250f;
    public float ClimbThrust = 250f;
    public float GroundDistance = 0.2f;
    public bool allowInputs = true;

    public MenuController menuController;
    public LevelChanger levelChanger;

    //public float DashDistance = 5f;
    public LayerMask Ground;
    public AudioSource pickUpSound;
    private Rigidbody2D _body;
    private Vector2 _inputs = Vector2.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    public Animator _anim;
    public GameScorer gameScore;

    private bool _is_colliding_ladder = false;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _groundChecker = transform.GetChild(0);
        pickUpSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!allowInputs)
        {
            return;
        }

        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        _body.AddForce(new Vector2(Input.GetAxis("Horizontal"), 0) * HorizontalMoveThrust);

        if (_is_colliding_ladder && Input.GetButton("Jump"))
        {
            _body.AddForce(Vector2.up * ClimbThrust);
        } else if (Input.GetButtonDown("Jump"))
        {
            _body.velocity += Vector2.up * JumpThrust;
        }

        _anim.SetBool("Crouching", Input.GetAxis("Vertical") < -0.1);
        _anim.SetBool("Running", Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1);
        _anim.SetBool("Climbing", _is_colliding_ladder);

        if (Input.GetAxis("Horizontal") < -0.1) GetComponent<SpriteRenderer>().flipX = true;
        if (Input.GetAxis("Horizontal") > 0.1) GetComponent<SpriteRenderer>().flipX = false;

        float f = 2;
        _body.velocity = new Vector2(Mathf.Min(_body.velocity.x, 10*f), Mathf.Min(_body.velocity.y, 10 * f));
        _body.velocity = new Vector2(Mathf.Max(_body.velocity.x, -10 * f), Mathf.Max(_body.velocity.y, -10 * f));

        //if (Input.GetButtonDown("Jump") && _is_colliding_ladder)
        //{
        //    _body.AddForce(Vector2.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode2D.Impulse);
        //}

        //if (Input.GetButtonDown("Jump") && _isGrounded)
        //{
        //    _body.AddForce(Vector2.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode2D.Force);
        //}
        //if (Input.GetButtonDown("Dash"))
        //{
        //    Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
        //    _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ladder")
        {
            _is_colliding_ladder = true;
        }

        if (collision.CompareTag("gem"))
        {
            pickUpSound.Play();
            Destroy(collision.gameObject);
            gameScore.AddScore(10);
        }

        if (collision.CompareTag("exitdoor"))
        {
            levelChanger.LoadSceneAsyncWithFade("Level1");
        }

        if (collision.CompareTag("spike"))
        {
            StartCoroutine(DieAsync());
        }
    }

    public IEnumerator DieAsync()
    {
        _anim.SetTrigger("die");
        allowInputs = false;
        yield return new WaitForSeconds(3);
        menuController.ShowDeathScreen();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ladder")
        {
            _is_colliding_ladder = false;
        }
    }
}