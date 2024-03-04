using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinnowBehavior : MonoBehaviour
{
    [SerializeField] float passiveSpeed = 3;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] UnderwaterCheck uc;
    private Vector3[] waypoints = new Vector3[4];
    int currentWaypoint;
    bool forceApplied; //True after the force has been applied to the gar in the direction of its next waypoint
    void Start()
    {
        passiveSpeed *= rb.mass;
        SetWaypoints();
    }

    void Update()
    {
        if (waypoints[currentWaypoint] != null && Vector2.Distance(transform.position, waypoints[currentWaypoint]) < 5f)
        {
            ChooseNextWaypoint();
        }

        //Make the fish fall down when out of water
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
        LookAtVelocity();
        LockRotation(rb.velocity);
     
        rb.drag = 0.001f;
        if (!forceApplied)
        {
            MoveTowardsWaypoint();
        } 
    }
    void LookAtVelocity()
    {
        Vector2 velocity = Vector2.zero;
        float angle;
            if (rb.velocity != Vector2.zero)
                velocity = rb.velocity;
            else
                velocity = new Vector2(velocity.x, velocity.y);

            angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        // Rotate the GameObject to face the direction of velocity
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 180);
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
    private void LockRotation(Vector3 target)
    {
        //Locks rotation to 20 degrees
        if (target.x < 0) //looking left
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

        float xOffsetLeft = Random.Range(5, 30); float yOffsetDown = Random.Range(2, 5);
        waypoints[0] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y - yOffsetDown);

        xOffsetLeft = Random.Range(5, 30); float yOffsetUp = Random.Range(2, 5);
        waypoints[1] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y + yOffsetUp);

        float xOffsetRight = Random.Range(5, 30); yOffsetUp = Random.Range(2, 5);
        waypoints[2] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y + yOffsetUp);

        xOffsetRight = Random.Range(5, 30); yOffsetDown = Random.Range(2, 5);
        waypoints[3] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y - yOffsetDown);

        int randomWaypoint = Random.Range(0, waypoints.Length);
        currentWaypoint = randomWaypoint;
    }
}