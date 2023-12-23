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
        if (touchingObjects.Count > 0)
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
        if (collision.gameObject == tongueLauncher.grappleTarget || collision.gameObject.transform.parent == tongueLauncher.grappleTarget)
        {
            touchingObjects.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
         touchingObjects.Remove(collision.gameObject);   
    }
}