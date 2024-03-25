using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Windows;
using UnityEditor.PackageManager;

public class DeathScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] LevelGenerator levelGenerator;
    [SerializeField] GameObject eatenDeathScene;
    [SerializeField] TextMeshPro killerText;
    [SerializeField] GameObject drownScene;
    [SerializeField] TextMeshPro youDrownedText;
    [SerializeField] TextMeshPro[] drownedText;
    [SerializeField] GameObject poisonedScene;
    [SerializeField] TextMeshPro youWerePoisonedText;
    [SerializeField] TextMeshPro[] poisonedText;
    [SerializeField] Image greenout;
    [SerializeField] GameObject pauseButton;

    string deathBiome;
    bool deathBiomeSetOnce;

    bool drownedFadeIn;
    bool poisonedFadeIn;
    public void RespawnButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        if (playerController.dead) 
        {
            pauseButton.SetActive(false); //Deactivate pause button
            if (!deathBiomeSetOnce) //Set biome
            {
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
                deathBiomeSetOnce = true;
            }

            if (playerController.eaten && !playerController.drowned && !playerController.poisoned) //eaten
            {
                eatenDeathScene.SetActive(true);
                if (playerController.killer != null)
                    killerText.text = AorAn(playerController.killer) + RemoveClone(playerController.killer).ToUpper();
            }
            if (playerController.drowned && !playerController.poisoned && !playerController.eaten) //drowned
            {
                drownScene.SetActive(true);
                StartCoroutine(BeginDrownFadeIn());
            }
            if (playerController.poisoned && !playerController.drowned && !playerController.eaten) //poisoned
            {
                poisonedScene.SetActive(true);
                Greenout();
            }
        }
        if (drownedFadeIn)
        {
            FadeInDrownScene();
        }
        if (poisonedFadeIn)
        {
            FadeInPoisonScene();
        }
    }
    static string AorAn(string name)
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
    IEnumerator BeginDrownFadeIn() 
    {
        yield return new WaitForSeconds(1);
        drownedFadeIn = true;
    }
    private void Greenout()
    {
        //Begins fade in when the player loses oxygen and fades out when the player gains oxygen
        greenout.color = new Color(greenout.color.r, greenout.color.g, greenout.color.b,
        Mathf.Clamp(greenout.color.a + 0.35f * Time.deltaTime, 0, 1));

        //Die after the blackout is opaque
        if (greenout.color.a >= 1)
        {
            FadeInPoisonScene();
        }
    }
    private void FadeInPoisonScene()
    {
        youWerePoisonedText.color = new Color(youWerePoisonedText.color.r, youWerePoisonedText.color.g, youWerePoisonedText.color.b, youWerePoisonedText.color.a + (1f * Time.deltaTime));

        if (youWerePoisonedText.color.a >= 0.7f)
        {
            foreach (TextMeshPro text in poisonedText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (1.5f * Time.deltaTime));
            }
        }
        if (poisonedText[0].color.a >= 1)
        {
            Time.timeScale = 0;
        }
    }
}
