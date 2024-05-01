using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip eatSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip swimSFX;
    [SerializeField] AudioClip grappleSFX;
    [SerializeField] AudioClip splashSFX;
    [SerializeField] AudioClip exitWaterSFX;
    [SerializeField] AudioClip lilypadLandSFX;
    [SerializeField] AudioClip logLandSFX;
    [SerializeField] AudioClip mudLandSFX;
    [SerializeField] AudioClip cypressLandSFX;
    [SerializeField] AudioClip plantSFX;
    [SerializeField] AudioClip cattailSFX;
    [SerializeField] AudioClip drownSFX;
    [SerializeField] AudioClip poisonedSFX;


    public void PlaySFX(string clipToPlay)
    {
        switch (clipToPlay)
        {
            case "Eat":
                audioSource.clip = eatSFX;
                break;
            case "Jump":
                audioSource.clip = jumpSFX;
                break;
            case "Swim":
                audioSource.clip = swimSFX;
                break;
            case "Grapple":
                audioSource.clip = grappleSFX;
                break;
            case "Splash":
                audioSource.clip = splashSFX;
                break;
            case "Exit Water":
                audioSource.clip = exitWaterSFX;
                break;
            case "Lilypad Land":
                audioSource.clip = lilypadLandSFX;
                break;
            case "Log Land":
                audioSource.clip = logLandSFX;
                break;
            case "Mud Land":
                audioSource.clip = mudLandSFX;
                break;
            case "Cypress Land":
                audioSource.clip = cypressLandSFX;
                break;
            case "Plant":
                audioSource.clip = plantSFX;
                break;
            case "Cattail":
                audioSource.clip = cattailSFX;
                break;
            case "Drowned":
                audioSource.clip = drownSFX;
                break;
            case "Poisoned":
                audioSource.clip = poisonedSFX;
                break;

        }

        audioSource.Play();
    }
}
