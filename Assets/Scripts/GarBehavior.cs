using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarBehavior : MonoBehaviour
{
    [SerializeField] PredatorVision pv;
    [SerializeField] UnderwaterCheck uc;
    [SerializeField] PredatorGrab hitbox;
    [SerializeField] WaypointTurner turner;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] float passiveSpeed = 10;
    [SerializeField] float chaseSpeed = 30F;
    [SerializeField] float chaseInterval = 1.2f;
    [SerializeField] Transform target;

    Vector3 initialPosition;
    private Vector3[] waypoints = new Vector3[4];
    int currentWaypoint;

    bool forceApplied; //True after the force has been applied to the gar in the direction of its next waypoint
    bool chaseForceApplied; //True after the force has been applied to the gar in the direction of the player
    bool attacking; //True if the gar was hunting last frame
    bool attackDelayCompleted;
    void Start()
    {
        animator.enabled = false;
        passiveSpeed *= rb.mass;
        chaseSpeed *= rb.mass;
        SetWaypoints();
    }

    void Update()
    {
        if (waypoints[currentWaypoint] != null && Vector2.Distance(transform.position, waypoints[currentWaypoint]) < 5f)
        {
            ChooseNextWaypoint();
        }

        ReturnToWaypoints();

        if (hitbox.dead) //Makes the predator die and float to the surface when it gets poisoned
        {
            GetComponentInChildren<PolygonCollider2D>().gameObject.layer = 6;
            GetComponentInChildren<PolygonCollider2D>().gameObject.tag = "Grapplable";
            gameObject.layer = 6; //Ground
            rb.mass = 5;
            rb.gravityScale = 1;
            animator.enabled = false;
            transform.localScale = new Vector3(transform.localScale.x, -1, 1); // Flip the sprite
            Destroy(this);
        }

        //Make the gar fall down when out of water
        if (!hitbox.dead)
        {
            if (uc.underwater)
            {
                rb.gravityScale = 0;
            }
            else
            {
                rb.gravityScale = 5;
            }
        }

        if(pv.rangeLeft)
        {
            attackDelayCompleted = false;
            pv.rangeLeft = false;
        }
    }
    private void FixedUpdate()
    {
        if (!hitbox.grabbed)
        {
            LookAtVelocity();
            LockRotation(rb.velocity);
        }

        if (hitbox.grabbed && !hitbox.dead)
        {
            StartCoroutine(EatPlayer());
        }
        else
        {
            if (!pv.huntingMode) //Not hunting
            {
                rb.drag = 0.001f;
                if (!forceApplied)
                {
                    MoveTowardsWaypoint();
                }
            }
            else //Hunting
            {
                rb.drag = 0.6f;
                if (pv.frog != null)
                {
                    if (attackDelayCompleted && !chaseForceApplied)
                    {
                        MoveTowardsPlayer();
                    }
                }
            }
        }

        if (turner.hitMud || turner.hitSlideRight || turner.hitSlideLeft)
        {
            SetWaypoints();
        }
    }
    void LookAtVelocity()
    {
        Vector2 velocity = Vector2.zero;
        float angle;
        if (!pv.huntingMode)
        {
            if (rb.velocity != Vector2.zero)
                velocity = rb.velocity;
            else
                velocity = new Vector2(velocity.x, velocity.y);

            angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        }
        else
        {
            Vector2 target = pv.frog.position - transform.position;
            angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }

        // Rotate the GameObject to face the direction of velocity
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 180);

        if (pv.huntingMode)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.075f);
        else
            transform.rotation = targetRotation;

        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            transform.localScale = new Vector3(1, -1, 1); // Flip the sprite
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);  // Reset the sprite scale
        }
    }
    void MoveTowardsWaypoint()
    {
        Vector3 direction = waypoints[currentWaypoint] - transform.position;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * passiveSpeed, ForceMode2D.Impulse);
        forceApplied = true;
    }
    void MoveTowardsPlayer()
    {
        //Vector3 direction = pv.frog.position - transform.position;
        Vector3 direction = target.position - transform.position;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * chaseSpeed, ForceMode2D.Impulse);
        chaseForceApplied = true;
        StartCoroutine(LungeDelay(chaseInterval));
    }
    private IEnumerator LungeDelay(float lookDelay)
    {
        yield return new WaitForSeconds(lookDelay);
        chaseForceApplied = false;
        if (!forceApplied)
            forceApplied = true;
    }
    void ChooseNextWaypoint()
    {
        int randomAssignment = Random.Range(1, 3);
        if (currentWaypoint == 0 || currentWaypoint == 1) //Left Waypoints
        {
            if (randomAssignment == 1)
            {
                currentWaypoint = 2;
            }
            else
            {
                currentWaypoint = 3;
            }
        }
        else //Right Waypoints
        {
            if (randomAssignment == 1)
            {
                currentWaypoint = 0;
            }
            else
            {
                currentWaypoint = 1;
            }
        }

        forceApplied = false;
    }
    void ReturnToWaypoints()
    {
        //If the gar just stopped hunting, go back to waypoints
        if (!pv.huntingMode && attacking)
        {
            StartCoroutine(WaitThenReturn());
        }

        //If the gar just saw a player, wait to attack them
        if(pv.huntingMode && !attacking) 
        {
            StartCoroutine(InitialAttackDelay());
        }

        if (pv.huntingMode)
        {
            attacking = true;
        }
        else
        {
            attacking = false;
        }
    }
    IEnumerator WaitThenReturn()
    {
        yield return new WaitForSeconds(1.2f);
        forceApplied = false;
    }
    IEnumerator InitialAttackDelay()
    {
        yield return new WaitForSeconds(0.3f);
        attackDelayCompleted = true;
    }
    private void LockRotation(Vector3 target)
    {
        //Locks rotation to 20 degrees
        if (target.x < 0) //looking left
        {
            //Lock between 330 and 30
            if (transform.eulerAngles.z > 300 && transform.eulerAngles.z < 330)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 330);
            }
            else if (transform.eulerAngles.z < 180 && transform.eulerAngles.z >30)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 30);
            }
        }
        else //looking right
        {
            //Lock between 150 and 210
            if (transform.eulerAngles.z < 150)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 150);
            }
            if (transform.eulerAngles.z > 210)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 210);
            }
        }
    }

    void SetWaypoints()
    {
        if (!(turner.hitMud || turner.hitSlideRight || turner.hitSlideLeft))
        {
            initialPosition = transform.position;
        }
        else
        {
            if (turner.hitMud)
            {
                initialPosition = transform.position + new Vector3(0, 15, 0);
            }
            else if (turner.hitSlideRight)
            {
                initialPosition = transform.position + new Vector3(-50, 0, 0);
            }
            else
            {
                initialPosition = transform.position + new Vector3(50, 0, 0);
            }
            turner.hitMud = false;
            turner.hitSlideLeft = false;
            turner.hitSlideRight = false;
            ChooseNextWaypoint();
        }

        float xOffsetLeft = Random.Range(20, 45); float yOffsetDown = Random.Range(2, 5);
        waypoints[0] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y - yOffsetDown);

        xOffsetLeft = Random.Range(20, 45); float yOffsetUp = Random.Range(2, 5);
        waypoints[1] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y + yOffsetUp);

        float xOffsetRight = Random.Range(20, 45); yOffsetUp = Random.Range(2, 5);
        waypoints[2] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y + yOffsetUp);

        xOffsetRight = Random.Range(20, 45); yOffsetDown = Random.Range(2, 5);
        waypoints[3] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y - yOffsetDown);

        int randomWaypoint = Random.Range(0, waypoints.Length);
        currentWaypoint = randomWaypoint;
    }
    IEnumerator EatPlayer()
    {
        yield return new WaitForSeconds(0.3f);
        if (pv.frog != null && !hitbox.dead && ((FindFirstObjectByType<DeathScript>().dontRespawnPressed || FindFirstObjectByType<DeathScript>().respawnedOnce) && pv.frog.GetComponent<PlayerController>().eaten)) 
        {
                if (pv.frog.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Flip the sprite

            animator.enabled = true;
            animator.SetBool("Thrash", true);
        }
    }
}