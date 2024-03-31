using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] GameObject tutorialPopUp;
    void Start()
    {
        if (PlayerPrefs.GetInt("Tutorial Complete", 0) == 0)
        {
            tutorialPopUp.SetActive(true);
        }
    }
    public void No()
    {
        tutorialPopUp.SetActive(false);
        PlayerPrefs.SetInt("Tutorial Complete", 1);
    }
    public void Yes()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
