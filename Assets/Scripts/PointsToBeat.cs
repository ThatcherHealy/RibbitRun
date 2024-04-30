using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsToBeat : MonoBehaviour
{
    [SerializeField] TextMeshPro x;
    [SerializeField] TextMeshPro afterX;
    [SerializeField] ScoreController scoreController;
    float difference;
    private void Start()
    {
        
    }
    private void Update()
    {
        difference = scoreController.Highscore() - scoreController.score;
        if (isActiveAndEnabled) 
        {
            if (difference == 1)
            {
                x.text = (difference).ToString();
                afterX.text = "point to beat highscore";
            }
            else if (difference <= 0)
            {
                x.text = "";
                afterX.text = "";
            }
            else
            {
                x.text = (difference).ToString();
                afterX.text = "points to beat highscore";
            }
        }
    }
}
