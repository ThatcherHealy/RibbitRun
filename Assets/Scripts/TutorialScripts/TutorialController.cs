using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] OxygenAndMoistureController controller;
    public Transform respawnPosition;
    [SerializeField] Transform respawnPosition2;
    [SerializeField] TutorialPhase3 tp3;
    public bool protectConditions;

    private void Update()
    {
        if (protectConditions) 
        {
            controller.currentMoisture = 100;
            controller.currentOxygen = 100;
        }
        if (tp3.phase3)
        {
            respawnPosition.position = respawnPosition2.position;
        }
    }
}
