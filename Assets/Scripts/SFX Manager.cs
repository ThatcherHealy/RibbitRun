using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    [SerializeField] AudioSource audioSource4;
    [SerializeField] AudioSource audioSource5;
    [SerializeField] AudioSource audioSource6;
    AudioSource audioSource;
    [SerializeField] AudioClip eatSFX;
    [SerializeField] AudioClip eatenSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip swimSFX;
    [SerializeField] AudioClip grappleSFX;
    [SerializeField] AudioClip splashSFX;
    [SerializeField] AudioClip bigSplashSFX;
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
    [SerializeField] AudioClip falconSwooshSFX;
    [SerializeField] AudioClip heronFlapSFX;
    [SerializeField] AudioClip heronCallSFX;
    [SerializeField] AudioClip falconCallSFX;
    [SerializeField] AudioClip salmonSpawnSFX;
    [SerializeField] AudioClip arapaimaSpawnSFX;


    [SerializeField] AudioClip generalClickSFX;
    [SerializeField] AudioClip exitClickSFX;
    [SerializeField] AudioClip startSFX;
    [SerializeField] AudioClip highScoreSFX;
    [SerializeField] AudioClip defaultRibbit;
    [SerializeField] AudioClip treeFrogRibbit;
    [SerializeField] AudioClip frogletRibbit;
    [SerializeField] AudioClip bullfrogRibbit;
    [SerializeField] AudioClip poisonDartFrogRibbit;

    public bool oneReady = true;
    public bool twoReady = true;
    public bool threeReady = true;
    public bool fourReady = true;
    public bool fiveReady = true;
    public bool sixReady = true;
    static bool mute;
    public void PlaySFX(string clipToPlay)
    {
        if(!mute) 
        {
            if (oneReady)
                audioSource = audioSource1;
            else if (twoReady)
                audioSource = audioSource2;
            else if (threeReady)
                audioSource = audioSource3;
            else if (fourReady)
                audioSource = audioSource4;
            else if (fiveReady)
                audioSource = audioSource5;
            else if (sixReady)
                audioSource = audioSource6;
            else
                audioSource = audioSource1;

            audioSource.volume = 1;
            switch (clipToPlay)
            {
                case "Eat":
                    audioSource.clip = eatSFX;
                    break;
                case "Eaten":
                    audioSource.clip = eatenSFX;
                    break;
                case "Jump":
                    audioSource.volume = 0.8f;
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
                case "Big Splash":
                    audioSource.clip = bigSplashSFX;
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
                    audioSource.volume = 0.5f;
                    audioSource.clip = mudLandSFX;
                    break;
                case "Cypress Land":
                    audioSource.volume = 0.75f;
                    audioSource.clip = cypressLandSFX;
                    break;
                case "Plant":
                    audioSource.volume = 0.5f;
                    audioSource.clip = plantSFX;
                    break;
                case "Cattail":
                    audioSource.clip = cattailSFX;
                    break;
                case "Bird Leap":
                    audioSource.clip = birdLeapSFX;
                    break;
                case "Bird Fly":
                    audioSource.volume = 0.75f;
                    audioSource.clip = birdFlySFX;
                    break;
                case "Bird Chirp":
                    audioSource.clip = birdChirpSFX;
                    break;
                case "Bird Chirp 2":
                    audioSource.clip = birdChirp2SFX;
                    break;
                case "Falcon Swoosh":
                    audioSource.clip = falconSwooshSFX;
                    break;
                case "Falcon Call":
                    audioSource.clip = falconCallSFX;
                    break;
                case "Heron Flap":
                    audioSource.clip = heronFlapSFX;
                    break;
                case "Heron Call":
                    audioSource.volume = 0.6f;
                    audioSource.clip = heronCallSFX;
                    break;
                case "Salmon Spawn":
                    audioSource.clip = salmonSpawnSFX;
                    break;
                case "Arapaima Spawn":
                    audioSource.clip = arapaimaSpawnSFX;
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
                case "Exit Click":
                    audioSource.clip = exitClickSFX;
                    break;
                case "Start":
                    audioSource.clip = startSFX;
                    break;
                case "Highscore":
                    audioSource.clip = highScoreSFX;
                    break;
                    
            }

            StartCoroutine(UseAudioSource(audioSource));
            audioSource.Play();
        }
    }
    public void PlayRibbit()
    {
        if (!mute)
        {
            if (oneReady)
                audioSource = audioSource1;
            else if (twoReady)
                audioSource = audioSource2;
            else if (threeReady)
                audioSource = audioSource3;
            else if (fourReady)
                audioSource = audioSource4;
            else if (fiveReady)
                audioSource = audioSource5;
            else if (sixReady)
                audioSource = audioSource6;
            if (PlayerController.species == PlayerController.Species.Default)
            {
                audioSource.clip = defaultRibbit;
            }
            else if (PlayerController.species == PlayerController.Species.Treefrog)
            {
                audioSource.clip = treeFrogRibbit;
                StartCoroutine(WaitThenPlayRibbit());
            }
            else if (PlayerController.species == PlayerController.Species.Froglet)
            {
                audioSource.clip = frogletRibbit;
            }
            else if (PlayerController.species == PlayerController.Species.BullFrog)
            {
                audioSource.clip = bullfrogRibbit;
            }
            else if (PlayerController.species == PlayerController.Species.PoisonDartFrog)
            {
                audioSource.clip = poisonDartFrogRibbit;
            }
            StartCoroutine(UseAudioSource(audioSource));
            audioSource.Play();
        }
    }

    //When one audio source is being used, use the next one in line so sounds can overlap
    IEnumerator UseAudioSource (AudioSource audioSource)
    {
        if(audioSource == audioSource1)
            oneReady = false;
        else if (audioSource == audioSource2) 
            twoReady = false;
        else if (audioSource == audioSource3)
            threeReady = false;
        else if (audioSource == audioSource4)
            fourReady = false;
        else if (audioSource == audioSource5)
            fiveReady = false;
        else if (audioSource == audioSource6)
            sixReady = false;

        yield return new WaitForSeconds(audioSource.clip.length);

        if (audioSource == audioSource1)
        {
            oneReady = true;
        }
        else if (audioSource == audioSource2)
        {
            twoReady = true;
        }
        else if (audioSource == audioSource3)
        {
            threeReady = true;
        }
        else if (audioSource == audioSource4)
        {
            fourReady = true;
        }
        else if (audioSource == audioSource5)
        {
            fiveReady = true;
        }
        else if (audioSource == audioSource6)
        {
            sixReady = true;
        }
    }
    IEnumerator WaitThenPlayRibbit()
    {
        yield return new WaitForSeconds(0.12f);
        audioSource.clip = treeFrogRibbit;
        StartCoroutine(UseAudioSource(audioSource));
        audioSource.Play();
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("SFX Mute") == 0)
            mute = false;
        else
            mute = true;
    }
    public static bool GetMuteStatus()
    {
        return mute;
    }
}
