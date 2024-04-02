using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] GameObject tutorialPopUp;
    [SerializeField] GameObject mainMenu;
    void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial Complete", 0) == 0)
        {
            tutorialPopUp.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
    public void No()
    {
        tutorialPopUp.SetActive(false);
        mainMenu.SetActive(true);
        PlayerPrefs.SetInt("Tutorial Complete", 1);
    }
    public void Yes()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
