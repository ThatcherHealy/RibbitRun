using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePointDetector : MonoBehaviour
{
    public TongueLauncher tongueLauncher;
    public TongueLine tongueLine;
    public bool closeToGrapplePoint;
    public HashSet<GameObject> touchingObjects = new();
    Collider2D colliderParentChildCollider;
    public bool bugHit;

    private void Update()
    {
        if (touchingObjects.Count > 0)
        {
            Detatch();
        }

        else if (touchingObjects.Count == 0 && !bugHit)
        {
            closeToGrapplePoint = false;
        }

        //Dodges a bug where destroyed flies would continue to add to the touchingObjects
        if (tongueLauncher.grappleTarget == null)
        {
            touchingObjects.Clear();
        }
    }
    void Detatch()
    {
        closeToGrapplePoint = true;
    }

    //End the grapple when the frog gets too close
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(tongueLauncher.grappleTarget != null && tongueLauncher.grappleTarget.layer != 8)
        {
            if ((collision.gameObject.tag != "NoSwim" && collision.gameObject.layer != 8 && tongueLauncher.grappleTarget != null && tongueLauncher.grappleTarget.transform != null)
            && (collision.gameObject.transform == tongueLauncher.grappleTarget.transform))
            {
                touchingObjects.Add(collision.gameObject);
            }
        }
        else 
        {
            if (tongueLauncher.grappleTarget != null && tongueLauncher.grappleTarget.transform.parent != null && collision.gameObject.transform.parent != null
            && collision.gameObject.transform.parent == tongueLauncher.grappleTarget.transform.parent)
            {
                colliderParentChildCollider = collision.gameObject.transform.parent.GetComponentInChildren<Collider2D>();
                touchingObjects.Add(colliderParentChildCollider.gameObject);
                bugHit = true;
                Detatch();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        touchingObjects.Remove(collision.gameObject);

        if(colliderParentChildCollider != null)
        {
            touchingObjects.Remove(colliderParentChildCollider.gameObject);
        }
    }
}