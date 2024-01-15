using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;

public class CattailController : MonoBehaviour
{
    public Color color;
    public LineRenderer lr;
    public GameObject cattailTop;
    public Transform stemPoint;
    public Rigidbody2D rb;
    public CattailDestroyedSensor destroyedSensor;
    private TongueLauncher tongueLauncher;
    private GameObject player;
    public bool fixedPosition = true;
    public int extension = 2;
    Vector3 inwards;

    private float startDistance;
    Vector2 stemBeginning;
    bool forceApplied = false;
    void Start()
    {
        tongueLauncher = FindObjectOfType<TongueLauncher>();
        player = GameObject.Find("Frog");

        SetColor();
        FindStemBeginning();
    }

    void FindStemBeginning()
    {
        //Find the first point on the ground under where the lilypad spawns
        RaycastHit2D[] hit = Physics2D.RaycastAll(stemPoint.position, Vector2.down);

        bool hitMud = false;

        for (int i = 0; i < hit.Length; i++) //Stop searching when the raycast hits mud
        {
            if (hitMud == true) { break; }

            if (hit[i].collider.transform.gameObject.layer == 3)  //Returns true when the first mud (hit[i]) is detected
            {
                stemBeginning = hit[i].point;
                startDistance = Vector2.Distance(stemBeginning, cattailTop.transform.position);

                hitMud = true;
            }
        }

        if (stemBeginning != null && stemBeginning != Vector2.zero) 
        {
            lr.positionCount = 1;
            lr.SetPosition(0, stemBeginning);
        }
    }
    void Update()
    {
        if (cattailTop != null)
        {
            if (lr.positionCount != 2)
                lr.positionCount = 2;

            lr.SetPosition(1, stemPoint.position);

            LockPosition();
        }

        if (destroyedSensor.destroyed) 
        {
            //Particles
        } 
    }
    private void FixedUpdate()
    {
        if (cattailTop != null)
        {
            Sway();
            LockRotation();
        }
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
            stemPoint.position = cattailTop.transform.position - inwards.normalized * 5;

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
        float distance = Vector2.Distance(stemBeginning, cattailTop.transform.position);
        inwards = stemBeginning - (Vector2)cattailTop.transform.position;

        stemPoint.position = cattailTop.transform.position - inwards.normalized * extension;
        if (distance != startDistance)
        {
            //Sets a limit on how far the lilypad can be from the base of the stem
            inwards = inwards.normalized;
            inwards *= distance - startDistance;
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
    void SetColor()
    {
       lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
       float alpha = 1.0f;
       Gradient gradient = new Gradient();
       gradient.SetKeys(
           new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
           new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });
      lr.colorGradient = gradient;
    }
}