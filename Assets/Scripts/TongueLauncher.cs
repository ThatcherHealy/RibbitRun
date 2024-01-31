using System.Collections;
using UnityEngine;

public class TongueLauncher : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public TongueLine tongueLine;
    public GrapplePointDetector grapplePointDetector;
    public PlayerController playerController;
    public PauseButtons pauseScript;

    [Header("Layers Settings:")]
    [SerializeField] private bool grappleToAll = false;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform player;
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;
    public LineRenderer lr;
    [SerializeField] Transform tongueRangeCircle;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D rb;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistance = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;
    public AnimationCurve launchCurve;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    [Header("My Variables")]
    public GameObject grappleTarget;
    public Vector3 dragStartPosition;
    public Vector3 dragEndPosition;
    Vector3 secondLinePoint;
    Touch touch;
    public bool touchEnded;
    public bool grapplePointIdentified = false;
    private Vector2 hitPoint;
    private Collider2D hitCollider;
    [HideInInspector] public Vector2 addedForce;
    [SerializeField] private LayerMask ignoreLayer;

    private void Start()
    {
        tongueLine.enabled = false;
        m_springJoint2D.enabled = false;
        touchEnded = false;
    }

    private void Update()
    {
        if (!playerController.dead && !pauseScript.pause)
        {
            GetTouch();
        }
        SlowTime();
        SwitchFromPointToTransform();
        KeepGrapplePointOnCollider();
        TransitionAimLine();
        DetatchWhenClose();
        Launch();
    }
    private void FixedUpdate()
    {
        if(lr.positionCount > 0) 
        {
            tongueRangeCircle.transform.localScale = new Vector3(maxDistance * 5.7333f, maxDistance * 6.466f, 0);
            tongueRangeCircle.gameObject.SetActive(true);
        }
        else
        {
            tongueRangeCircle.gameObject.SetActive(false);
        }
    }

    void GetTouch()
    {
        //Identify touch
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        //Get initial touch position
        if (touch.phase == TouchPhase.Began)
        {
            dragStartPosition = Camera.main.WorldToViewportPoint(touch.position);
            dragStartPosition.z = 0;
        }

        //Get current touch position
        dragEndPosition = Camera.main.WorldToViewportPoint(touch.position);
        dragEndPosition.z = 0;

        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Dragging();
        }

        if (touch.phase == TouchPhase.Ended && !playerController.isGrounded && !playerController.isSwimming && touchEnded)
        {
            //On first click, set grapple point then grapple
            if (touchEnded)
            {
                SetGrapplePoint();
            }
            touchEnded = false;
        }
    }

    void SlowTime()
    {
        //When aiming, slow down time
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            && !tongueLine.isGrappling && !playerController.isGrounded && !playerController.isSwimming
            && !pauseScript.pause)
        {
            touchEnded = true;
            Time.timeScale = 0.3f;
        }
        else if (!playerController.dead && !pauseScript.pause) //Resume time when tongue is fired or when the player is grounded
        {
            Time.timeScale = 1;
        }
    }
    void SwitchFromPointToTransform()
    {
        if (grapplePointIdentified && grappleTarget != null) //Switch the grapple point from a point on an object to the center of that object if it is prey
        {
            if ((grappleTarget.transform.gameObject.layer == 7 || grappleTarget.transform.gameObject.layer == 12 || grappleTarget.transform.gameObject.layer == 8))
            {
                grapplePoint = grappleTarget.transform.position;
            }
        }
    }
    void KeepGrapplePointOnCollider()
    {
        //Make the grapple point stay as the edge of the collider where the raycast hit
        if (grappleTarget != null && hitCollider != null && hitPoint != null && grapplePoint != null
            && grappleTarget.layer != 7 && grappleTarget.layer != 8 && grappleTarget.layer != 12)
        {
            grapplePoint = hitCollider.ClosestPoint(hitPoint);
            hitPoint = grapplePoint;
        }
    }
    void TransitionAimLine()
    {
        //Remove the aim line when grounded or swimming
        if ((playerController.isGrounded || playerController.isSwimming) && lr.positionCount > 0)
        {
            playerController.skipToJump = true;
            lr.positionCount = 0;
        }
    }

    void Dragging()
    {
        if (!playerController.isGrounded)
        {
            //Set the aim line to a clamped vector of the distance between the first click on the screen to the current dragging position
            lr.positionCount = 2;
            int maxDrag = 12;
            secondLinePoint = transform.position + (Vector3.ClampMagnitude((dragStartPosition - dragEndPosition), maxDrag));

            //Create the line
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, secondLinePoint);
        }
    }
    private void DetatchWhenClose()
    {
        //If the tongue gets too close to its target, it unattaches;
        if (grapplePointDetector.closeToGrapplePoint)
        {
            tongueLine.enabled = false;
            m_springJoint2D.enabled = false;
            rb.gravityScale = 1.2f;
            grapplePointDetector.bugHit = false;
        }
    }

    void SetGrapplePoint()
    {
        //Remove the aim line when the grapple is fired
        lr.positionCount = 0;

        //Direction of the first click on the screen to when the screen is released
        Vector2 distanceVector = dragStartPosition - dragEndPosition;

        if (Physics2D.Raycast(firePoint.position, distanceVector))
        {
            //Aim a raycast that starts at the player and is fired at the direction of the initial touch - the last touch
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector);

            if (_hit.collider.gameObject.CompareTag("Grapplable") || grappleToAll)
            {
                //If the hit object can be grappled to, grapple to it
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    hitPoint = _hit.point;
                    hitCollider = _hit.collider;
                    grappleTarget = _hit.collider.gameObject;

                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    tongueLine.enabled = true;
                    grapplePointIdentified = true;
                }
            }
        }
    }
    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 springDistanceVector = firePoint.position - gunHolder.position;
                    m_springJoint2D.distance = springDistanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:

                    int strength;
                    rb.gravityScale = 0;
                    strength = 20;

                    //Set a force towards the grapple target for detectors, and towards grapple point for everything else
                    if (grapplePointIdentified && grappleTarget != null && (grappleTarget.transform.gameObject.layer == 7 || grappleTarget.transform.gameObject.layer == 12 || grappleTarget.transform.gameObject.layer == 8))
                    {
                        addedForce = ((Vector2)grappleTarget.transform.position - (Vector2)transform.position).normalized * strength;

                    }
                    else
                    {
                        addedForce = (grapplePoint - (Vector2)transform.position).normalized * strength;
                    }

                    rb.velocity = addedForce;

                    break;
            }
        }
    }
    private void Launch()
    {
        if (launchToPoint && tongueLine.isGrappling)
        {
            if (launchType == LaunchType.Transform_Launch)
            {
                Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
                Vector2 targetPos = grapplePoint - firePointDistance;
                gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, launchCurve.Evaluate(Time.deltaTime) * launchSpeed);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        //Creates a gizmo that shows the grapple range
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}