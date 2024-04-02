using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPastHeron : MonoBehaviour
{
    [SerializeField] TutorialPhase3 tp3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tp3.pastTheHeron = true;
        }
    }
}
