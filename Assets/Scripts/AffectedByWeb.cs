using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedByWeb : MonoBehaviour
{
    bool stuck;
    Vector3 stickPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Web")) //bugs
            {
                stickPoint = collision.ClosestPoint(transform.position);
                stuck = true;

                if (transform.parent.name == "Fly(Clone)")
                {
                    Destroy(transform.parent.gameObject.GetComponent<FlyBehavior>());
                }
                if (transform.parent.name == "Dragonly(Clone)")
                {
                    Destroy(transform.parent.gameObject.GetComponent<DragonflyBehavior>());
                }
                if (transform.parent.name == "WaterStrider(Clone)")
                {
                    Destroy(transform.parent.gameObject.GetComponent<WaterStriderBehavior>());
                }
            }
        }
    }
    private void Update()
    {
        if (stuck) 
        {
            transform.position = stickPoint;
        }
    }
}
