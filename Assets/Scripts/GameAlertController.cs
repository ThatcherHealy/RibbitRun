using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameAlertController : MonoBehaviour
{
    [SerializeField] GameObject alertPrefab;
    [SerializeField] GameObject notification;
    [SerializeField] SFXManager sfx;
    [SerializeField] ScoreController sc;
    GameObject alert;
    Transform alertTransform;
    bool tWasUnlocked, fWasUnlocked, bWasUnlocked, pWasUnlocked;
    bool tSpawned, fSpawned, bSpawned, pSpawned;

    private void Start()
    {
        tWasUnlocked = PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1;
        fWasUnlocked = PlayerPrefs.GetInt("frogletUnlocked", 0) == 1;
        bWasUnlocked = PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1;
        pWasUnlocked = PlayerPrefs.GetInt("poisonDartFrogUnlocked", 0) == 1;
    }
    public void CheckForNewUnlocks()
    {
        //Treefrog
        if ((PlayerPrefs.GetInt("Highscore", 0) >= 1500 || sc.score > 1500) && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1)
        {
            PlayerPrefs.SetInt("treeFrogUnlocked", 1);
            Debug.Log("Tree Frog Unlocked");
        }

        //Froglet
        if (PlayerPrefs.GetInt("InsectsEaten", 0) >= 250 && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1 && PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1)
        {
            PlayerPrefs.SetInt("frogletUnlocked", 1);
        }

        //Bullfrog
        if (PlayerPrefs.GetInt("FishEaten", 0) >= 100 && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1 && PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
        {
            PlayerPrefs.SetInt("bullfrogUnlocked", 1);
        }

        //Poison Dart Frog
        if ((PlayerPrefs.GetInt("Highscore", 0) >= 5000 || sc.score > 5000) && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1 && PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1)
        {
            PlayerPrefs.SetInt("poisonDartFrogUnlocked", 1);
        }
    }
    private void Update()
    {
        CheckForNewUnlocks();

        if (PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1 && !tWasUnlocked && !tSpawned)
        {
            Debug.Log("In First if");
            Notification("Tree Frog");
            tSpawned = true;
        }
        if (PlayerPrefs.GetInt("frogletUnlocked", 0) == 1 && !fWasUnlocked && !fSpawned)
        {
            Notification("Froglet");
            fSpawned = true;
        }
        if (PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1 && !bWasUnlocked && !bSpawned)
        {
            Notification("Bullfrog");
            bSpawned = true;
        }
        if (PlayerPrefs.GetInt("poisonDartFrogUnlocked", 0) == 1 && !pWasUnlocked && !pSpawned)
        {
            if (!bSpawned)
                Notification("Poison Dart Frog");
            else
                StartCoroutine(WaitToSpawnPoisonDartFrogNotification());
            pSpawned = true;
        }

        tWasUnlocked = PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1;
        fWasUnlocked = PlayerPrefs.GetInt("frogletUnlocked", 0) == 1;
        bWasUnlocked = PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1;
        pWasUnlocked = PlayerPrefs.GetInt("poisonDartFrogUnlocked", 0) == 1;
    }
    private void Notification(string species)
    {
        GameObject noti = Instantiate(notification, new Vector3(0, 150, 0), Quaternion.identity, transform);
        noti.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
        sfx.PlaySFX("Notification");
        if (species == "Tree Frog")
        {
            noti.GetComponentInChildren<TextMeshPro>().text = "Tree Frog\nUnlocked";
        }
        if (species == "Froglet")
        {
            noti.GetComponentInChildren<TextMeshPro>().text = "Froglet\nUnlocked";
        }
        if (species == "Bullfrog")
        {
            noti.GetComponentInChildren<TextMeshPro>().text = "Bullfrog\nUnlocked";
        }
        if (species == "Poison Dart Frog")
        {
            noti.GetComponentInChildren<TextMeshPro>().text = "Poison Dart\nFrog Unlocked";
        }
    }
    IEnumerator WaitToSpawnPoisonDartFrogNotification() 
    {
        yield return new WaitForSeconds(5);
        Notification("Poison Dart Frog");
    }
    public void Alert(Transform alertPosition)
    {
        if (PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1 && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1)
        {
            alert = Instantiate(alertPrefab, alertPosition.position, Quaternion.identity);
            alertTransform = alertPosition;
        }
        else if (PlayerPrefs.GetInt("frogletUnlocked", 0) == 1 && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1)
        {
            alert = Instantiate(alertPrefab, alertPosition.position, Quaternion.identity);
            alertTransform = alertPosition;
        }
        else if (PlayerPrefs.GetInt("bullfrogUnlocked", 0) == 1 && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1)
        {
            alert = Instantiate(alertPrefab, alertPosition.position, Quaternion.identity);
            alertTransform = alertPosition;
        }
        else if (PlayerPrefs.GetInt("poisonDartFrogUnlocked", 0) == 1 && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1)
        {
            alert = Instantiate(alertPrefab, alertPosition.position, Quaternion.identity);
            alertTransform = alertPosition;
        }
    }
    private void FixedUpdate()
    {
        if (alert != null)
        {
            alert.transform.position = alertTransform.position;
        }
    }
}