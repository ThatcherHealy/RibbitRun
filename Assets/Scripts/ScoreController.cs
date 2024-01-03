using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private float xDistance;
    private float farthestDistance = 0;
    public Transform playerPosition;
    public int score = 0;
    public TextMeshPro floatingText;
    public GameObject floatingTextParent;
    public Canvas canvas;


    private void Start()
    {
        scoreText.enabled = false;
    }
    private void Update()
    {
        //Starts displaying score after the player gets a point
        if (score > 0)
            scoreText.enabled = true;

        scoreText.text = score.ToString();

        //Adds a point whenever the player goes 4 units farther in the x axis than their previous location
        xDistance = Vector3.Distance(Vector3.zero, new Vector3(playerPosition.position.x, 0,0));
        int scoreThreshold = 4;

        if (xDistance > farthestDistance + scoreThreshold)
        {
            farthestDistance = xDistance;
            Score(1);
        }
    }

    public int Score(int scoreChange)
    {
        //Updates the score by the amount inputted
        score += scoreChange;
        return score;
    }

    public void SpawnFloatingText(int value, Vector3 position)
    {
        //Spawns a score of the value inputted at the location inputted and then destroys it  

        floatingText.text = value.ToString();
        Vector3 offset = new Vector3(0, 2, -0.5f);

        GameObject parent = Instantiate(floatingTextParent, position + offset, Quaternion.identity);
        TextMeshPro spawnedScore = Instantiate(floatingText, position, Quaternion.identity, parent.transform);
        Destroy(spawnedScore.gameObject, 2);
        Destroy(parent, 2);
    }
}