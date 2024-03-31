using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    [SerializeField] GameObject jumpTutorial;

    private void Start()
    {
        
    }

    private void Update()
    {
        pc.saturated = true;
    }
}
