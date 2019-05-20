using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs; // room choices
    public GameObject player;
    public GameObject exitDoor;
    public float roomSize; // distance between room walls

    private readonly RoomSpec[,] grid = new RoomSpec[4, 4]; // store instantiated rooms
    private readonly Direction[] allDirections = new Direction[] { Direction.Left, Direction.Right, Direction.Top, Direction.Bottom };

    //private RoomSpec firstRoom; // the RoomSpec of the first room (the starting room of the player)

    // Start is called before the first frame update
    void Start()
    {
        InstantiateCriticalPathRooms();
        InstantiateAllOtherRooms();
        Destroy(gameObject);
    }

    private void InstantiateCriticalPathRooms()
    {
        int i = 0; // grid position we are currently spawning
        int j = UnityEngine.Random.Range(0, 4); // grid position we are currently spawning

        Direction? prev_dir = null;

        bool hasPlacedExitDoor = false;
        
        while (true)
        {
            //Debug.Log("running i=" + i + " j=" + j);

            bool thisIsFirstRoom = prev_dir == null;

            // where we can go from current i, j
            Direction[] validDirections = Array.FindAll(allDirections, d =>
                d != Direction.Top // never go up
                && (j > 0 || d != Direction.Left) // do not go left if we're at the leftmost room already
                && (j < 3 || d != Direction.Right) // do not go right if we're at the rightmost room already
                && (i < 3 || d != Direction.Bottom) // do not go bottom if we're at the lowest room already
                && (grid[i + d.IndexDelta().i, j + d.IndexDelta().j] == null) // do not go on an already spawned room
            );

            // chosen direction
            Direction? dir = validDirections.GetRandNul();
            
            // filter prefabs by rooms that have an entrance in the chosen exit direction
            GameObject[] roomPrefabsThatHaveExitInChosenDir = Array.FindAll(roomPrefabs, rp =>
                {
                    RoomSpec spec = rp.GetComponentInChildren<RoomSpec>(true);
                    return spec != null
                        && (prev_dir == null || spec.HasExit(prev_dir.Value.Invert())) // entrance from previous dir
                        && (dir == null || spec.HasExit(dir.Value)) // exit in next dir
                        && (!thisIsFirstRoom || spec.canSpawnPlayer) // if this is the first room we are spawning, it must have a player spawn location
                        && (dir != null || spec.hasExitDoor) // has exit door if it is last spawning room
                        ;
                }
            );

            // pick one randomly
            GameObject roomPrefab = roomPrefabsThatHaveExitInChosenDir.GetRand();
            if (roomPrefab == null)
            {
                throw new Exception("thisIsFirstRoom=" + thisIsFirstRoom + " add a room prefab that has an exit of type " + (dir != null ? dir.Value.ToString() : "null"));
            }

            // spawn
            //Debug.Log("spawning i=" + i + " j=" + j);
            float x = (j - 1) * roomSize - roomSize / 2;
            float y = -((i - 1) * roomSize - roomSize / 2);
            grid[i, j] = Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<RoomSpec>();

            // place the player (already spawned) here
            if (thisIsFirstRoom)
            {
                player.transform.position = grid[i, j].playerSpawnLocation.transform.position;
            }

            // put exit door and stop
            if (dir == null)
            {
                exitDoor.transform.position = grid[i, j].exitSpawnLocation.transform.position;
                break;
            }

            // save values as previous
            prev_dir = dir;

            // move pointers in the next dir
            i += dir.Value.IndexDelta().i;
            j += dir.Value.IndexDelta().j;
        }
    }

    //private IEnumerator UpdatePlayerPositionAsync(Vector3 newPos)
    //{
    //    yield return new WaitForSecondsRealtime(3);
    //    player.transform.position = newPos;
    //    Debug.Log("setting player loc to " + newPos);
    //}
    private void InstantiateAllOtherRooms()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (grid[i, j] == null)
                {
                    float x = (j - 1) * roomSize - roomSize / 2;
                    float y = -((i - 1) * roomSize - roomSize / 2);
                    grid[i, j] = Instantiate(roomPrefabs.GetRand(), new Vector3(x, y, 0), Quaternion.identity).GetComponent<RoomSpec>();
                }
            }
        }
    }
}
