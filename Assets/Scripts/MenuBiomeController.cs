using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelGenerator;

public class MenuBiomeController : MonoBehaviour
{
    [SerializeField] GameObject bogSampleScene;
    [SerializeField] GameObject amazonSampleScene;
    [SerializeField] GameObject cypressSampleScene;

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
            }
            else if (PlayerPrefs.GetString("StartBiome") == "Cypress")
            {
                cypressSampleScene.SetActive(true);
            }
            else if (PlayerPrefs.GetString("StartBiome") == "Amazon")
            {
                amazonSampleScene.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetString("StartBiome", "Bog");
                bogSampleScene.SetActive(true);
            }
        }
    }
}
