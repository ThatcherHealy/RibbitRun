using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    AudioSource audioSource;
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
    [SerializeField] AudioClip rockSFX;
    [SerializeField] AudioClip birdLeapSFX;
    [SerializeField] AudioClip birdFlySFX;
    [SerializeField] AudioClip birdChirpSFX;
    [SerializeField] AudioClip birdChirp2SFX;
    [SerializeField] AudioClip generalClickSFX;

    float delay = 0.15f;
    public bool oneReady = true;
    public bool twoReady = true;
    public bool threeReady = true;
    public void PlaySFX(string clipToPlay)
    {
        if (oneReady)
            audioSource = audioSource1;
        else if (twoReady)
            audioSource = audioSource2;
        else
            audioSource = audioSource3;

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
            case "Bird Leap":
                audioSource.clip = birdLeapSFX;
                break;
            case "Bird Fly":
                audioSource.clip = birdFlySFX;
                break;
            case "Bird Chirp":
                audioSource.clip = birdChirpSFX;
                break;
            case "Bird Chirp 2":
                Debug.Log("Playing Sound");
                audioSource.clip = birdChirp2SFX;
                break;
            case "Drown":
                audioSource.clip = drownSFX;
                break;
            case "Poison":
                audioSource.clip = poisonedSFX;
                break;
            case "Rock":
                audioSource.clip = rockSFX;
                break;
            case "Click":
                audioSource.clip = generalClickSFX;
                break;
        }

        StartCoroutine(UseAudioSource(audioSource));
        audioSource.Play();
    }

    //When one audio source is being used, use the next one in line so sounds can overlap
    IEnumerator UseAudioSource (AudioSource audioSource)
    {
        if(audioSource == audioSource1)
            oneReady = false;
        else if (audioSource == audioSource2) 
            twoReady = false;
        else
            threeReady = false;

        yield return new WaitForSeconds(audioSource.clip.length);

        if (audioSource == audioSource1)
            oneReady = true;
        else if (audioSource == audioSource2)
            twoReady = true;
        else
            threeReady = true;
    }
}
