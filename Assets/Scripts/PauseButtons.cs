using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    [SerializeField] bool tutorial;
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] ScoreController scoreController;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseIcon;
    [SerializeField] GameObject score;
    SFXManager sfx;
    public bool pause;
    bool unpause;

    [SerializeField] GameObject SFXMuteBar;
    [SerializeField] Image SFXbutton;
    [SerializeField] GameObject musicMuteBar;
    [SerializeField] Image musicButton;
    [SerializeField] Color onColor;
    private void Start()
    {
        sfx = FindFirstObjectByType<SFXManager>();
    }
    private void Update()
    {
        if(unpause) 
        {
            Time.timeScale = 1;
        }

        if (PlayerPrefs.GetInt("SFX Mute", 0) == 0)
        {
            SFXbutton.color = onColor;
            SFXMuteBar.SetActive(false);
        }
        else
        {
            //SFXbutton.color = Color.grey;
            SFXMuteBar.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Music Mute", 0) == 0)
        {
            musicButton.color = onColor;
            musicMuteBar.SetActive(false);
        }
        else
        {
            //musicButton.color = Color.grey;
            musicMuteBar.SetActive(true);
        }
    }
    public void Pause()
    {
        sfx.PlaySFX("Click");
        pause = true;
        pauseMenu.SetActive(true);
        score.SetActive(false);
        pauseIcon.SetActive(false);
        Time.timeScale = 0;
    }
    public void Resume()
    {
        pause = false;
        sfx.PlaySFX("Exit Click");
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
    }
    public void Restart()
    {
        sfx.PlaySFX("Start");
        pause = false;
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
        if (!tutorial)
        {
            SetBiome();
            scoreController.CheckHighscore(scoreController.score);
        }
        StartCoroutine(WaitThenLoadScene("GameScene"));
    }
    public void TutorialRestart()
    {
        sfx.PlaySFX("Start");

        pause = false;
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
        StartCoroutine(WaitThenLoadScene("Tutorial"));
    }
    public void Home()
    {
        Time.timeScale = 1;

        sfx.PlaySFX("Exit Click");
        StartCoroutine(WaitThenLoadScene("MainMenu"));

        if (!tutorial) 
        {
            SetBiome();
            scoreController.CheckHighscore(scoreController.score);
        }
    }
    public void MuteSound()
    {
        if (SFXManager.GetMuteStatus() == true)
        {
            PlayerPrefs.SetInt("SFX Mute", 0);
        }
        else
        {
            PlayerPrefs.SetInt("SFX Mute", 1);
        }
    }
    public void MuteMusic()
    {
        if (MusicContinuity.GetMuteStatus() == true)
        {
            PlayerPrefs.SetInt("Music Mute", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Music Mute", 1);
        }
    }
    void SetBiome()
    {
        string deathBiome = "";
        if (levelGenerator.playerBiome == LevelGenerator.Biome.Bog)
        {
            deathBiome = "Bog";
        }
        if (levelGenerator.playerBiome == LevelGenerator.Biome.Cypress)
        {
            deathBiome = "Cypress";
        }
        if (levelGenerator.playerBiome == LevelGenerator.Biome.Amazon)
        {
            deathBiome = "Amazon";
        }
        PlayerPrefs.SetString("StartBiome", deathBiome);
    }
    IEnumerator WaitThenLoadScene(string sceneName)
    {
        Time.timeScale = 1;
        unpause = true;
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneName);
    }
}