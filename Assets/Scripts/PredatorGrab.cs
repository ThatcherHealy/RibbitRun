using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PredatorGrab : MonoBehaviour
{
    PlayerController pc;
    Transform frog;
    [SerializeField] Transform grabArea;
    public bool grabbed;
    bool locked;

    private void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!pc.eaten)
        {
            if (collision != null)
            {
                if (collision.CompareTag("Player"))
                {
                    grabbed = true;

                    if (collision.name == "Frog")
                        frog = collision.gameObject.transform;
                    else
                        frog = collision.gameObject.transform.parent;
                }
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
