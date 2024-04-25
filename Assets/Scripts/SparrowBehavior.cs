using System.Collections;
using UnityEngine;

public class SparrowBehavior : MonoBehaviour
{
    Rigidbody2D rb;
    TongueLauncher tl;
    [SerializeField] GameObject grappleDetector;
    [SerializeField] SparrowTurner turner;
    Vector3 initialPosition;
    Vector3 nextPosition;
    Vector3 escapePosition;
    Vector3 exitPosition;
    Vector3 initialScale;
    public bool run;
    public bool escape;
    public bool exit;
    bool stop;
    bool positionsSet;
    bool rotationSet;
    bool scared;
    bool startRunTimer;
    bool turned;
    [SerializeField] PredatorVision vision;

    [SerializeField] Transform sprite;
    [SerializeField] Animator animator;
    string currentState;
    string IDLE = "SparrowIdle";
    string LEAP = "SparrowLeap";
    string FLIGHT = "SparrowFlight";
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
        else
        {
            rb.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
        turner.active = true;
        TurnAround();

        if (run || escape || exit) //Lock rotation
        {
            rb.transform.localRotation = Quaternion.identity;
            rb.freezeRotation = true;
        }

        if (!run && !escape && !exit) //Stay locked at initial position until the player gets near
        {
            transform.localPosition = initialPosition;
            ChangeAnimationState(IDLE);
        }


        if (vision.huntingMode && !run && !escape && !exit) //Run when player is within vision box
        {
            ChangeAnimationState(LEAP);

            if (!positionsSet) 
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
            if(!startRunTimer)
            {
                StartCoroutine(RunTimer());
                startRunTimer = true;
            }
        }

        if (escape && !run && !exit) //Fly away
        {
            ChangeAnimationState(FLIGHT);
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
        if(tl != null)
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
    IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(0.7f);
        exit = false;
        run = false;
        escape = true;
    }
    void ChangeAnimationState(string newState)
    {
        //Stop the same animation from interrupting itself
        if (currentState == newState) return;

        //Play the new animation
        animator.Play(newState);
        currentState = newState;

        if(currentState == LEAP) 
        {
            sprite.localPosition = new Vector3(1.23f, -0.18f, 0);
        }
        else
        {
            sprite.localPosition = new Vector3(0.23f, -0.18f, 0);
        }
    }
    void TurnAround()
    {
        if(!turner.turnDirection.Equals("") && !turned)
        {
            if(transform.localScale.x < 0)
            {
                nextPosition = initialPosition - new Vector3(5, 0);
                escapePosition = nextPosition - new Vector3(200, 0);
                exitPosition = escapePosition - new Vector3(200, 0);
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
                turned = true;
            }
            else if (transform.localScale.x > 0)
            {
                nextPosition = initialPosition + new Vector3(5, 0);
                escapePosition = nextPosition + new Vector3(200, 0);
                exitPosition = escapePosition + new Vector3(200, 0);
                transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
                turned = true;
            }
        }
    }
}