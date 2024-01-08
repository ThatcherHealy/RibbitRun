using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScript : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject deathScene;
    public void RespawnButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        if (playerController.dead) 
        {
            Time.timeScale = 0;
            deathScene.SetActive(true);
        }
    }
}
