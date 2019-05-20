using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScorer : MonoBehaviour
{
    public int currentCount = 0;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        AddScore(PlayerPrefs.GetInt("score", 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int count)
    {
        currentCount += count;
        PlayerPrefs.SetInt("score", currentCount);
        text.text = "SCORE: " + currentCount;
    }
}
