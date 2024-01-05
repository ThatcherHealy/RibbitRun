using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePointDetector : MonoBehaviour
{
    public TongueLauncher tongueLauncher;
    public TongueLine tongueLine;
    public bool closeToGrapplePoint;
    HashSet<GameObject> touchingObjects = new HashSet<GameObject>();

    private void Update()
    {
        if (touchingObjects.Count > 0 || Vector2.Distance(transform.position, tongueLauncher.grapplePoint) < 2)
        {
            closeToGrapplePoint = true;
        }

        else if (touchingObjects.Count == 0)
        {
            closeToGrapplePoint= false;
        }

        //Dodges a bug where destroyed flies would continue to add to the touchingObjects
        if (tongueLauncher.grappleTarget == null)
        {
            touchingObjects.Clear();
        }
    }

    //End the grapple when the frog gets too close
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tongueLauncher.grappleTarget != null && (collision.gameObject == tongueLauncher.grappleTarget 
            || (collision.gameObject.transform.parent == tongueLauncher.grappleTarget.transform && collision.gameObject.tag != "NoSwim" && collision.gameObject.layer != 8)))
        {
            touchingObjects.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
         touchingObjects.Remove(collision.gameObject);   
    }
}