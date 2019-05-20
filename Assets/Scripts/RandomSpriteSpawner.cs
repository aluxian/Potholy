using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteSpawner : MonoBehaviour
{
    public Sprite[] spriteChoices;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = spriteChoices.GetRand();
    }

}
