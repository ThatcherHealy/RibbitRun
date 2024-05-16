using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnedObject;

    //There is a 1/probability chance that the object is spawned 
    [SerializeField] float probabilty;
    [SerializeField] float hardModeProbabilty;
    [SerializeField] bool guaranteed;

    [SerializeField] bool child;
    [SerializeField] Transform parent;

    [SerializeField] bool increment;
    [SerializeField] float incrementDistance;
    [SerializeField] float incrementSize;
    [SerializeField] float hardModeIncrementDistance;
    [SerializeField] float hardModeincrementSize;
    private void Start()
    {

        if(PlayerPrefs.GetInt("HardMode") == 0) //Normal Mode
        {
            if (increment && SceneManager.GetActiveScene().name == "GameScene")
            {
                //Increment spawn probability by incrementSize every incrementDistance points
                ScoreController sc = FindFirstObjectByType<ScoreController>();
                float multiplier = Mathf.Round(sc.score / incrementDistance);
                for (int i = 0; i < multiplier; i++)
                {
                    probabilty += incrementSize;
                }
            }


            if (guaranteed || Random.Range(1, 100) <= probabilty)
            {
                if (!child)
                {
                    Instantiate(spawnedObject, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(spawnedObject, transform.position, Quaternion.identity, parent);
                }
            }
        }
        else //Hard Mode
        {
            if (increment && SceneManager.GetActiveScene().name == "GameScene")
            {
                //Increment spawn probability by incrementSize every incrementDistance points
                ScoreController sc = FindFirstObjectByType<ScoreController>();
                float multiplier = Mathf.Round(sc.score / hardModeIncrementDistance);
                for (int i = 0; i < multiplier; i++)
                {
                    probabilty += hardModeincrementSize;
                }
            }



            if (guaranteed || Random.Range(1, 100) <= hardModeProbabilty)
            {
                if (!child)
                {
                    Instantiate(spawnedObject, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(spawnedObject, transform.position, Quaternion.identity, parent);
                }
            }
        }

    }

}