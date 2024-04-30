using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalconBehavior : MonoBehaviour
{
    [SerializeField] PredatorGrab hitbox;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform sprite;
    LevelGenerator lg;
    Transform player;

    public bool diving = true;
    float diveSpeed = 65;
    float yVelocity;
    float xVelocity;

    Vector3 target;
    void Start()
    {
        lg = FindFirstObjectByType<LevelGenerator>();
        player = FindFirstObjectByType<PlayerController>().transform;
    }
    private void FixedUpdate()
    {
        SetTargetPosition();
        DetectEndOfDive();
        SetVelocity();

        if(diving) 
        {
            SetDirection();
        }
        LookAtVelocity();

    }

    void SetVelocity() 
    {
        // Calculate the difference in x position between falcon and target
        float xDifference = target.x - transform.position.x;

        // Calculate the time it would take to reach the target's x position
        float timeToReachTargetX = Mathf.Abs(xDifference) / diveSpeed;

        // Calculate the required horizontal velocity to reach the target's x position
        xVelocity = (xDifference / timeToReachTargetX)/3.5f;
        if (float.IsNaN(xVelocity))
            xVelocity = 0f;

        if (diving)
        {
            yVelocity = -diveSpeed;
        }
        else
        {
            yVelocity = diveSpeed;
        }

        rb.velocity = new Vector2(xVelocity, yVelocity);
    }

    private void SetTargetPosition()
    {
        if (diving)
        {
            target = player.position;
            if (target.y <= lg.playerRefEndPoint.y - 8) //Locks the target to a certain distance under the water
            {
                target = new Vector3(target.x, lg.playerRefEndPoint.y - 8, 0);
            }
        }
        else //Change the target position to past the frog so that the falcon flies upward past the frog
        {
            //scale x = 1 is left, -1 is right
            if (transform.localScale.x < 0)
            {
                target = new Vector3(player.position.x + 100, player.position.y);
            }
            else
            {
                target = new Vector3(player.position.x - 100, player.position.y);
            }
        }

    }

    void DetectEndOfDive()
    {
        if (hitbox.grabbed ) 
        {
            diving = false;
        }
        else
        {
            if (target.y <= lg.playerRefEndPoint.y)
            {
                if (transform.position.y < target.y - 5)
                {
                    diving = false;
                }
            }
            else
            {
                if (transform.position.y < lg.playerRefEndPoint.y - 5)
                {
                    diving = false;
                }
            }
        }
    }

    void SetDirection() 
    {
        if (diving) 
        {
            if (Mathf.Abs(transform.position.y - target.y) >= 40f) //High
            {
                if (Mathf.Abs(transform.position.x - target.x) <= 1f) //Directly above
                {
                    transform.localScale = transform.localScale;
                }
                else if (transform.position.x < target.x) //Approaching from the left
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else //Approaching from the right
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

        }
    }
    void LookAtVelocity()
    {
        Vector2 velocity = Vector2.zero;
        float angle = 0;
        
        
            if (rb.velocity != Vector2.zero)
                velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            else
                velocity = new Vector2(velocity.x, velocity.y);

            angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;


        if (!(diving && Mathf.Abs(transform.position.x - target.x) <= 3f))
        {
            if (transform.localScale.x < 0)
                sprite.rotation = Quaternion.Euler(0f, 0f, angle + 50);
            else
                sprite.rotation = Quaternion.Euler(0f, 0f, angle + 120);
        }
    }

    void Update()
    {
        if (hitbox.dead) //Makes the predator die and float to the surface when it gets poisoned
        {
            player.GetComponent<PlayerController>().eatenByFalcon = false;
            rb.velocity = new Vector2(-20, 0);
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(-15, 0);
            GetComponentInChildren<PolygonCollider2D>().gameObject.tag = "Grapplable";
            GetComponentInChildren<PolygonCollider2D>().gameObject.layer = LayerMask.NameToLayer("DeadPredator");
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 30;
            rb.gravityScale = 1;
            Destroy(hitbox.gameObject);
            Destroy(this);
        }
    }
}
