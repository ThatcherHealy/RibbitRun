using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SparrowBehavior : MonoBehaviour
{
    Rigidbody2D rb;
    TongueLauncher tl;
    [SerializeField] GameObject grappleDetector;
    Vector3 initialPosition;
    Vector3 nextPosition;
    Vector3 escapePosition;
    Vector3 exitPosition;
    Vector3 initialScale;
    bool run;
    bool escape;
    bool exit;
    bool stop;
    bool positionsSet;
    bool rotationSet;
    bool scared;
    [SerializeField] PredatorVision vision;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tl = FindFirstObjectByType<TongueLauncher>();
        initialPosition = transform.localPosition;

        //Initialize positions so the bird doesnt skip states
        nextPosition = initialPosition + new Vector3(5, 10);
        escapePosition = nextPosition + new Vector3(200, 20);
        exitPosition = escapePosition + new Vector3(0, 500);

        initialScale = transform.localScale;
        int chance = Random.Range(1, 3);
        if (chance == 2)
            transform.localScale = new Vector3(-initialScale.x,initialScale.y,initialScale.z);

        if(transform.parent != null)
        {
            if (transform.parent.name != "Log(Clone")
            {
                rb.freezeRotation = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (run || escape || exit) //Lock rotation
        {
            rb.transform.localRotation = Quaternion.identity;
            rb.freezeRotation = true;
        }

        if (!run && !escape && !exit) //Stay locked at initial position until the player gets near
        {
            transform.localPosition = initialPosition;
        }


        if (vision.huntingMode) //Run when player is within vision box
        {
            if(!positionsSet) 
            {
                run = true;

                //Chooses the direction for the bird to run
                if (vision.frog != null && vision.frog.position.x <= transform.position.x)
                {
                    nextPosition = initialPosition + new Vector3(5, 10);
                    escapePosition = nextPosition + new Vector3(200, 20);
                    exitPosition = escapePosition + new Vector3(0, 500);
                    transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
                }
                else
                {
                    nextPosition = initialPosition + new Vector3(-5, 10);
                    escapePosition = nextPosition + new Vector3(-200, 20);
                    exitPosition = escapePosition + new Vector3(0, 500);
                    transform.localScale = new Vector3(-Mathf.Abs(initialScale.x),initialScale.y,initialScale.z);
                }
                positionsSet = true;
            }
        }
        if (run && !escape && !exit) //Initial jump upward
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, nextPosition, Time.fixedDeltaTime * 2f);
        }


        if (Vector2.Distance(transform.localPosition, nextPosition) < 2f) //Start running faster once the bird gets to its next position
        {
            exit = false;
            run = false;
            escape = true;
        }
        if (escape && !run && !exit) //Fly away
        {
            if (!rotationSet) 
            {
                transform.localScale = new Vector3(-transform.localScale.x, initialScale.y, initialScale.z);
                rotationSet = true;
            }
            float power = 50;
            if (scared)
                power = 65;

            if(!stop)
                rb.velocity = (escapePosition - transform.localPosition).normalized * power;
        }

        if (Vector2.Distance(transform.localPosition, escapePosition) < 10f) //Destroy the bird when it's done running away
        {
            run = false;
            escape = false;
            exit = true; 
        }
        if (exit && !escape && !run)
        {
            //rb.velocity = Vector2.up * 50;
            if(!stop)
                rb.velocity = (exitPosition - transform.localPosition).normalized * 75;
            Destroy(gameObject, 2);
        }
    }
    private void Update()
    {
        if (tl.grappleTarget != null)
        {
            if (tl.grappleTarget == grappleDetector) //Slows down the bird when you grapple to it so it can't get away and makes it only grapplable once
            {
                stop = true;
                rb.velocity /= 1.01f;
                grappleDetector.tag = "Untagged";
                scared = true; //Speeds up the bird cause they're scared :(
            }
        }
        else
            stop = false;
    }
}