using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPopUp : MonoBehaviour
{
    [SerializeField] GameObject tutorialPopUp;
    [SerializeField] GameObject[] mainMenu;
    MenuSFXManager sfx;
    void Start()
    {
        sfx = FindFirstObjectByType<MenuSFXManager>();
        if (PlayerPrefs.GetInt("Tutorial Complete", 0) == 0)
        {
            tutorialPopUp.SetActive(true);
            foreach (GameObject obj in mainMenu) 
            {
                obj.SetActive(false);
            }
        }
    }
    public void No()
    {
        sfx.PlaySFX("Exit Click");
        tutorialPopUp.SetActive(false);
        foreach (GameObject obj in mainMenu)
        {
            obj.SetActive(true);
        }
        PlayerPrefs.SetInt("Tutorial Complete", 1);
    }
    public void Yes()
    {
        sfx.PlaySFX("Click");
        SceneManager.LoadScene("Tutorial");
    }
}
