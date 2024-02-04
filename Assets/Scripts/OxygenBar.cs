using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class OxygenBar : MonoBehaviour
{
    public Slider oxygenBar;
    public Slider moistureBar;
    public Transform frog;
    public PlayerController playerController;
    public GameObject oxygenFill;
    public GameObject moistureFill;
    public Image blackout;
    public PostProcessVolume volume;
    //public PostProcessProfile profile;
    private Vignette vignette;
    private float maxOxygen = 100;
    private float maxMoisture = 100;
    private float currentOxygen = 100;
    private float currentMoisture = 100;

    public float num;
    void Start()
    {
        volume.profile.TryGetSettings(out vignette);

        oxygenBar.maxValue = maxOxygen;
        currentOxygen = maxOxygen;
        oxygenBar.value = currentOxygen;

        moistureBar.maxValue = maxMoisture;
        currentMoisture = maxMoisture;
        moistureBar.value = currentMoisture;
    }
    private void FixedUpdate()
    {
        LoseAndGainOxygenandMoisture();
    }

    private void LoseAndGainOxygenandMoisture() 
    {
        if (playerController.isSwimming)
        {
            currentOxygen -= 0.08f;
        }
        else
        {
            currentOxygen += 0.5f;
        }

        if (!playerController.isSwimming)
        {
            currentMoisture -= 0.07f;
        }
        else
        {
            currentMoisture += 2f;
        }
    }

    void Update()
    {
        oxygenBar.value = currentOxygen;
        moistureBar.value = currentMoisture;

        BoundBar();
        Appear();
        DepleteToZero();

        Blackout();
        Dry();
    }
    private void BoundBar() 
    {
        if (currentOxygen > 100)
        {
            currentOxygen = 100;
        }
        if (currentOxygen < 0)
        {
            currentOxygen = 0;
        }

        if (currentMoisture > 100)
        {
            currentMoisture = 100;
        }
        if (currentMoisture < 0)
        {
            currentMoisture = 0;
        }
    }
    private void Appear()
    {

        if (currentOxygen < currentMoisture)
        {
            oxygenBar.gameObject.SetActive(true);
            moistureBar.gameObject.SetActive(false);
        }
        if (currentMoisture < currentOxygen)
        {
            moistureBar.gameObject.SetActive(true);
            oxygenBar.gameObject.SetActive(false);
        }   
    }
    private void DepleteToZero()
    {
        if (oxygenBar.value < 4)
        {
            oxygenFill.SetActive(false);
        }
        else
        {
            oxygenFill.SetActive(true);
        }

        if (moistureBar.value < 4)
        {
            moistureFill.SetActive(false);
        }
        else
        {
            moistureFill.SetActive(true);
        }
    }
    private void Blackout()
    {
        if (currentOxygen <= 0)
        {
            blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b, blackout.color.a + (0.35f * Time.deltaTime));
        }
        else
        {
            blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b, blackout.color.a - (1 * Time.deltaTime));
        }

        if (blackout.color.a < 0)
        {
            blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b, 0);
        }

        //Die
        if(blackout.color.a >= 1)
        {
            playerController.dead = true;
            playerController.drowned = true;
        }
    }
    private void Dry()
    {
        if (currentMoisture <= 30)
        {
            vignette.intensity.value += (0.05f * Time.deltaTime);
        }
        else
        {
            if (vignette.intensity.value > 0)
            {
                vignette.intensity.value -= (1 * Time.deltaTime);
            }
        }

        if (vignette.intensity.value < 0)
        {
            vignette.intensity.value = 0;
        }
        if (vignette.intensity.value > 0.3f)
        {
            vignette.intensity.value = 0.3f;
        }

        //Dry
        if (vignette.intensity.value >= 0.25f)
        {
            playerController.dried = true;
        }
        if (moistureBar.value > 0)
        {
            playerController.dried = false;
        }
    }
}
