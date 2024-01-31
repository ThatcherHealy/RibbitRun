using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtons : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseIcon;
    [SerializeField] GameObject score;
    public bool pause;
    public void Pause()
    {
        pause = true;
        pauseMenu.SetActive(true);
        score.SetActive(false);
        pauseIcon.SetActive(false);
        Time.timeScale = 0;
    }
    public void Resume()
    {
        pause = false;
        pauseMenu.SetActive(false);
        score.SetActive(true);
        pauseIcon.SetActive(true);
        Time.timeScale = 1;
    }
}
