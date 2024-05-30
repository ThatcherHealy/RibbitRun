using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicContinuity : MonoBehaviour
{
    public static MusicContinuity instance;
    [SerializeField] string sceneAllowedOne;
    [SerializeField] string sceneAllowedTwo;
    static bool mute;

    private void Awake()
    {
        if (!InCorrectScene())
        {
            Destroy(gameObject);
        }

        //Make music manager remain across scenes
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            if(gameObject.name == instance.name)
            {
                Destroy(gameObject);
            }
        }
    }
    private void Update()
    {
        if (PlayerPrefs.GetInt("Music Mute") == 0)
            mute = false;
        else
            mute = true;

        GetComponent<AudioSource>().mute = mute;
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music Volume", 1);

        if (!InCorrectScene())
        {
            Destroy(gameObject);
        }
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
    bool InCorrectScene()
    {
        //Destroy the music manager when outside of the allowed scenes
        if (sceneAllowedOne != null && sceneAllowedTwo != null)
        {
            if (!(SceneManager.GetActiveScene().name == sceneAllowedOne || SceneManager.GetActiveScene().name == sceneAllowedTwo))
            {
                return false;
            }
        }
        else if (sceneAllowedOne != null)
        {
            if (!(SceneManager.GetActiveScene().name == sceneAllowedOne))
            {
                return false;
            }
        }
        return true;
    }
    public static bool GetMuteStatus()
    {
        return mute;
    }
}
