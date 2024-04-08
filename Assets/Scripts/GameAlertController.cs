using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameAlertController : MonoBehaviour
{
    [SerializeField] GameObject alertPrefab;
    GameObject alert;
    Transform alertTransform;

    public void CheckForNewUnlocks(Transform alertPosition)
    {
        //Treefrog
        if (PlayerPrefs.GetInt("Highscore", 0) >= 1500 && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1)
        {
            Alert(alertPosition);
            PlayerPrefs.SetInt("treeFrogUnlocked", 1);
        }

        //Froglet
        if (PlayerPrefs.GetInt("FishEaten", 0) >= 100 && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1 && PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1)
        {
            Alert(alertPosition);
            PlayerPrefs.SetInt("frogletUnlocked", 1);
        }

        //Bullfrog
        if (PlayerPrefs.GetInt("InsectsEaten", 0) >= 500 && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1 && PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
        {
            Alert(alertPosition);
            PlayerPrefs.SetInt("bullfrogUnlocked", 1);
        }

        //Poison Dart Frog
        if (PlayerPrefs.GetInt("Highscore", 0) >= 5000 && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1 && PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1)
        {
            Alert(alertPosition);
            PlayerPrefs.SetInt("poisonDartFrogUnlocked", 1);
        }
    }

    private void Alert(Transform alertPosition)
    {
        alert = Instantiate(alertPrefab, alertPosition.position, Quaternion.identity);
        alertTransform = alertPosition;
    }
    private void Update()
    {
        if(alert != null)
        {
            alert.transform.position = alertTransform.position;
        }
    }
}
