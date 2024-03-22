using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorVision : MonoBehaviour
{
    public bool huntingMode;
    public Transform frog;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Player") && collision.name == "Frog") 
            {
                huntingMode = true;
                frog = collision.gameObject.transform;
            }
        }
    }
}