using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class OxygenAndMoistureController : MonoBehaviour
{
    [SerializeField] Slider oxygenBar;
    [SerializeField] Slider moistureBar;
    [SerializeField] Transform frog;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject oxygenFill;
    [SerializeField] GameObject moistureFill;
    [SerializeField] Image blackout;
    [SerializeField] PostProcessVolume volume;
    private Vignette vignette;
    
    private float maxOxygen = 100;
    private float maxMoisture = 100;
    private float currentOxygen = 100;
    private float currentMoisture = 100;
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
            currentOxygen -= 0.06f; //Oxygen loss speed
        }
        else
        {
            currentOxygen += 0.5f; //Oxygen gain speed
        }

        if (!playerController.isSwimming)
        {
            currentMoisture -= 0.06f; //Moisture loss speed
        }
        else
        {
            currentMoisture += 2f; //Moisture gain speed
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
        //Keep bar values between 0 and 100
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
        //Only show the bar with the lowest fill
        oxygenBar.gameObject.SetActive(currentOxygen < currentMoisture);
        moistureBar.gameObject.SetActive(currentMoisture < currentOxygen);

        if (currentMoisture >= 100 && currentOxygen >= 100)
        {
            moistureBar.gameObject.SetActive(false);
            oxygenBar.gameObject.SetActive(false);
        }
    }
    private void DepleteToZero()
    {
        //Makes fill disappear when the value < 4, this fixes the fill area looking weird as it approaches 0
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
        //Begins fade in when the player loses oxygen and fades out when the player gains oxygen
        blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b,
        Mathf.Clamp(blackout.color.a + (currentOxygen <= 0 ? 0.25f : -1f) * Time.deltaTime, 0, 1));

        //Die after the blackout is opaque
        if(blackout.color.a >= 1)
        {
            playerController.dead = true;
            playerController.drowned = true;
        }
    }
    private void Dry()
    {
        //Start vignette effect when the player gets to 30% moisture
        //Removes vignette effect when the player goes above 30%
        vignette.intensity.value += (currentMoisture <= 30 ? 0.05f : -1f) * Time.deltaTime;
        vignette.intensity.value = Mathf.Clamp(vignette.intensity.value, 0, 0.3f);

         //Dry out when the vignette is almost at max intensity and be cured when the player touches water
         if (vignette.intensity.value >= 0.22f)
         {
             playerController.dried = true;
         }
         if (moistureBar.value > 0)
         {
             playerController.dried = false;
         } 
    }
}
