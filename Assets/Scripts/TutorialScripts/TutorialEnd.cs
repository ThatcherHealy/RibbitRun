using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEnd : MonoBehaviour
{
    public static int scoreTransfer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scoreTransfer = FindFirstObjectByType<ScoreController>().score;
            PlayerPrefs.SetInt("Tutorial Complete", 1);
            SceneManager.LoadScene("GameScene");
        }
    }
}
