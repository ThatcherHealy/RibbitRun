using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    private float lilypadMassMin;
    private float lilypadMassMax;
    private float logMassMin;
    private float logMassMax;
    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (gameObject.name == "Log" || gameObject.name == "Log(Clone)" || gameObject.name == "AmazonLog(Clone)")
        {
            logMassMin = 75; logMassMax = 130;
            rb.mass = Random.Range(logMassMin, logMassMax);
        }
        else
        {
            lilypadMassMin = 0.5f; lilypadMassMax = 1.5f;
            rb.mass = Random.Range(lilypadMassMin, lilypadMassMax);
        }
    }
}