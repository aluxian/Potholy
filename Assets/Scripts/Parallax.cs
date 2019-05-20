using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class Parallax : MonoBehaviour
{
    public List<Color> cols = new List<Color>();
    public Transform pos;
    public float distanceModifier;
    public float speed;
    public List<Transform> backgroundLayers = new List<Transform>();
    public Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Player");
        Color newCol = cols[Random.Range(0, cols.Count)];
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            backgroundLayers[i].gameObject.GetComponent<Tilemap>().color = newCol;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < backgroundLayers.Count; i++)
        {
            backgroundLayers[i].position = -playerTransform.position * speed * ((i+1) * distanceModifier);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Color newCol = cols[Random.Range(0, cols.Count)];
            for (int i = 0; i < backgroundLayers.Count; i++)
            {
                backgroundLayers[i].gameObject.GetComponent<Tilemap>().color = newCol;
            }
        }



    }
}
