using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverText : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        int currentScore = ScoreManager.instance.score;
        text.text += currentScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
