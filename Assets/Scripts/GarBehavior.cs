using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarBehavior : MonoBehaviour
{
    [SerializeField] PredatorVision pv;
    [SerializeField] UnderwaterCheck uc;
    [SerializeField] PredatorGrab hitbox;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] float passiveSpeed = 10;
    [SerializeField] float chaseSpeed = 20;
    private Vector3[] waypoints = new Vector3[4];
    int currentWaypoint;

    bool lockedOnToWaypoint; //True when the gar is looking at next waypoint
    bool forceApplied; //True after the force has been applied to the gar in the direction of its next waypoint
    bool chaseForceApplied; //True after the force has been applied to the gar in the direction of the player
    bool attacking; //True if the gar was hunting last frame
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

        if (uc.underwater)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 5;
        }
    }
    private void FixedUpdate()
    {

        if (hitbox.grabbed)
        {
            StartCoroutine(EatPlayer());
        }
        else
        {
            if (!pv.huntingMode) //Not hunting
            {
                LockRotation(waypoints[currentWaypoint]);
                rb.drag = 0.001f;
                if (!forceApplied)
                {
                    if (!lockedOnToWaypoint)
                    {
                        LookAtWaypoint();
                    }
                    else //Apply force once locked on
                    {
                        MoveTowardsWaypoint();
                    }
                }
            }
            else //Hunting
            {
                rb.drag = 0.6f;
                if (pv.frog != null)
                {
                    LookAtPlayer();
                    if (!chaseForceApplied)
                    {
                        MoveTowardsPlayer();
                    }
                }
            }
        }
    }
    void LookAtWaypoint()
    {
        Vector3 direction = waypoints[currentWaypoint] - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust sprite flip based on the direction
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
            transform.localScale = new Vector3(1, 1, 1); // Flip the sprite
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
            transform.localScale = new Vector3(1, -1, 1);  // Reset the sprite scale
        }
        lockedOnToWaypoint = true;
    }
    void MoveTowardsWaypoint()
    {
        Vector3 direction = waypoints[currentWaypoint] - transform.position;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * passiveSpeed, ForceMode2D.Impulse);
        forceApplied = true;
    }
    void LookAtPlayer()
    {
        Vector3 direction = pv.frog.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Set rotation to look at frog
        Quaternion targetRotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.02f);

        LockRotation(pv.frog.position);

        //Flip the sprite when looking right so the sprite is always upright
        if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
        {
            transform.localScale = new Vector3(1, -1, 1); // Flip the sprite
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);  // Reset the sprite scale
        }
    }
    void MoveTowardsPlayer()
    {
        Vector3 direction = pv.frog.position - transform.position;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * chaseSpeed, ForceMode2D.Impulse);
        chaseForceApplied = true;
        StartCoroutine(LungeDelay(1.2f));
    }
    private IEnumerator LungeDelay(float lookDelay)
    {
        yield return new WaitForSeconds(lookDelay);
        chaseForceApplied = false;
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

        lockedOnToWaypoint = false;
        forceApplied = false;
    }
    void ReturnToWaypoints()
    {
        //If the gar just stopped hunting, go back to waypoints
        if (!pv.huntingMode && attacking)
        {
            lockedOnToWaypoint = false;
            forceApplied = false;
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
    private void LockRotation(Vector3 target)
    {
        //Locks rotation to 20 degrees
        if (target.x < transform.position.x) //looking left
        {
            //Lock between 340 and 20
            if (transform.eulerAngles.z > 300 && transform.eulerAngles.z < 340)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 340);
            }
            else if (transform.eulerAngles.z < 180 && transform.eulerAngles.z > 20)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 20);
            }
        }
        else //looking right
        {
            //Lock between 160 and 200
            if (transform.eulerAngles.z < 160)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 160);
            }
            if (transform.eulerAngles.z > 200)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 200);
            }
        }
    }

    void SetWaypoints()
    {
        Vector3 initialPosition = transform.position;

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
        if (pv.frog != null) 
        {
                if (pv.frog.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // Flip the sprite

        animator.enabled = true;
        animator.SetBool("Thrash", true);
        }
    }
}