using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] bool sfxSlider;
    [SerializeField] bool menu;
    [SerializeField] MainMenuButtons mainMenuButtons;
    [SerializeField] PauseButtons pauseButtons;
    float previousSliderValue;

    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();

        if (sfxSlider)
        {
            slider.value = PlayerPrefs.GetFloat("SFX Volume", 1);
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat("Music Volume", 1);
        }
       
        previousSliderValue = slider.value;
    }

    void Update()
    {
        if(sfxSlider) 
        {
            PlayerPrefs.SetFloat("SFX Volume", slider.value);

            if(menu)
            {
                if(slider.value <= 0.001 && previousSliderValue > 0.001 && PlayerPrefs.GetInt("SFX Mute", 0) == 0 || (slider.value > 0.001 && previousSliderValue <= 0.001 && PlayerPrefs.GetInt("SFX Mute", 0) == 1))
                {
                    mainMenuButtons.MuteSound();
                }
            }
            else
            {
                if(slider.value <= 0.001f && previousSliderValue > 0.001 && PlayerPrefs.GetInt("SFX Mute", 0) == 0 || (slider.value > 0.001 && previousSliderValue <= 0.001 && PlayerPrefs.GetInt("SFX Mute", 0) == 1))
                {
                    pauseButtons.MuteSound();
                }
            }
        }
        else
        {
            PlayerPrefs.SetFloat("Music Volume", slider.value);

            if (menu)
            {
                if (slider.value <= 0.001 && previousSliderValue > 0.001 && PlayerPrefs.GetInt("Music Mute", 0) == 0 || (slider.value > 0.001 && previousSliderValue <= 0.001 && PlayerPrefs.GetInt("Music Mute", 0) == 1))
                {
                    mainMenuButtons.MuteMusic();
                }
            }
            else
            {
                if (slider.value <= 0.001f && previousSliderValue > 0.001 && PlayerPrefs.GetInt("Music Mute", 0) == 0 || (slider.value > 0.001 && previousSliderValue <= 0.001 && PlayerPrefs.GetInt("Music Mute", 0) == 1))
                {
                    pauseButtons.MuteMusic();
                }
            }
        }

        previousSliderValue = slider.value;
    }
    
}
