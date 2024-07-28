using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicContinuity : MonoBehaviour
{
    [SerializeField] bool menu;
    Scene currentScene;
    public static MusicContinuity instance;
    [SerializeField] AudioSource gameMusic;
    [SerializeField] AudioSource underwaterGameMusic;
    [SerializeField] AudioSource deadGameMusic;

    PlayerController pc;
    DeathScript ds;

    bool underwaterMusic;

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
    private void Start()
    {
        if (!menu)
        {
            currentScene = SceneManager.GetActiveScene();
            pc = FindFirstObjectByType<PlayerController>();
            ds = FindFirstObjectByType<DeathScript>();
            gameMusic.mute = false; underwaterGameMusic.mute = true; deadGameMusic.mute = true;
        }
    }
    private void Update()
    {
        if(!menu && SceneManager.GetActiveScene() != currentScene )
        {
            pc = FindFirstObjectByType<PlayerController>();
            ds = FindFirstObjectByType<DeathScript>();
            currentScene = SceneManager.GetActiveScene();
        }

        if (PlayerPrefs.GetInt("Music Mute") == 0)
            mute = false;
        else
            mute = true;
        if(!menu)
        {
            if (PlayerPrefs.GetInt("Music Mute") == 1)
            {
                gameMusic.mute = mute; underwaterGameMusic.mute = mute; deadGameMusic.mute = mute;
            }
        }
        else
        {
            gameMusic.mute = mute;
        }

        if (!menu)
        {
            gameMusic.volume = PlayerPrefs.GetFloat("Music Volume", 1); underwaterGameMusic.volume = PlayerPrefs.GetFloat("Music Volume", 1); deadGameMusic.volume = PlayerPrefs.GetFloat("Music Volume", 1);
        }
        else
            gameMusic.volume = PlayerPrefs.GetFloat("Music Volume", 1);

        if (InCorrectScene() == false)
        {
            Destroy(gameObject);
        }
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        if (!menu)
        {
            if (pc != null && pc.isSwimming)
            {
                underwaterMusic = true;
            }
            if (pc != null && !pc.wet)
            {
                underwaterMusic = false;
            }
        }

    }
    private void FixedUpdate()
    {
        if (!menu)
        {
            if (PlayerPrefs.GetInt("Music Mute") == 0)
            {
                if (pc.dead && (ds.dontRespawnPressed || ds.respawnedOnce))
                {
                    gameMusic.mute = true; underwaterGameMusic.mute = true; deadGameMusic.mute = false;
                    deadGameMusic.pitch = 0.7f;
                    //Debug.Log("Dead Music");
                }
                else
                {
                    if (underwaterMusic)
                    {
                        gameMusic.mute = true; underwaterGameMusic.mute = false; deadGameMusic.mute = true;
                        //Debug.Log("Underwater Music");

                    }
                    else
                    {
                        gameMusic.mute = false; underwaterGameMusic.mute = true; deadGameMusic.mute = true;
                        //Debug.Log("Normal Music");

                    }
                }
            }
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
