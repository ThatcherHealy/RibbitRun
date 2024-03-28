using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertController : MonoBehaviour
{
    [SerializeField] GameObject alertPrefab;
    void Start()
    {
        CheckForNewUnlocks();
    }
    void CheckForNewUnlocks() 
    {
        //Treefrog
        if (PlayerPrefs.GetInt("Highscore", 0) >= 1500 && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1)
        {
            Alert();
            PlayerPrefs.SetInt("treeFrogUnlocked", 1);
        }

        //Froglet
        if (PlayerPrefs.GetInt("FishEaten", 0) >= 100 && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1 && PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1)
        {
            Alert();
            PlayerPrefs.SetInt("frogletUnlocked", 1);
        }

        //Bullfrog
        if (PlayerPrefs.GetInt("InsectsEaten", 0) >= 500 && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1 && PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
        {
            Alert();
            PlayerPrefs.SetInt("bullfrogUnlocked", 1);
        }

        //Poison Dart Frog
        if (PlayerPrefs.GetInt("Highscore", 0) >= 5000 && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1 && PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1)
        {
            Alert();
            PlayerPrefs.SetInt("poisonDartFrogUnlocked", 1);
        }
    }

    private void Alert()
    {
        Instantiate(alertPrefab, new Vector2(-7.99f, -5.69f), Quaternion.identity);
    }
}
