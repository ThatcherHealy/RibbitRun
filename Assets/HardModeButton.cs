using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardModeButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] GameObject hardModePopup;
    MenuSFXManager sfx;
    void Start()
    {
        sfx = FindFirstObjectByType<MenuSFXManager>();

        if (PlayerPrefs.GetInt("poisonDartFrogUnlocked") == 1)
            PlayerPrefs.SetInt("HardModeUnlocked", 1);

        if (PlayerPrefs.GetInt("HardModeUnlocked") == 0)
        {
            button.interactable = false;
            image.color = Color.grey;
        }
        else
        {
            button.interactable = true;
            image.color = Color.white;
        }
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("HardMode", 0) == 0)
        {
            hardModePopup.SetActive(false);
            image.color = Color.white;
        }
        else
        {
            hardModePopup.SetActive(true);
            image.color = Color.red;
        }
    }
    public void EnableHardmode() 
    {
        if (PlayerPrefs.GetInt("HardMode", 0) == 0)
        {
            sfx.PlaySFX("Hard Mode");
            PlayerPrefs.SetInt("HardMode", 1);
        }
        else
        {
            sfx.PlaySFX("Exit Click");
            PlayerPrefs.SetInt("HardMode", 0);
        }
    }
}
