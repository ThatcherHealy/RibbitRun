using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    private float lilypadMassMin;
    private float lilypadMassMax;
    private float logMassMin;
    private float logMassMax;
    private float waterLilyMassMin;
    private float waterLilyMassMax;
    GameObject obj;
    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        obj = gameObject;
        if(obj.transform.parent != null)
        {
            if (obj.transform.parent != null && obj.transform.parent.name == "WaterLily1(Clone)" || obj.transform.parent.name == "WaterLily2(Clone)" || obj.transform.parent.name == "WaterLily3(Clone)")
            {
                obj = rb.transform.parent.gameObject;
            }
        }

        if (obj.name == "Log" ||obj.name == "Log(Clone)" || obj.name == "AmazonLog(Clone)")
        {
            logMassMin = 75; logMassMax = 130;
            rb.mass = Random.Range(logMassMin, logMassMax);
        }
        else if (obj.name == "WaterLily1(Clone)" || obj.name == "WaterLily2(Clone)" || obj.name == "WaterLily3(Clone)")
        {
            waterLilyMassMin = 5; waterLilyMassMax = 7;
            rb.mass = Random.Range(waterLilyMassMin, waterLilyMassMax);
        }
        else
        {
            lilypadMassMin = 0.5f; lilypadMassMax = 1.5f;
            rb.mass = Random.Range(lilypadMassMin, lilypadMassMax);
        }
    }
}