using UnityEngine;
using System.Collections;
using System;

public static class ArrayExt
{
    public static T? GetRandNul<T>(this T[] arr) where T : struct
    {
        if (arr.Length == 0)
        {
            return null;
        }
        int index = UnityEngine.Random.Range(0, arr.Length);
        return arr[index];
    }

    public static T GetRand<T>(this T[] arr) where T : class
    {
        if (arr.Length == 0)
        {
            return null;
        }
        int index = UnityEngine.Random.Range(0, arr.Length);
        return arr[index];
    }
}
