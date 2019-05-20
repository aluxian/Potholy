using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject[] objects;
    
    // Start is called before the first frame update
    void Start()
    {
        // spawn random object on start
        GameObject child = Instantiate(objects.GetRand(), transform.position, Quaternion.identity);

        // make it a child of current gameObject
        child.transform.parent = transform;
    }

}
