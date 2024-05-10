using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioSource audioSource3;
    [SerializeField] AudioSource audioSource4;
    [SerializeField] AudioSource audioSource5;
    [SerializeField] AudioSource audioSource6;
    AudioSource audioSource;

    [SerializeField] AudioClip generalClickSFX;
    [SerializeField] AudioClip startSFX;
    [SerializeField] AudioClip exitClickSFX;
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

            switch (clipToPlay)
            {
                case "General Click":
                    audioSource.clip = generalClickSFX;
                    break;
                case "Start":
                    audioSource.clip = startSFX;
                    break;
                case "Exit Click":
                    audioSource.clip = exitClickSFX;
                    break;
                case "Default Ribbit":
                    audioSource.clip = defaultRibbit;
                    break;
                case "Tree Frog Ribbit":
                    audioSource.clip = treeFrogRibbit;
                    break;
                case "Froglet Ribbit":
                    audioSource.clip = frogletRibbit;
                    break;
                case "Bullfrog Ribbit":
                    audioSource.clip = bullfrogRibbit;
                    break;
                case "Poison Dart Frog Ribbit":
                    audioSource.clip = poisonDartFrogRibbit;
                    break;

            }

            StartCoroutine(UseAudioSource(audioSource));
            audioSource.Play();
        }

    } 

    //When one audio source is being used, use the next one in line so sounds can overlap
    IEnumerator UseAudioSource(AudioSource audioSource)
    {
        if (audioSource == audioSource1)
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
