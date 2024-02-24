using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TongueLauncher tongueLauncher;
    [SerializeField] private TongueLine tongueLine;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer jumpLr;
    [SerializeField] private LineRenderer swimLr;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private PauseButtons pauseScript;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask slide;
    [SerializeField] GameObject tongueRangeCircle;
    [SerializeField] GameObject cattailParticles;
    [SerializeField] GameObject mudParticles;
    [SerializeField] GameObject splashParticles;

    [Header("States")]
    public bool isGrounded;
    public bool isSliding;
    public bool jump;
    public bool isSwimming;
    public bool saturated;
    public bool dead;
    public bool eaten;
    public bool drowned;
    public bool dried;

    [Header("Settings")]
    [SerializeField] bool conserveMomentum;
    [SerializeField] bool aimingJumpStopsMomentum;

    [HideInInspector] public bool skipToJump;
    [HideInInspector] public bool changeBiome;
    [HideInInspector] public bool transitionCamera;
    float defaultGravityScale;
    private float power = 6;
    private float maxDrag = 5;
    private bool draggingStarted = false;
    private bool cantSwim;
    Vector3 secondLinePoint;
    Vector3 draggingPos;
    Vector3 dragStartPos;
    Vector3 dragReleasePos;
    Touch touch;
    bool splashParticleCooldown;

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!dead && !pauseScript.pause) 
        {
            DetectInputs();
        }

        if (dead)
        {
            swimLr.positionCount = 0;
            jumpLr.positionCount = 0;
        }

        if (dried)
        {
            maxDrag = 1.5f;
        }
        else
        {
            maxDrag = 5;
        }

        if (drowned)
        {
            rb.velocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        Jump();
        GroundCheck();
        Swimming();
    }

    void GroundCheck()
    {
        float extraDistance = 0.35f;
        RaycastHit2D raycastHitGround = Physics2D.BoxCast(circleCollider.bounds.center,
        new Vector2(circleCollider.bounds.size.x * 0.4f, circleCollider.bounds.size.y * 0.7f), 0f, Vector2.down, extraDistance, ground);

        RaycastHit2D raycastHitSlide = Physics2D.BoxCast(circleCollider.bounds.center,
       new Vector2(circleCollider.bounds.size.x * 0.4f, circleCollider.bounds.size.y * 0.7f), 0f, Vector2.down, extraDistance, slide);

        if (raycastHitGround.collider != null || raycastHitSlide.collider != null)
        {
            isGrounded = true;
            if (raycastHitSlide.collider != null)
            {
                isSliding = true;
            }
        }
        else
        {
            isGrounded = false;
        }

        if (raycastHitSlide.collider == null)
        {
            isSliding = false;
        }


        if (isGrounded)
        {
            isSwimming = false;
            tongueLine.isGrappling = false;

            if (!conserveMomentum)
                rb.velocity = Vector2.zero;
        }
        else
        {
            jumpLr.positionCount = 0;
        }
    }
    void Swimming()
    {
        if (isSwimming)
        {
            float slowingFactor = 1f;
            rb.drag = slowingFactor;

            rb.gravityScale = defaultGravityScale / 2;

            tongueLine.isGrappling = false;
        }
        else //Remove the swim line and resume time when out of the water
        {
            if (!tongueLine.isGrappling)
                rb.gravityScale = defaultGravityScale;

            swimLr.positionCount = 0;
            rb.drag = 0;
        }
    }
    void DetectInputs()
    {
        if (Input.touchCount > 0 && (isGrounded || isSwimming))
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                draggingStarted = true;
                DragStart();
            }
            if (((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && draggingStarted) || skipToJump)
            {
                Dragging();
            }
            if ((touch.phase == TouchPhase.Ended && draggingStarted) || (touch.phase == TouchPhase.Ended && skipToJump))
            {
                DragRelease();
                draggingStarted = false;
            }
        }
    }
    void Jump() 
    {
        if (jump) 
        {
            if (isSwimming) //Swim
            {
                rb.velocity *= 0.3f;
                power = 5f;
            }
            else if (!isSwimming && !tongueLine.isGrappling) //Jump
            {
                rb.velocity = Vector2.zero;
                power = 6f;
            }

            Vector3 force = dragStartPos - dragReleasePos;
            Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power * rb.mass;
            rb.AddForce(clampedForce, ForceMode2D.Impulse);

            jump = false;
        }
    }
    void DragStart() 
    {
        dragStartPos = Camera.main.WorldToViewportPoint(touch.position); 
        dragStartPos.z = 0;
        if (isSwimming) 
        {
            swimLr.positionCount = 1;
            swimLr.SetPosition(0, transform.position);
        }
        else
        {
            jumpLr.positionCount = 1;
            jumpLr.SetPosition(0, transform.position);
        }
    }
    void Dragging() 
    {            
       draggingPos = Camera.main.WorldToViewportPoint(touch.position);
       draggingPos.z = 0;

        if (skipToJump)
            dragStartPos = tongueLauncher.dragStartPosition;

        int extraforce = 5;
        if (dried)
        {
            extraforce = 2;
        }
        secondLinePoint = transform.position + (Vector3.ClampMagnitude((dragStartPos - draggingPos), maxDrag + extraforce));
        
        if (isSwimming)
        {
            jumpLr.positionCount = 0;
            swimLr.positionCount = 2;
            swimLr.SetPosition(0, transform.position);
            swimLr.SetPosition(1, secondLinePoint);
        }
        else
        {
            if (!isSliding) 
            {
                if (aimingJumpStopsMomentum)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }

            swimLr.positionCount = 0;
            jumpLr.positionCount = 2;
            jumpLr.SetPosition(0, transform.position);
            jumpLr.SetPosition(1, secondLinePoint);
        }
    }
    void DragRelease () 
    {
        skipToJump = false;
        tongueLauncher.touchEnded = false;
        jumpLr.positionCount = 0;
        swimLr.positionCount = 0;
        dragReleasePos = draggingPos;
        dragReleasePos.z = 0;

        jump = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 12) //Prey
        {
            AddPreyScore(collision);

            //When grappling to prey continue momentum and destroy prey
            if (tongueLine.isGrappling && tongueLauncher.grappleTarget != null && collision.transform.parent == tongueLauncher.grappleTarget.transform)
            {
                tongueLauncher.grapplePointIdentified = false;
                rb.gravityScale = defaultGravityScale;

                power = 7;
                if (collision.gameObject.layer == 12)
                    power = 2;

                rb.AddForce(power * rb.mass * tongueLauncher.addedForce.normalized, ForceMode2D.Impulse);

                //Remove the aim line when the frog eats prey
                tongueLauncher.lr.positionCount = 0;

                //Cancel the grapple
                tongueLine.enabled = false;
                tongueLine.isGrappling = false;
                tongueLauncher.grapplePointIdentified = false;
                tongueLauncher.grappleTarget = null;
            }

            Destroy(collision.transform.parent.gameObject);
        }
        if (collision.gameObject.layer == 11) //Cattail
        {
            if (tongueLauncher.grappleTarget != null && collision.transform == tongueLauncher.grappleTarget.transform)
            {
                tongueLauncher.grapplePointIdentified = false;
                rb.gravityScale = defaultGravityScale;
                power = 25;

                //Spawn Cattail Particles
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                GameObject particles = Instantiate(cattailParticles, collision.ClosestPoint(transform.position), Quaternion.Euler(-angle, 90, 0));
                Destroy(particles, 3);

                // Launch the player up and to the right if they are coming from the left
                if (tongueLauncher.grappleTarget != null && (tongueLauncher.grappleTarget.transform.position.x - transform.position.x >= 0))
                    rb.AddForce((Vector2.one - new Vector2(0,0.4f)) * power * rb.mass, ForceMode2D.Impulse); // approx. 60 degree angle

                else //Launch the player up and to the left if they are coming from the right
                    rb.AddForce(new Vector2(-Vector2.one.x, new Vector2(0, 0.7f).y) * power * rb.mass, ForceMode2D.Impulse); // approx. -60 degree angle

                //Remove the aim line when the frog eats prey
                tongueLauncher.lr.positionCount = 0;

                //Cancel the grapple
                tongueLine.enabled = false;
                tongueLine.isGrappling = false;
                tongueLauncher.grapplePointIdentified = false;
                tongueLauncher.grappleTarget = null;

                //Destroy the cattail
                Destroy(collision.transform.gameObject);
            }
        }

        //When the player enters a no-swim-zone, they can't swim until they leave
        if (collision.gameObject.CompareTag("NoSwim"))
            cantSwim = true;

        //When the player gets hit by a predator, they die
        if (collision.gameObject.CompareTag("Predator"))
        {
            if (!drowned)
            {
                eaten = true;
                dead = true;
            }
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            if (!splashParticleCooldown && !isSwimming)
            {
                GameObject splash = Instantiate(splashParticles, (Vector3)collision.ClosestPoint(transform.position), Quaternion.Euler(-90, 0, 0));
                ParticleSystem.MainModule ps = splash.GetComponent<ParticleSystem>().main;
                ps.startSpeedMultiplier = Mathf.Abs((rb.velocity.y));

                Destroy(splash, 1);
                StartCoroutine(SplashCooldown());
            }
        }

        if (collision.gameObject.CompareTag("BiomeSwapper"))
        {
            changeBiome = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("CameraTransition"))
        {
            transitionCamera = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == 14) //When the player is on mud, it doesnt lose moisture
        {
            saturated = true;
        }
    }
    IEnumerator SplashCooldown()
    {
        splashParticleCooldown = true;
        yield return new WaitForSeconds(0.2f);
        splashParticleCooldown = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //The player swims when they are in water, not grounded, and not in a no-swim-zone
        if (collision.gameObject.tag == "Water")
        {
            if(!isGrounded && !cantSwim)
            {
                isSwimming = true;
            }
        }
        if (collision.gameObject.layer == 14 && isSliding) //When the player is on mud, it doesnt lose moisture
        {
            MudParticles(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //When the player leaves a no-swim-zone, they can swim
        if (collision.gameObject.CompareTag("NoSwim") && cantSwim)
            cantSwim = false;
        if (collision.gameObject.CompareTag("Water"))
            isSwimming = false;
        if (collision.gameObject.layer == 14) //When the player is on mud, it doesnt lose moisture
        {
            saturated = false;
        }
    }

    void AddPreyScore(Collider2D collision)
    {
        if (collision.transform.parent != null)
        {
            //if beetle, add 5
            if (collision.gameObject.transform.parent.name == "Beetle(Clone)")
            {
                scoreController.SpawnFloatingText(5, transform.position);
                scoreController.Score(5);
            }
            //if fly or waterstrider, add 10
            if (collision.gameObject.transform.parent.name == "Fly(Clone)" || collision.gameObject.transform.parent.name == "WaterStrider(Clone)")
            {
                scoreController.SpawnFloatingText(10, transform.position);
                scoreController.Score(10);
            }

            //if slug or snail, add 15
            else if (collision.gameObject.transform.parent.name == "Slug(Clone)" || collision.gameObject.transform.parent.name == "Snail(Clone)")
            {
                scoreController.SpawnFloatingText(15, transform.position);
                scoreController.Score(15);
            }

            //if dragonfly, add 25
            else if (collision.gameObject.transform.parent.name == "Dragonfly(Clone)")
            {
                scoreController.SpawnFloatingText(25, transform.position);
                scoreController.Score(25);
            }
        }
    }
    void MudParticles(Collider2D col)
    {
        Vector2 normal = (Vector2)transform.position - col.ClosestPoint(transform.position);
        float angle = Mathf.Atan2(normal.y,normal.x) * Mathf.Rad2Deg;
        if (rb.velocity.x > 0)
        {
            GameObject mp = Instantiate(mudParticles, col.ClosestPoint(transform.position), Quaternion.Euler(-angle - 90, 90, 0));
            ParticleSystem.MainModule ps = mp.GetComponent<ParticleSystem>().main;
            ps.startSpeedMultiplier = rb.velocity.magnitude;
            Destroy(mp, 1);
        }
        else if (rb.velocity.x < 0)
        {
            GameObject mp = Instantiate(mudParticles, col.ClosestPoint(transform.position), Quaternion.Euler(-angle + 90, 90, 0));
            ParticleSystem.MainModule ps = mp.GetComponent<ParticleSystem>().main;
            ps.startSpeedMultiplier = rb.velocity.magnitude;
            Destroy(mp, 1);
        }
    }
}