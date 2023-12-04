using UnityEngine;

public class TongueLauncher : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public TongueLine tongueLine;
    public GrapplePointDetector grapplePointDetector;
    public PlayerController playerController;

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


    private void Start()
    {
        tongueLine.enabled = false;
        m_springJoint2D.enabled = false;
        touchEnded = false;
    }

    private void Update()
    {
        //Identify touch
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }
        //When aiming, slow down time
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && !tongueLine.isGrappling && !playerController.isGrounded && !playerController.isSwimming)
        {
            touchEnded = true;
            Time.timeScale = 0.3f;
        }
        //Resume time when tongue is fired or when the player is grounded
        if ((tongueLine.isGrappling || playerController.isGrounded) && Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        if (touch.phase == TouchPhase.Ended && !playerController.isGrounded && !playerController.isSwimming && touchEnded)
        {
            //On first click, set grapple point then grapple
            if (touchEnded)
            {
                SetGrapplePoint();
            }

            touchEnded = false;

            //On grapple, resume time
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }
        }

        if (launchToPoint && tongueLine.isGrappling)
        {
            if (launchType == LaunchType.Transform_Launch)
            {
                Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
                Vector2 targetPos = grapplePoint - firePointDistance;
                gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, launchCurve.Evaluate(Time.deltaTime) * launchSpeed);
            }
        }

        //If the tongue gets too close to its target, it unattaches;
        if (grapplePointDetector.closeToGrapplePoint)
        {
            tongueLine.enabled = false;
            m_springJoint2D.enabled = false;
            rb.gravityScale = 1.2f;
            tongueLine.isGrappling = false;
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
        //Debug.DrawRay(firePoint.position, dragStartPosition - dragEndPosition, Color.red, 1);

        if (grapplePointIdentified)
        {
            if (grappleTarget != null && grappleTarget.transform.gameObject.layer == 7) //Prey
            {
                grapplePoint = grappleTarget.transform.position;
            }
        }
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Dragging();
        }
        //Remove the aim line when grounded
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
            if (_hit.transform.gameObject.layer == 8) //Detector
            {
                //If the grapple latches onto the detector of an object, redirect it to the detector's parent
                grappleTarget = _hit.transform.parent.gameObject;
            }
            else
            {
                //Otherwise just identify what the grapple latched onto
                grappleTarget = _hit.transform.gameObject;
            }
            if (_hit.transform.gameObject.CompareTag("Grapplable") || grappleToAll)
            {
                //If the hit object can be grappled to, grapple to it
                if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistance || !hasMaxDistance)
                {
                    grapplePoint = _hit.point;
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


                    rb.gravityScale = 0;
                    if (grappleTarget.layer == 7) //Makes the frog dash through prey
                    {
                        rb.velocity = new Vector3((grappleTarget.transform.position.x - transform.position.x), (grappleTarget.transform.position.y - transform.position.y), 0).normalized * 10;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
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