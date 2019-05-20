using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
 * Defines info about a room.
 */
public class RoomSpec : MonoBehaviour
{
    public bool hasLeftExit;
    public bool hasRightExit;
    public bool hasTopExit;
    public bool hasBottomExit;
    public GameObject playerSpawnLocation;
    public GameObject exitSpawnLocation;

    public Direction[] Exits
    {
        get
        {
            List<Direction> dirs = new List<Direction>();
            if (hasLeftExit) dirs.Add(Direction.Left);
            if (hasRightExit) dirs.Add(Direction.Right);
            if (hasTopExit) dirs.Add(Direction.Top);
            if (hasBottomExit) dirs.Add(Direction.Bottom);
            return dirs.ToArray();
        }
    }

    public bool canSpawnPlayer
    {
        get
        {
            return playerSpawnLocation != null;
        }
    }

    public bool hasExitDoor
    {
        get
        {
            return exitSpawnLocation != null;
        }
    }

    public bool HasExit(Direction dir)
    {
        return Array.Exists(Exits, elem => elem == dir);
    }
}
