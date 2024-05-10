using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerController;
using static Unity.Collections.AllocatorManager;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject[] mainStuff;
    [SerializeField] Image[] buttons;
    [SerializeField] FrogUnlock unlock;
    MenuSFXManager sfx;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "SpeciesMenu")
            SelectCurrentSpecies();
    }
    private void Start()
    {
        sfx = FindFirstObjectByType<MenuSFXManager>();
    }

    public void Play()
    {
        StartCoroutine(WaitThenLoadScene("GameScene"));
        sfx.PlaySFX("Start");
    }
    public void SpeciesMenu()
    {
        StartCoroutine(WaitThenLoadScene("SpeciesMenu"));
        sfx.PlaySFX("General Click");

    }
    public void Return()
    {
        StartCoroutine(WaitThenLoadScene("MainMenu"));
        sfx.PlaySFX("Exit Click");
    }
    public void Options()
    {
        optionsMenu.SetActive(true);
        sfx.PlaySFX("General Click");
        foreach (GameObject obj in mainStuff)
        {
            obj.SetActive(false);
        }
    }
    public void OptionsBack()
    {
        optionsMenu.SetActive(false);
        foreach (GameObject obj in mainStuff)
        {
            obj.SetActive(true);
        }
        sfx.PlaySFX("Exit Click");
    }
    public void ReplayTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void MuteSound()
    {
        if(SFXManager.GetMuteStatus() == true || MenuSFXManager.GetMuteStatus() == true)
        {
            Debug.Log("Unmute");
            PlayerPrefs.SetInt("SFX Mute", 0);
        }
        else
        {
            Debug.Log("Mute");
            PlayerPrefs.SetInt("SFX Mute", 1);
        }
    }
    public void MuteMusic()
    {
        if (MusicContinuity.GetMuteStatus() == true)
        {
            PlayerPrefs.SetInt("Music Mute", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Music Mute", 1);
        }
    }
    public void SelectDefault()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        if (PlayerPrefs.GetString("Species", "Default") != "Default") 
            sfx.PlaySFX("Default Ribbit");

        PlayerPrefs.SetString("Species", "Default");
    }
    public void SelectTreefrog()
    {
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;
        UnselectOthers(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>());
        if (PlayerPrefs.GetString("Species", "Default") != "Tree Frog")
        {
            sfx.PlaySFX("Tree Frog Ribbit");
            StartCoroutine(WaitThenPlaySFX("Tree Frog Ribbit"));
        }

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
        if (PlayerPrefs.GetString("Species") != "Froglet")
        {
            sfx.PlaySFX("Froglet Ribbit");
        }

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
        if (PlayerPrefs.GetString("Species", "Default") != "Bullfrog")
            sfx.PlaySFX("Bullfrog Ribbit");

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
        if (PlayerPrefs.GetString("Species", "Default") != "Poison Dart Frog")
            sfx.PlaySFX("Poison Dart Frog Ribbit");

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
    IEnumerator WaitThenLoadScene(string sceneName) 
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator WaitThenPlaySFX(string sfxName)
    {
        yield return new WaitForSeconds(0.12f);
        sfx.PlaySFX(sfxName);
    }
}
