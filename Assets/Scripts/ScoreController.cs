using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score;

    [SerializeField] bool tutorial;
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] Transform playerPosition;
    [SerializeField] TextMeshPro floatingText;
    [SerializeField] GameObject floatingTextParent;
    [SerializeField] Canvas canvas;
    [SerializeField] PlayerController playerController;
    [SerializeField] LevelGenerator lg;
    [SerializeField] TextMeshPro finalScoreText;
    [SerializeField] TextMeshPro highscoreText;
    [SerializeField] TextMeshPro drownedFinalScoreText;
    [SerializeField] TextMeshPro drownedHighscoreText;
    [SerializeField] TextMeshPro poisonedFinalScoreText;
    [SerializeField] TextMeshPro poisonedHighscoreText;
    [SerializeField] Transform startPoint;

    private float initialPosition;
    private float xDistance;
    private float farthestDistance;
    private float totalDistanceTravelled;

    private void Start()
    {
        farthestDistance = startPoint.position.x;
        scoreText.enabled = false;

        if (lg != null)
            initialPosition = lg.endPoint.x;
        else
            initialPosition = startPoint.transform.position.x;
    }
    private void Update()
    {
        //Starts displaying score after the player gets a point
        if (score > 0)
        {
            scoreText.enabled = true;
        }

        scoreText.text = score.ToString();

        //Adds a point whenever the player goes 4 units farther in the x axis than their previous location
        if (playerPosition.position.x > initialPosition)
            xDistance = Vector3.Distance(new Vector3(initialPosition,0,0), new Vector3(playerPosition.position.x, 0,0));
        int scoreThreshold = 4;

        if (xDistance > farthestDistance + scoreThreshold)
        {
            farthestDistance = xDistance;
            Score(1);
            totalDistanceTravelled += 1;
        }

        //When the player dies, display their final score
        if (playerController.dead && !tutorial)
        {
            if (playerController.drowned) 
            {
                finalScoreText = drownedFinalScoreText;
                highscoreText = drownedHighscoreText;
            }
            if (playerController.poisoned)
            {
                finalScoreText = poisonedFinalScoreText;
                highscoreText = poisonedHighscoreText;
            }
            scoreText.text = new string(" ");
            finalScoreText.text = score.ToString();

            CheckHighscore(score);
            highscoreText.text = PlayerPrefs.GetInt("Highscore", 0).ToString();
        }
    }

    public void Score(int scoreChange)
    {
        //Updates the score by the amount inputted
        if(!playerController.dead) 
        {
            score += scoreChange;
        }
    }

    public void CheckHighscore(int finalScore)
    {
        //Set the main highscore
        if (finalScore > PlayerPrefs.GetInt("Highscore", 0))
        {
            PlayerPrefs.SetInt("Highscore", finalScore);
        }

        //Set individual species highscore 
        if(PlayerController.species == PlayerController.Species.Default)
        {
            if (finalScore > PlayerPrefs.GetInt("DefaultHighscore", 0))
            {
                PlayerPrefs.SetInt("DefaultHighscore", finalScore);
            }
        }
        if (PlayerController.species == PlayerController.Species.Treefrog)
        {
            if (finalScore > PlayerPrefs.GetInt("TreeFrogHighscore", 0))
            {
                PlayerPrefs.SetInt("TreeFrogHighscore", finalScore);
            }
        }
        if (PlayerController.species == PlayerController.Species.Froglet)
        {
            if (finalScore > PlayerPrefs.GetInt("FrogletHighscore", 0))
            {
                PlayerPrefs.SetInt("FrogletHighscore", finalScore);
            }
        }
        if (PlayerController.species == PlayerController.Species.BullFrog)
        {
            if (finalScore > PlayerPrefs.GetInt("BullfrogHighscore", 0))
            {
                PlayerPrefs.SetInt("BullfrogHighscore", finalScore);
            }
        }
        if (PlayerController.species == PlayerController.Species.PoisonDartFrog)
        {
            if (finalScore > PlayerPrefs.GetInt("PoisonDartFrogHighscore", 0))
            {
                PlayerPrefs.SetInt("PoisonDartFrogHighscore", finalScore);
            }
        }
    }

    public void SpawnFloatingText(int value, Vector3 position, Color color)
    {
        if (!playerController.dead)
        {
            //Spawns a score of the value inputted at the location inputted and then destroys it  
            floatingText.text = value.ToString();
            Vector3 offset = new Vector3(0, 2, -0.5f);

            GameObject parent = Instantiate(floatingTextParent, position + offset, Quaternion.identity);
            TextMeshPro spawnedScore = Instantiate(floatingText, position, Quaternion.identity, parent.transform);
            spawnedScore.color = color;

            Destroy(spawnedScore.gameObject, 2);
            Destroy(parent, 2);
        }
    }
}