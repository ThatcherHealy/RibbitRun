using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Color brown;
    [SerializeField] SFXManager sfx;
    [SerializeField] private TongueLauncher tongueLauncher;
    [SerializeField] private TongueLine tongueLine;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer jumpLr;
    [SerializeField] private LineRenderer swimLr;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private PauseButtons pauseScript;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private OxygenAndMoistureController oxygenAndMoistureController;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask slide;
    [SerializeField] GameObject tongueRangeCircle;

    [SerializeField] GameObject cattailParticles;
    [SerializeField] GameObject mudParticles;
    [SerializeField] GameObject splashParticles;
    [SerializeField] GameObject eatParticles;
    [SerializeField] GameObject sparrowEatParticles;
    [SerializeField] GameObject dartFrogPoisonParticles;
    GameObject activeDartFrogPoisonParticles;
    public string biomeIn;
    public bool eatenByFalcon;

    [Header("States")]
    public bool isGrounded;
    public bool isSliding;
    public bool jump;
    public bool isSwimming;
    public bool saturated;
    public bool wet;
    public bool dead;
    public bool eaten;
    public string killer;
    public PredatorGrab killerGrab;
    public bool drowned;
    public bool dried;
    public bool poisoned;
    public bool invulnerable;
    [HideInInspector] public bool grabbedByPoisonedFalcon;
    [HideInInspector] public bool grabbedByPoisonedPredator;
    public enum Species { Default, Treefrog, Froglet, BullFrog, PoisonDartFrog };
    [HideInInspector] public bool poisonAvailable;

    [Header("Settings")]
    [SerializeField] float aimMultiplier = 2;
    public static Species species;
    public bool conserveMomentum;
    [SerializeField] bool aimingJumpStopsMomentum;

    [HideInInspector] public bool skipToJump;
    [HideInInspector] public bool changeBiome;
    [HideInInspector] public bool transitionCamera;
    float defaultGravityScale;
    private float power;
    private float jumpingPower;
    private float swimmingPower;
    private float maxJumpAimLineLength, initialMaxJumpAimLineLength, driedMaxJumpAimLineLength = 5;
    private float maxSwimAimLineLength, initialMaxSwimAimLineLength, driedMaxSwimAimLineLength = 5;
    private bool draggingStarted = false;
    private bool cantSwim;
    Vector3 secondLinePoint;
    Vector3 draggingPos;
    Vector3 dragStartPos;
    Vector3 dragReleasePos;
    Touch touch;
    bool splashParticleCooldown;
    public bool killerFinalized;
    GameObject currentPlant;

    [Header("Animation")]
    bool jumpAnimationPlaying;
    [SerializeField] Transform sprite;
    [SerializeField] Animator animator;
    string currentState;
    Vector3 initialSpriteScale;
    bool grappleRotationSet;
    bool wasSwimming;
    bool facingRight = true;
    string SLIDE = "FrogSlide";
    string IDLE = "FrogIdle";
    string JUMP = "FrogJump";
    string MIDAIR = "FrogMidair";
    string GRAPPLE = "FrogGrapple";
    string READY_SWIM = "FrogReadySwim";
    string SWIM = "FrogSwim";
    string MIDSWIM = "FrogMidswim";
    string STRAIGHT_JUMP = "FrogStraightJump";
    string STRAIGHT_GRAPPLE = "FrogStraightGrapple";

    private void Awake()
    {
        //Application.targetFrameRate = 60;
    }

    private void Start()
    {
        sfx = FindFirstObjectByType<SFXManager>();

        Time.timeScale = 1.0f;
        SetSpecies();
        ConfigureSpecies();
        initialMaxJumpAimLineLength = maxJumpAimLineLength;
        initialMaxSwimAimLineLength = maxSwimAimLineLength;

        defaultGravityScale = rb.gravityScale;
        rb.freezeRotation = true;
        if(levelGenerator != null) 
        {
            biomeIn = levelGenerator.biomeSpawning.ToString();
        }

        initialSpriteScale = sprite.localScale;
    }

    void Update()
    {
        if (!dead && !pauseScript.pause) 
        {
            DetectInputs();
            Jump();
        }

        if (dead)
        {
            swimLr.positionCount = 0;
            jumpLr.positionCount = 0;
        }

        if (drowned)
        {
            rb.velocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        GroundCheck();
        Swimming();
        AnimateFrog();
        SetDirection();
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

            if (!conserveMomentum && !jump)
            {
                rb.velocity = Vector2.zero;
            }
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
            if (!tongueLine.isGrappling)
            {
                float slowingFactor = 1f;
                rb.drag = slowingFactor;

                rb.gravityScale = defaultGravityScale / 2;

                tongueLine.isGrappling = false;
            }
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
            Vector3 force = dragStartPos - dragReleasePos;
            float fillPercentage;

            if (isSwimming) //Swim
            {
                fillPercentage = Mathf.InverseLerp(0, (maxSwimAimLineLength), (secondLinePoint - transform.position).magnitude);

                if (fillPercentage > 0.1f) //Don't do anything unless the drag was significant
                {
                    rb.velocity *= 0.3f;
                    power = swimmingPower;
                    ChangeAnimationState(SWIM);
                    sfx.PlaySFX("Swim");
                }
            }
            else //Jump
            {
                fillPercentage = Mathf.InverseLerp(0, (maxJumpAimLineLength), (secondLinePoint - transform.position).magnitude);

                if (fillPercentage > 0.1f) //Don't do anything unless the drag was significant
                {
                    rb.velocity = Vector2.zero;
                    power = jumpingPower;
                    ChangeAnimationState(JUMP);
                    sfx.PlaySFX("Jump");
                    StartCoroutine(JumpAnimationTimer());
                }
            }

            if (dried)
                power = 5;

            if(fillPercentage > 0.1f) //Don't do anything unless the drag was significant
            {
                Vector3 clampedForce = (force.normalized) * fillPercentage * (power) * rb.mass;
                rb.AddForce(clampedForce, ForceMode2D.Impulse);

                if (isSwimming)
                    SetSpriteRotation(secondLinePoint - transform.position, 0);
            }

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

        if (dried)
        {
            maxJumpAimLineLength = driedMaxJumpAimLineLength;
            maxSwimAimLineLength = driedMaxSwimAimLineLength;
        }        
        else
        {
            maxJumpAimLineLength = initialMaxJumpAimLineLength;
            maxSwimAimLineLength = initialMaxSwimAimLineLength;
        }


        if (isSwimming)
        {
            secondLinePoint = transform.position + Vector3.ClampMagnitude(((dragStartPos - draggingPos) * aimMultiplier), maxSwimAimLineLength);
            jumpLr.positionCount = 0;
            swimLr.positionCount = 2;
            swimLr.SetPosition(0, transform.position);
            swimLr.SetPosition(1, secondLinePoint);

            float fillPercentage = Mathf.InverseLerp(0, (maxSwimAimLineLength), (secondLinePoint - transform.position).magnitude);
            if(fillPercentage > 0.1f)
            {
                ChangeAnimationState(READY_SWIM);
            }
        }
        else
        {

            secondLinePoint = transform.position + Vector3.ClampMagnitude(((dragStartPos - draggingPos) * aimMultiplier), maxJumpAimLineLength);
            if (!isSliding) 
            {
                if(isGrounded)
                    ChangeAnimationState(IDLE);

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
    void SetSpecies() 
    {
        if (PlayerPrefs.GetString("Species") == "Default")
            species = Species.Default;
        else if (PlayerPrefs.GetString("Species") == "Tree Frog")
            species = Species.Treefrog;
        else if (PlayerPrefs.GetString("Species") == "Froglet")
            species = Species.Froglet;
        else if (PlayerPrefs.GetString("Species") == "Bullfrog")
            species = Species.BullFrog;
        else if (PlayerPrefs.GetString("Species") == "Poison Dart Frog")
            species = Species.PoisonDartFrog;
        else
        {
            PlayerPrefs.SetString("Species", "Default");
            species = Species.Default;
        }
    }

    void ConfigureSpecies()
    {
        switch (species) 
        {
            case Species.Default:
                jumpingPower = 28;
                swimmingPower = 28f;
                maxJumpAimLineLength = 10;
                maxSwimAimLineLength = 8;
                oxygenAndMoistureController.oxygenLossRate = 0.05f;
                oxygenAndMoistureController.moistureLossRate = 0.06f;
                tongueLauncher.baseMaxDistance = 25;
                break;
            case Species.Treefrog:
                jumpingPower = 35f;
                swimmingPower = 25;
                maxJumpAimLineLength = 12;
                maxSwimAimLineLength = 7;
                oxygenAndMoistureController.oxygenLossRate = 0.08f;
                oxygenAndMoistureController.moistureLossRate = 0.06f;
                tongueLauncher.baseMaxDistance = 33;
                tongueLauncher.grappleStrength = 25;
                break;
            case Species.Froglet:
                jumpingPower = 20;
                swimmingPower = 40f;
                maxJumpAimLineLength = 6;
                maxSwimAimLineLength = 13;
                oxygenAndMoistureController.oxygenLossRate = 0f;
                oxygenAndMoistureController.moistureLossRate = 0.15f;
                tongueLauncher.baseMaxDistance = 18;
                tongueLauncher.grappleStrength = 18;
                break;
            case Species.BullFrog:
                jumpingPower = 29;
                swimmingPower = 32;
                maxJumpAimLineLength = 10;
                maxSwimAimLineLength = 10;
                oxygenAndMoistureController.oxygenLossRate = 0.04f;
                oxygenAndMoistureController.moistureLossRate = 0.05f;
                tongueLauncher.baseMaxDistance = 30;
                break;
            case Species.PoisonDartFrog:
                jumpingPower = 32;
                swimmingPower = 29;
                maxJumpAimLineLength = 10;
                maxSwimAimLineLength = 10;
                oxygenAndMoistureController.oxygenLossRate = 0.05f;
                oxygenAndMoistureController.moistureLossRate = 0.06f;
                tongueLauncher.baseMaxDistance = 25;

                poisonAvailable = true;
                activeDartFrogPoisonParticles = Instantiate(dartFrogPoisonParticles, transform.position, Quaternion.identity, transform);
                break;
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////// ANIMATION //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void AnimateFrog()
    {
        if (eaten || grabbedByPoisonedPredator)
        {
            ChangeAnimationState(GRAPPLE);
        }
        else
        {
            //When the player is grounded, they are idle when their velocity is less than 1 and sliding otherwise
            if (isGrounded && !draggingStarted && !jumpAnimationPlaying)
            {
                if (Mathf.Abs(rb.velocity.x) <= 1f && !isSliding)
                {
                    ChangeAnimationState(IDLE);
                    sprite.rotation = Quaternion.identity;
                }
                else
                {
                    ChangeAnimationState(SLIDE);
                }
            }

            //Make the player always slide on slides
            if (isSliding && !jumpAnimationPlaying)
            {
                ChangeAnimationState(SLIDE);
            }

            //Open mouth while aiming
            if (tongueLauncher.aimingGrapple && !isGrounded && !isSwimming)
            {
                if (currentState == MIDAIR)
                    ChangeAnimationState(GRAPPLE);

                if (currentState == STRAIGHT_JUMP)
                    ChangeAnimationState(STRAIGHT_GRAPPLE);
            }

            //Open mouth while grappling, but only set rotation at the moment the frog begins grappling
            if (tongueLine.isGrappling)
            {
                ChangeAnimationState(STRAIGHT_GRAPPLE);

                if (!grappleRotationSet)
                {
                    SetSpriteRotation((Vector3)tongueLauncher.grapplePoint - transform.position, 0);
                    grappleRotationSet = true;
                }
            }
            else //When the grapple is done, resume midair animation
            {
                grappleRotationSet = false;
                if ((currentState == GRAPPLE || currentState == STRAIGHT_GRAPPLE) && !tongueLauncher.aimingGrapple)
                {
                    ChangeAnimationState(STRAIGHT_JUMP);
                }
            }

            //If you jump into the water, start MIDSWIM animation
            if (isSwimming && currentState != SWIM && currentState != READY_SWIM)
            {
                ChangeAnimationState(MIDSWIM);
            }

            //If you jump out of the water, start MIDJUMP animation
            if (!tongueLauncher.aimingGrapple && !isSwimming && !isSliding && (currentState == READY_SWIM || currentState == SWIM || currentState == MIDSWIM))
            {
                ChangeAnimationState(MIDAIR);
            }
            //If you jump out of the water while aiming grapple, start GRAPPLE animation
            if (tongueLauncher.aimingGrapple && !isSwimming && !isSliding && (currentState == READY_SWIM || currentState == SWIM || currentState == MIDSWIM))
            {
                ChangeAnimationState(GRAPPLE);
            }

            //Scrapped change that made slide go to straight jump when not grounded
            if(currentState == IDLE && !isGrounded)
            {
                if (tongueLine.isGrappling)
                    ChangeAnimationState(STRAIGHT_GRAPPLE);
                else
                    ChangeAnimationState(STRAIGHT_JUMP);
            } 

            //While in a straight animation state, rotate to match velocity
            if (currentState == STRAIGHT_JUMP || currentState == STRAIGHT_GRAPPLE)
            {
                if(rb.velocity.magnitude >= 5)
                    SetSpriteRotation((transform.position + (Vector3)rb.velocity) - transform.position, 0);
            }

            //Resets the rotation after you leave the water
            if (!isSwimming && wasSwimming)
            {
                sprite.rotation = Quaternion.identity;
                sfx.PlaySFX("Exit Water");
            }
            if(isSwimming && !wasSwimming)
            {
                sfx.PlaySFX("Splash");
            }
        }
        wasSwimming = isSwimming;
    }
    void SetDirection()
    {
        
        if (rb.velocity.x < -3f && facingRight)
        {
            // Player was moving in negative direction
            facingRight = false;
        }
        if (rb.velocity.x > 3f && !facingRight)
        {
            // Player was moving in positive direction
            facingRight = true;
        }

        if(facingRight)
        {
            sprite.localScale = new Vector3(-Mathf.Abs(initialSpriteScale.x), sprite.localScale.y, initialSpriteScale.z);
        }
        else
            sprite.localScale = new Vector3(Mathf.Abs(initialSpriteScale.x), sprite.localScale.y, initialSpriteScale.z);
    }
    void ChangeAnimationState(string newState) 
    {
        //Stop the same animation from interrupting itself
        if (currentState == newState) return;

        //Play the new animation
        animator.Play(newState);
        currentState = newState;

        //Fix the offset so the frog sits on the ground correctly
        if(currentState == IDLE) 
        {
            sprite.transform.localPosition = new Vector3(0, 0.4f, 0);
            sprite.transform.localScale = new Vector3(initialSpriteScale.x, Mathf.Abs(initialSpriteScale.y), 1);
        }
        else if (currentState == SLIDE) 
        {
            sprite.transform.localPosition = new Vector3(0, -0.2f, 0);
            sprite.transform.localScale = new Vector3(initialSpriteScale.x, Mathf.Abs(initialSpriteScale.y), 1);
        }
        else
        {
            sprite.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    void SetSpriteRotation(Vector3 target, float offset) 
    {
        float angle;
        angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        if (rb.velocity.x < 0f)
        {
            angle -= 180;
        }
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + offset);
        sprite.transform.rotation = targetRotation;

        if (sprite.transform.eulerAngles.z > 90 && sprite.transform.eulerAngles.z < 270)
        {
            sprite.transform.localScale = new Vector3(1, -1, 1); // Flip the sprite
        }
        else
        {
            sprite.transform.localScale = new Vector3(1, 1, 1);  // Reset the sprite scale
        }
        SetDirection();
    }

    IEnumerator JumpAnimationTimer() 
    {
        jumpAnimationPlaying = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 2);
        jumpAnimationPlaying = false;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////// COLLISIONS //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6 || collision.gameObject.layer == 13 || collision.gameObject.layer == 9) //ground, slugpath, or riverbed decoration
        {
            if(collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("Lilypad") || collision.gameObject.CompareTag("Lilypad"))
            {
                sfx.PlaySFX("Lilypad Land");
            }
            if ((collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("Log")) || collision.gameObject.CompareTag("Log"))
            {
                sfx.PlaySFX("Log Land");
            }
            if (collision.gameObject.GetComponent<CypressTag>() != null)
            {
                sfx.PlaySFX("Cypress Land");
            }
            if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("Rock") || collision.gameObject.CompareTag("Rock"))
            {
                sfx.PlaySFX("Rock");
            }
        }
        if (collision.gameObject.layer == 14 || collision.gameObject.layer == 3) //mud
        {
            sfx.PlaySFX("Mud Land");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 7 || collision.gameObject.layer == 12) 
            || (species == Species.BullFrog && collision.gameObject.CompareTag("BPrey"))
            || (species == Species.PoisonDartFrog && collision.gameObject.CompareTag("Poisonous"))) //Prey
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

            //When you eat a spider, don't destroy its web
            if (collision.gameObject.transform.parent.name == "Spider(Clone)" || collision.gameObject.transform.parent.name == "Spider")
            {
                Destroy(collision.gameObject);
                Destroy(collision.transform.parent.gameObject.GetComponentInChildren<BoxCollider2D>());
            }
            else
            {
                Destroy(collision.transform.parent.gameObject);
            }
        }
        if (collision.gameObject.layer == 11) //Cattail
        {
            if (tongueLauncher.grappleTarget != null && collision.transform == tongueLauncher.grappleTarget.transform)
            {
                tongueLauncher.grapplePointIdentified = false;
                rb.gravityScale = defaultGravityScale;
                power = 25;

                sfx.PlaySFX("Cattail");

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
            bool eatenByAlligator = false;
            if ((collision.transform.parent.transform.parent != null))
            {
                if (collision.transform.parent.transform.parent.gameObject.name == "ALLIGATOR")
                {
                    
                    eatenByAlligator = true;
                }
            }
            if (!drowned && (!poisonAvailable) && !invulnerable 
                || eatenByAlligator)
            {
                killerGrab = collision.gameObject.GetComponent<PredatorGrab>();

                if (!killerFinalized)
                {
                    if (collision.transform.parent.transform.parent != null)
                    {
                        killer = collision.transform.parent.transform.parent.gameObject.name;
                    }
                    else if (collision.transform.parent != null)
                    {
                        killer = collision.transform.parent.gameObject.name;
                    }
                    else
                    {
                        killer = collision.gameObject.name;
                    }
                }
                

                eaten = true;
                dead = true;
            }
            else if (poisonAvailable) 
            {
                poisonAvailable = false;
                collision.gameObject.tag = "Untagged";
                invulnerable = true;
                StartCoroutine(DartFrogPoison(collision));
            }
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            wet = true;
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
            biomeIn = levelGenerator.biomeSpawning.ToString();
            transitionCamera = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == 14) //When the player is on mud, it doesnt lose moisture
        {
            saturated = true;
        }

        //Die to poison spiders
        if (collision.gameObject.CompareTag("Poisonous") && species != Species.PoisonDartFrog)
        {
            sfx.PlaySFX("Poison");
            dead = true;
            poisoned = true;
        }

        //Plant SFX
        if ((collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.CompareTag("Plant")) || collision.gameObject.CompareTag("Plant"))
        {
            if(currentPlant == null)
            {
                sfx.PlaySFX("Plant");
                currentPlant = collision.gameObject.transform.parent.gameObject;
            }
            if(collision.gameObject.transform.parent.gameObject != currentPlant)
            {
                sfx.PlaySFX("Plant");
                currentPlant = collision.gameObject.transform.parent.gameObject;
            }

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
        //When sliding, match the sprite rotation to the ground
        if ((collision.gameObject.layer == 14 || collision.gameObject.layer == 6) && currentState == SLIDE)
        {
            Vector2 normal = (Vector2)transform.position - collision.ClosestPoint(transform.position);
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            sprite.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        //The player swims when they are in water, not grounded, and not in a no-swim-zone
        if (collision.gameObject.tag == "Water")
        {
            wet = true;
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
        {
            isSwimming = false;
            wet = false;
        }
        if (collision.gameObject.layer == 14) //When the player is on mud, it doesnt lose moisture
        {
            saturated = false;
        }
    }

    void AddPreyScore(Collider2D collision)
    {
        if (collision.transform.parent != null)
        {
            sfx.PlaySFX("Eat");
            //if beetle, add 5
            if (collision.gameObject.transform.parent.name == "Beetle(Clone)")
            {
                scoreController.SpawnFloatingText(5, transform.position, Color.white);
                scoreController.Score(5);

                if (PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
                {
                    PlayerPrefs.SetInt("InsectsEaten", PlayerPrefs.GetInt("InsectsEaten", 0) + 1);
                }
            }
            //if fly or waterstrider, add 10
            if (collision.gameObject.transform.parent.name == "Fly(Clone)" || collision.gameObject.transform.parent.name == "WaterStrider(Clone)")
            {
                scoreController.SpawnFloatingText(10, transform.position, Color.white);
                scoreController.Score(10);

                if (PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
                {
                    PlayerPrefs.SetInt("InsectsEaten", PlayerPrefs.GetInt("InsectsEaten", 0) + 1);
                }
            }
            //if slug or snail, add 15
            else if (collision.gameObject.transform.parent.name == "Slug(Clone)" || collision.gameObject.transform.parent.name == "Snail(Clone)")
            {
                scoreController.SpawnFloatingText(15, transform.position, Color.white);
                scoreController.Score(15);
            }

            //if minnow, add 15
            else if (collision.gameObject.transform.parent.name == "BogMinnow(Clone)" || collision.gameObject.transform.parent.name == "AmazonMinnow(Clone)")
            {
                if (PlayerPrefs.GetInt("treeFrogUnlocked") == 1)
                {
                    PlayerPrefs.SetInt("FishEaten", PlayerPrefs.GetInt("FishEaten", 0) + 1);
                }

                scoreController.SpawnFloatingText(15, transform.position, Color.white);
                scoreController.Score(15);
            }

            //If spider, add 20
            else if ((collision.gameObject.transform.parent.name == "Spider" || collision.gameObject.transform.parent.name == "Spider(Clone)"))
            {
                if (!collision.gameObject.transform.parent.gameObject.GetComponent<SpiderBehavior>().poisonous)
                {
                    scoreController.SpawnFloatingText(20, transform.position, Color.white);
                    scoreController.Score(20);
                }
                else //If poison dart frog eats a poisonous spider, add green 50
                {
                    if (species == Species.PoisonDartFrog)
                    {
                        scoreController.SpawnFloatingText(50, transform.position, Color.green);
                        scoreController.Score(50);
                    }
                }
            }

            //if dragonfly, add 25
            else if (collision.gameObject.transform.parent.name == "Dragonfly(Clone)")
            {
                scoreController.SpawnFloatingText(25, transform.position, Color.white);
                scoreController.Score(25);

                if (PlayerPrefs.GetInt("frogletUnlocked", 0) == 1)
                {
                    PlayerPrefs.SetInt("InsectsEaten", PlayerPrefs.GetInt("InsectsEaten", 0) + 1);
                }
            }
            //if cichlid, add blue 50
            else if (collision.gameObject.transform.parent.name == "Cichlid(Clone)")
            {
                scoreController.SpawnFloatingText(50, transform.position, Color.blue);
                scoreController.Score(50);
            }
            //if bird, add brown 50
            else if (collision.gameObject.transform.parent.name == "Sparrow(Clone)")
            {
                scoreController.SpawnFloatingText(50, transform.position, brown);
                scoreController.Score(50);
            }
            //if goldfish, add yellow 100
            else if (collision.gameObject.transform.parent.name == "Goldfish(Clone)")
            {
                sfx.PlaySFX("Eat2");
                if (PlayerPrefs.GetInt("treeFrogUnlocked", 0) == 1)
                {
                    PlayerPrefs.SetInt("FishEaten", PlayerPrefs.GetInt("FishEaten", 0) + 1);
                }

                scoreController.SpawnFloatingText(100, transform.position, Color.yellow);
                scoreController.Score(100);
            }
            EatParticles(collision);
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
    void EatParticles(Collider2D col)
    {
        GameObject ep;
        if (col.gameObject.transform.parent.name == "Sparrow(Clone)")
            ep = Instantiate(sparrowEatParticles, col.transform.position, Quaternion.identity);
        else
            ep = Instantiate(eatParticles, col.transform.position, Quaternion.identity);

        Destroy(ep, 1);
    }
    IEnumerator DartFrogPoison(Collider2D col) 
    {
        activeDartFrogPoisonParticles.transform.SetParent(col.transform.parent);

        if (col.transform.parent.name == "Fish" || col.transform.parent.name == "Fish (1)" || col.transform.parent.name == "Fish (2)" 
            || col.transform.parent.name == "Fish (3)" || col.transform.parent.name == "Fish (4)" || col.transform.parent.name == "Fish (5)")
        {
            activeDartFrogPoisonParticles.transform.localPosition = new Vector3(-1.7f, 1.1f, 0);
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(8.5f, 4, 4);
        }
        else if (col.transform.parent.name == "Heron(Clone)" || col.transform.parent.name == "Heron")
        {
            activeDartFrogPoisonParticles.transform.localPosition = new Vector3(18.2f, -0.9f, 0);
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(6, 6, 6);
            activeDartFrogPoisonParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 11;
        }
        else if (col.transform.parent.name == "Gar(Clone)" || col.transform.parent.name == "Gar")
        {
            activeDartFrogPoisonParticles.transform.localPosition = Vector3.zero;
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(10, 3, 8);
            activeDartFrogPoisonParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 7;
        }
        else if (col.transform.parent.name == "Arapaima(Clone)" || col.transform.parent.name == "Arapaima")
        {
            activeDartFrogPoisonParticles.transform.localPosition = new Vector3(3, 1.47f, 0);
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(18.75f, 4.625f, 6);
            activeDartFrogPoisonParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 11;
        }
        else if (col.transform.parent.name == "Piranha(Clone)" || col.transform.parent.name == "Piranha")
        {
            activeDartFrogPoisonParticles.transform.localPosition = new Vector3(2, 0.54f, 0);
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(3, 3, 3);
            activeDartFrogPoisonParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 2;
        }
        else if (col.transform.parent.name == "Falcon(Clone)" || col.transform.parent.name == "Falcon" || col.transform.parent.name == "Sprite")
        {
            eatenByFalcon = true;
            activeDartFrogPoisonParticles.transform.localPosition = new Vector3(0.58f, -0.28f, 0);
            activeDartFrogPoisonParticles.transform.localScale = new Vector3(3, 6, 1.5f);
            activeDartFrogPoisonParticles.transform.localRotation = Quaternion.Euler(0,0,140);
            activeDartFrogPoisonParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = 12;
        }


        col.gameObject.GetComponent<PredatorGrab>().poisoned = true;
        yield return new WaitForSeconds(10);
        invulnerable = false;
    }
}