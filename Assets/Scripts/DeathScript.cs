using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject eatenDeathScene;
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
                Time.timeScale = 0;
                eatenDeathScene.SetActive(true);
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
