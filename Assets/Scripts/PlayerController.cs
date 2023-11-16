using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private TongueLauncher tongueLauncher;
    [SerializeField] private TongueLine tongueLine;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer jumpLr;
    [SerializeField] private LineRenderer swimLr;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private LayerMask ground;
    private float power = 5;
    private float maxDrag = 5;
    public bool skipToJump;

    Vector3 secondLinePoint;
    Vector3 draggingPos;
    Vector3 dragStartPos;
    Touch touch;

    //States
    public bool isGrounded;
    public bool isJumping;
    public bool isSwimming;

    private bool forceApplied;
    private bool draggingStarted = false;

    private bool cantSwim;

    private void Start()
    {
        isJumping = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        //Ground Check
        float extraDistance = 0.35f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(circleCollider.bounds.center,
        new Vector2(circleCollider.bounds.size.x * 0.4f, circleCollider.bounds.size.y * 0.7f), 0f, Vector2.down, extraDistance, ground);
        if (raycastHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //Jump Mechanic
        if (Input.touchCount > 0 && isGrounded)
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
        if (isGrounded && !forceApplied)
        {
            isJumping = false;
            rb.velocity = Vector2.zero;
        }

        //Swimming Mechanic
        if (Input.touchCount > 0 && isSwimming) //
        {
            tongueLauncher.lr.positionCount = 0;
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                draggingStarted = true;
                DragStart();
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Dragging();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                DragRelease();
                draggingStarted = false;
            }
        }
        if (isSwimming)
        {
            Time.timeScale = 1;
            int slowingFactor = 1;
            rb.drag = slowingFactor;
        }
        else //Remove the swim line and resume time when out of the water
        {
            swimLr.positionCount = 0;
            rb.drag = 0;
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
        
        secondLinePoint = transform.position + (Vector3.ClampMagnitude((dragStartPos - draggingPos), maxDrag + 5));
        
        if (isSwimming)
        {
            jumpLr.positionCount = 0;
            swimLr.positionCount = 2;
            swimLr.SetPosition(0, transform.position);
            swimLr.SetPosition(1, secondLinePoint);
        }
        else
        {
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
        Vector3 dragReleasePos = draggingPos;
        dragReleasePos.z = 0;

        if (isSwimming)
            rb.velocity *= 0.8f;

        Vector3 force = dragStartPos - dragReleasePos;
        Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power;
        rb.AddForce(clampedForce, ForceMode2D.Impulse);

        forceApplied = true;
        isJumping = true;


        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        //Used to make isJumping work correctly
        yield return new WaitForSeconds(0.1f);
        if (isGrounded)
            forceApplied = false;
        if (!isGrounded)
            StartCoroutine(Wait());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {     
            //Remove the aim line when the frog eats prey
            tongueLauncher.lr.positionCount = 0;

            //When grappling to prey, continue momentum and destroy prey
            if (tongueLine.isGrappling)
            {
                tongueLauncher.grapplePointIdentified = false;
                Destroy(collision.transform.parent.gameObject);

                rb.gravityScale = 1.2f;
                power = 5;
                rb.AddForce(rb.velocity.normalized * power, ForceMode2D.Impulse);

                tongueLine.enabled = false;
                tongueLine.isGrappling = false;
                tongueLauncher.grapplePointIdentified = false;
                tongueLauncher.grappleTarget = null;
            }
            else
            {
                Destroy(collision.transform.parent.gameObject);
            }
        }
        //When the player enters a no-swim-zone, they can't swim until they leave
        if (collision.gameObject.tag == "NoSwim")
            cantSwim = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //When the player leaves a no-swim-zone, they can swim
        if (collision.gameObject.tag == "NoSwim" && cantSwim)
            cantSwim = false;
        if (collision.gameObject.tag == "Water")
            isSwimming = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //The player swims when they are in water, not grounded, and not in a no-swim-zone
        if (collision.gameObject.tag == "Water" && !isGrounded && !cantSwim)
            isSwimming = true;
    }
}