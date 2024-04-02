using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueTutorial : MonoBehaviour
{
    [SerializeField] TongueLine tl;
    [SerializeField] GameObject tongueProgressBlocker;

    private void Update()
    {
        if (tl.isGrappling)
        {
            gameObject.layer = 7;
        }
        else
        {
            gameObject.layer = 0;
        }
    }
    private void OnDestroy()
    {
        if(tongueProgressBlocker != null)
            tongueProgressBlocker.SetActive(false);

        TongueTutorial[] others = FindObjectsOfType<TongueTutorial>();
        foreach (TongueTutorial tutorialScript in others)
        {
            if(tutorialScript != null)
            {
                tutorialScript.gameObject.layer = 7;
                Destroy(tutorialScript);
            }
        }
    }
}