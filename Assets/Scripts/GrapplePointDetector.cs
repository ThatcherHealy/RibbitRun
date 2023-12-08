using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePointDetector : MonoBehaviour
{
    public TongueLauncher tongueLauncher;
    public TongueLine tongueLine;
    public bool closeToGrapplePoint;
    public bool closeToGrappleTarget;
    HashSet<GameObject> touchingObjects = new HashSet<GameObject>();

    private void Update()
    {
        float range = 0.3f;

        if (Vector2.Distance(transform.position, tongueLauncher.grapplePoint) <= range)
        {
            closeToGrappleTarget = true;
        }
        else
        {
            closeToGrappleTarget= false;
        }

        if (touchingObjects.Count > 0 || closeToGrappleTarget)
        {
            closeToGrapplePoint = true;
        }
        else if (touchingObjects.Count == 0 && !closeToGrappleTarget)
        {
            closeToGrapplePoint= false;
        }
    }

    //End the grapple when the frog gets too close
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == tongueLauncher.grappleTarget && tongueLine.isGrappling)
        {
            touchingObjects.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
         touchingObjects.Remove(collision.gameObject);   
    }
}