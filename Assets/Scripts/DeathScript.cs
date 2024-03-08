using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Windows;

public class DeathScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject eatenDeathScene;
    [SerializeField] TextMeshPro killerText;
    [SerializeField] GameObject drownScene;
    [SerializeField] TextMeshPro youDrownedText;
    [SerializeField] TextMeshPro[] drownedText;

    bool fadeIn;
    public void RespawnButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        if (playerController.dead) 
        {
            if (playerController.eaten) 
            {
                //Time.timeScale = 0;
                eatenDeathScene.SetActive(true);
                if (playerController.killer != null)
                killerText.text = aOrAn(playerController.killer) + RemoveClone(playerController.killer).ToUpper();
            }
            if (playerController.drowned) 
            {
                drownScene.SetActive(true);
                StartCoroutine(BeginFadeIn());
            }
        }
        if (fadeIn)
        {
            FadeInDrownScene();
        }
    }
    static string aOrAn(string name)
    {
        string a;
        if (name.Substring(0, 1).ToLower() == "a" || name.Substring(0, 1).ToLower() == "e" || name.Substring(0, 1).ToLower() == "i"
            || name.Substring(0, 1).ToLower() == "o" || name.Substring(0, 1).ToLower() == "u")
        {
            a = "AN ";
        }
        else
        {
            a = "A ";
        }
        return a;
    }
    static string RemoveClone(string name)
    {
        if (name.Substring(name.Length - 7, 7) == "(Clone)" )
        {
            return name.Substring(0, name.Length - 7);
        }
        else
        {
            return name;
        }
    }


    private void FadeInDrownScene()
    {
        youDrownedText.color = new Color(youDrownedText.color.r, youDrownedText.color.g, youDrownedText.color.b, youDrownedText.color.a + (0.3f * Time.deltaTime));

        if (youDrownedText.color.a >= 0.7f)
        {
            foreach(TextMeshPro text in drownedText)
            {
                text.color = new Color(text.color.r,text.color.g,text.color.b,text.color.a + (1.5f * Time.deltaTime));
            }
        }
        if (drownedText[0].color.a >= 1)
        {
            Time.timeScale = 0;
        }
    }
    IEnumerator BeginFadeIn() 
    {
        yield return new WaitForSeconds(1);
        fadeIn = true;
    }
}
