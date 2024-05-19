using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HardModeButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshPro hardModeText;
    [SerializeField] GameObject hardModePopup;
    [SerializeField] Color unavailableColor;
    [SerializeField] Color availableColor;
    [SerializeField] Color activeColor;
    MenuSFXManager sfx;
    void Start()
    {
        sfx = FindFirstObjectByType<MenuSFXManager>();

        if (PlayerPrefs.GetInt("poisonDartFrogUnlocked") == 1)
            PlayerPrefs.SetInt("HardModeUnlocked", 1);

        if (PlayerPrefs.GetInt("HardModeUnlocked") == 0)
        {
            hardModeText.text = "Locked";
            button.interactable = false;
            image.color = unavailableColor;
        }
        else
        {
            button.interactable = true;
            image.color = availableColor;
        }
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("HardModeUnlocked") == 1)
        {
            if (PlayerPrefs.GetInt("HardMode", 0) == 0)
            {
                hardModePopup.SetActive(false);
                image.color = availableColor;
            }
            else
            {
                hardModePopup.SetActive(true);
                image.color = activeColor;
            }
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
