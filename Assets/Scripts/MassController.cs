using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    private int lilypadMassMin;
    private int lilypadMassMax;
    private int logMassMin;
    private int logMassMax;
    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (gameObject.name == "Log" || gameObject.name == "Log(Clone)")
        {
            logMassMin = 30; logMassMax = 120;
            rb.mass = Random.Range(logMassMin, logMassMax);
        }
        else
        {
            lilypadMassMin = 1; lilypadMassMax = 10;
            rb.mass = Random.Range(lilypadMassMin, lilypadMassMax);
        }
    }
}