using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CattailTutorial : MonoBehaviour
{
    [SerializeField] GameObject cattailProgressBlocker;

    private void OnDestroy()
    {
        if(cattailProgressBlocker != null)
            cattailProgressBlocker.SetActive(false);
    }
}
