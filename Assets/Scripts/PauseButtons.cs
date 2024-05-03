using System.Collections;
using System.Collections.Generic;
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
    private void Start()
    {
        sfx = FindFirstObjectByType<SFXManager>();
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
        sfx.PlaySFX("Click");

        pause = false;
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
    }
    public void Restart()
    {
        sfx.PlaySFX("Click");
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
        SceneManager.LoadScene("GameScene");
    }
    public void TutorialRestart()
    {
        sfx.PlaySFX("Click");

        pause = false;
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
        SceneManager.LoadScene("Tutorial");
    }
    public void Home()
    {
        sfx.PlaySFX("Click");

        SceneManager.LoadScene("MainMenu");
        if (!tutorial) 
        {
            SetBiome();
            scoreController.CheckHighscore(scoreController.score);
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
}