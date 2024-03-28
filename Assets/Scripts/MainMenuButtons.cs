using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerController;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] Image[] buttons;
    [SerializeField] FrogUnlock unlock;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "SpeciesMenu")
            SelectCurrentSpecies();
    }

    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void SpeciesMenu()
    {
        SceneManager.LoadScene("SpeciesMenu");
    }
    public void Return()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SelectDefault()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        PlayerPrefs.SetString("Species", "Default");
    }
    public void SelectTreefrog()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        PlayerPrefs.SetString("Species", "Tree Frog");
        PlayerPrefs.SetInt("TreeFrogClaimed", 1);
        if (unlock.treeFrogAlert != null) 
        {
            Destroy(unlock.treeFrogAlert);
        }
    }
    public void SelectFroglet()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        PlayerPrefs.SetString("Species", "Froglet");
        PlayerPrefs.SetInt("FrogletClaimed", 1);
        if (unlock.frogletAlert != null)
        {
            Destroy(unlock.frogletAlert);
        }
    }
    public void SelectBullfrog()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        PlayerPrefs.SetString("Species", "Bullfrog");
        PlayerPrefs.SetInt("BullfrogClaimed", 1);
        if (unlock.bullfrogAlert != null)
        {
            Destroy(unlock.bullfrogAlert);
        }
    }
    public void SelectPoisonDartFrog()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        PlayerPrefs.SetString("Species", "Poison Dart Frog");
        PlayerPrefs.SetInt("PoisonDartFrogClaimed", 1);
        if (unlock.poisonDartFrogAlert != null)
        {
            Destroy(unlock.poisonDartFrogAlert);
        }
    }

    void UnselectOthers(Image currentButton) 
    {
        foreach(Image button in buttons)
        {
            if (button != currentButton)
                button.color = Color.white;
        }
    }
    private void SelectCurrentSpecies()
    {
        if (PlayerPrefs.GetString("Species") == "Default")
            buttons[0].color = Color.green;
        else if (PlayerPrefs.GetString("Species") == "Tree Frog")
            buttons[1].color = Color.green;
        else if (PlayerPrefs.GetString("Species") == "Froglet")
            buttons[2].color = Color.green;
        else if (PlayerPrefs.GetString("Species") == "Bullfrog")
            buttons[3].color = Color.green;
        else if (PlayerPrefs.GetString("Species") == "Poison Dart Frog")
            buttons[4].color = Color.green;
        else
        {
            PlayerPrefs.SetString("Species", "Default");
            species = Species.Default;
            SelectCurrentSpecies();
        }
    }
}
