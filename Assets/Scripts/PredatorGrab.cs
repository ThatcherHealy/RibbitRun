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
    public bool poisoned;
    bool poisonedOnce;
    public bool dead;
    bool alligator;
    private void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
        if ((transform.parent.transform.parent != null))
        {
            if (transform.parent.transform.parent.gameObject.name == "ALLIGATOR")
            {
                alligator = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pc == null)
            pc = FindFirstObjectByType<PlayerController>();

        if (!pc.eaten || alligator) //If the player hasn't been eaten already, they get eaten
        {
            if (collision != null)
            {
                
                if (collision.CompareTag("Player"))
                {
                    if (collision.name == "Frog")
                        frog = collision.gameObject.transform;
                    else
                        frog = collision.gameObject.transform.parent;

                    if (!poisoned)
                    {
                        grabbed = true;
                        frog.gameObject.GetComponent<Rigidbody2D>().mass = 0;
                    }
                }
            }
        }
    }
    private void Update()
    {
        //Starts the timer to release the grab in two seconds when the predator is poisoned
        if (poisoned && !poisonedOnce)
        {
            poisonedOnce = true;
            StartCoroutine(CancelGrab());
        }
    }
    private void FixedUpdate()
    {
        if (grabbed && !dead) //Keeps the player grabbed when the predator isn't dead
        {
            frog.position = grabArea.position;
        }
        if (dead) //Returns the player's mass to normal when they escape
        {
            frog.gameObject.GetComponent<Rigidbody2D>().mass = 3;
        }
    }
    IEnumerator CancelGrab()
    {
        //Releases the player after 2 seconds
        yield return new WaitForSeconds(2);
        grabbed = false;
        dead = true;
        frog.gameObject.GetComponent<Rigidbody2D>().mass = 3;
        frog.gameObject.GetComponent<Rigidbody2D>().AddForce((frog.position - transform.position).normalized * 15, ForceMode2D.Impulse);
    }
}
