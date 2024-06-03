using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPhase3 : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    [SerializeField] GameObject warningTutorial;
    public bool phase3;
    [SerializeField] TutorialPredatorEvent tutorialPredatorEvent;
    public bool pastTheHeron;
    public bool predatorSpawned;
    bool stopTime;
    SFXManager sfx;

    private void Start()
    {
        sfx = FindFirstObjectByType<SFXManager>();
    }
    private void LateUpdate()
    {
        if (stopTime)
        {
            Time.timeScale = 0f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            phase3 = true;
            if(!predatorSpawned && !pastTheHeron) 
            {
                StartCoroutine(tutorialPredatorEvent.BirdEvent());
                StartCoroutine(WarningTutorial());
                predatorSpawned = true;
            }
        }
    }
    IEnumerator WarningTutorial()
    {
        yield return new WaitForSeconds(1);
        stopTime = true;
        pc.enabled = false;
        warningTutorial.SetActive(true);
        sfx.PlaySFX("Click");

    }
    public void DisableWarningTutorial()
    {
        stopTime = false;
        Time.timeScale = 1;
        pc.enabled = true;
        warningTutorial.SetActive(false);
        sfx.PlaySFX("Exit Click");

    }

}
