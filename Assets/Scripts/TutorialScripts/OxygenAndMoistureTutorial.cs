using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenAndMoistureTutorial : MonoBehaviour
{
    [SerializeField] TutorialPhase2 tp;
    [SerializeField] PlayerController pc;
    [SerializeField] GameObject oxygenTutorial;
    [SerializeField] GameObject moistureTutorial;
    bool oxygenExplained;
    bool oxygenTutorialDone;
    bool stopTime;
    bool moistureExplained;
    private void LateUpdate()
    {
        if (stopTime)
        {
            Time.timeScale = 0f;
        }
    }
    private void FixedUpdate()
    {
        if (tp.phase2)
        {
            if (pc.isSwimming && !oxygenExplained)
            {
                oxygenExplained = true;
                StartCoroutine(StartOxygenTutorial());
            }
            if (oxygenTutorialDone && !moistureExplained && !pc.isSwimming) 
            {
                moistureExplained = true;
                StartCoroutine(StartMoistureTutorial());
            }
        }
    }
    IEnumerator StartOxygenTutorial()
    {
        yield return new WaitForSeconds(1);
        OxygenTutorial();
    }
    void OxygenTutorial()
    {
        stopTime = true;
        pc.enabled = false;
        oxygenTutorial.SetActive(true);
    }
    public void DisableOxygenTutorial()
    {
        stopTime = false;
        pc.enabled = true;
        oxygenTutorialDone = true;
        oxygenTutorial.SetActive(false);
    }

    IEnumerator StartMoistureTutorial()
    {
        yield return new WaitForSeconds(1);
        MoistureTutorial();
    }
    void MoistureTutorial()
    {
        stopTime = true;
        pc.enabled = false;
        moistureTutorial.SetActive(true);
    }
    public void DisableMoistureTutorial()
    {
        stopTime = false;
        pc.enabled = true;
        moistureTutorial.SetActive(false);
    }
}