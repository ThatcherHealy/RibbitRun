using UnityEngine;

public class CattailController : MonoBehaviour
{
    public GameObject cattailTop;
    public Rigidbody2D rb;
    public CattailDestroyedSensor destroyedSensor;
    private TongueLauncher tongueLauncher;
    private GameObject player;
    public bool fixedPosition = true;
    Vector3 inwards;
    public GameObject stem;
    public HingeJoint2D stemHinge;

    private bool hitMud;
    private float startDistance;
    private float currentDistance;
    Vector2 stemBeginning;
    bool forceApplied = false;
    void Start()
    {
        tongueLauncher = FindObjectOfType<TongueLauncher>();
        player = GameObject.Find("Frog");

        FindStemBeginning();

    }

    void FindStemBeginning()
    {
        //Find the first point on the ground under where the lilypad spawns
        RaycastHit2D[] hit = Physics2D.RaycastAll(cattailTop.transform.position, Vector2.down);
        Debug.DrawRay(cattailTop.transform.position, Vector2.down);
        hitMud = false;

        for (int i = 0; i < hit.Length; i++) //Stop searching when the raycast hits mud
        {
            Debug.Log(hit[i].transform);
                if (hit[i].collider.transform.gameObject.layer == 3)  //Returns true when the first mud (hit[i]) is detected
                {
                    stemBeginning = hit[i].point;

                    hitMud = true;
                }
        }

        startDistance = Vector2.Distance(stemBeginning, cattailTop.transform.position);
    }
    void Update()
    {
        Debug.Log(hitMud);
        if (cattailTop != null && hitMud)
        {
            LockPosition();
        }

        if (destroyedSensor.destroyed) 
        {
            //Particles
        } 
    }
    private void FixedUpdate()
    {
        if (cattailTop != null && hitMud)
        {
            currentDistance = Vector2.Distance(stemBeginning, cattailTop.transform.position);
            inwards = stemBeginning - (Vector2)cattailTop.transform.position;
            Sway();
            SetStem();
            LockRotation();
        }
    }
    private void SetStem()
    {
        // Set the position of the GameObject (one end at point A)
        stem.transform.position = new Vector3((stemBeginning.x + cattailTop.transform.position.x) / 2, (stemBeginning.y + cattailTop.transform.position.y) / 2); //Midpoint

        // Calculate the rotation angle in radians
        float angle = Mathf.Atan2(inwards.y, inwards.x);

        // Convert the angle to degrees and set the rotation of the GameObject
        stem.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        // Optionally, you can scale the GameObject based on the distance between points A and B
        stem.transform.localScale = new Vector3(currentDistance, 0.17f, stem.transform.localScale.z);
    }
    void Sway()
    {
        bool right;
        if (player.transform.position.x - cattailTop.transform.position.x > 0)
            right = true;
        else
            right = false;

        if (tongueLauncher.grappleTarget != null && cattailTop.transform == tongueLauncher.grappleTarget.transform && !forceApplied)
        {
            fixedPosition = false;
            if (right && Vector2.Distance(new Vector2(player.transform.position.x, 0), new Vector2(cattailTop.transform.position.x, 0)) >= 8)
                rb.AddForce(Vector2.right * 20, ForceMode2D.Impulse);
            else if (!right && Vector2.Distance(new Vector2(player.transform.position.x, 0), new Vector2(cattailTop.transform.position.x, 0)) >= 8)
                rb.AddForce(Vector2.left * 20, ForceMode2D.Impulse);
            else
                rb.AddForce(Vector2.zero);

            forceApplied = true;
        }
    }

    public void LockPosition()
    {

        if (currentDistance != startDistance)
        {
            //Sets a limit on how far the lilypad can be from the base of the stem
            inwards = inwards.normalized;
            inwards *= currentDistance - startDistance;
            Debug.Log("Inwards = " + inwards + " Current Distance = " + currentDistance + " Start Distance = " + startDistance);
            cattailTop.transform.position += inwards;
        }

        //vector from this object towards the target location
        Vector3 vectorToTarget = (Vector2)cattailTop.transform.position - stemBeginning;

        // get the rotation that points the Z axis forward, and the Y axis 90 degrees away from the target
        // (resulting in the X axis facing the target)
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

        // changed this from a lerp to a RotateTowards because you were supplying a "speed" not an interpolation value
        cattailTop.transform.rotation = Quaternion.RotateTowards(cattailTop.transform.rotation, targetRotation, 40);
    }

   void LockRotation()
   {
        Vector3 eulerAngles = cattailTop.transform.eulerAngles;
        int range = 10, power = 3;

        //Locks the angle of the cattail to between -range and range
        //When the cattail reaches the range, it bounces to the center
        if (!((eulerAngles.z > 360 - range && eulerAngles.z <= 360) || eulerAngles.z < range))
        {
            if (eulerAngles.z > range && eulerAngles.z < 180)
            {
                cattailTop.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, range);
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.right * power, ForceMode2D.Impulse);
            }

            else if (!(eulerAngles.z > 360 - range && eulerAngles.z <= 360))
            {
                cattailTop.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 360 - range);
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.left * power, ForceMode2D.Impulse);
            }
        }

        //When the cattail resets to normal, it stops
        if (fixedPosition == true && ((!(eulerAngles.z > 360 - 1 && eulerAngles.z <= 360) && eulerAngles.z < 1 && eulerAngles.z < 180))
            /*&& tongueLauncher.grappleTarget != null && !tongueLine.isGrappling && cattailTop.transform == tongueLauncher.grappleTarget.transform*/)
        {
            rb.velocity = Vector2.zero;
        }
   }
}