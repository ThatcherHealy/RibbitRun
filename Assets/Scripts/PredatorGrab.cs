using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PredatorGrab : MonoBehaviour
{
    Transform frog;
    [SerializeField] Transform grabArea;
    public bool grabbed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Player") && collision.name == "Frog")
            {
                frog = collision.gameObject.transform;
                grabbed = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if (grabbed) 
        {
            frog.position = grabArea.position;
            frog.gameObject.GetComponent<Rigidbody2D>().mass = 0;
        }
    }
}