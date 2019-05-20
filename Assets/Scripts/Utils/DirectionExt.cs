using UnityEngine;
using System.Collections;

public static class DirectionExt
{
    public static IJPair IndexDelta(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Left: return new IJPair(0, -1);
            case Direction.Right: return new IJPair(0, +1);
            case Direction.Top: return new IJPair(-1, 0);
            case Direction.Bottom: return new IJPair(+1, 0);
        }

        return new IJPair(0, 0);
    }

    public static Direction Invert(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            case Direction.Top: return Direction.Bottom;
            case Direction.Bottom: return Direction.Top;
        }

        throw new System.Exception("missing a case");
    }
}
