using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelGenerator;

public class MenuBiomeController : MonoBehaviour
{
    [SerializeField] GameObject bogSampleScene;
    [SerializeField] GameObject amazonSampleScene;
    [SerializeField] GameObject cypressSampleScene;

    [SerializeField] Camera cam;
    [SerializeField] Color bogColor;
    [SerializeField] Color amazonColor;
    [SerializeField] Color cypressColor;

    void Awake()
    {
        if (Time.timeScale != 1) //Resume time
        { 
            Time.timeScale = 1; 
        }

        if (PlayerPrefs.GetString("StartBiome") != null) //Spawn the corresponding sample scene
        {
            //Start you in the biome you died in last
            if (PlayerPrefs.GetString("StartBiome") == "Bog")
            {
                bogSampleScene.SetActive(true);
                cam.backgroundColor = bogColor;
            }
            else if (PlayerPrefs.GetString("StartBiome") == "Cypress")
            {
                cypressSampleScene.SetActive(true);
                cam.backgroundColor = cypressColor;
            }
            else if (PlayerPrefs.GetString("StartBiome") == "Amazon")
            {
                amazonSampleScene.SetActive(true);
                cam.backgroundColor = amazonColor;
            }
            else
            {
                PlayerPrefs.SetString("StartBiome", "Bog");
                bogSampleScene.SetActive(true);
                cam.backgroundColor = bogColor;
            }
        }
    }
}
