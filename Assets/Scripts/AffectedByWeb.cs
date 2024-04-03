using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedByWeb : MonoBehaviour
{
    public bool stuck;
    Vector3 stickPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Web")) //bugs
            {
                stickPoint = collision.ClosestPoint(transform.position);
                stuck = true;

                if (transform.parent.GetComponent<FlyBehavior>() != null)
                {
                    Destroy(transform.parent.gameObject.GetComponent<FlyBehavior>());
                }
                if (transform.parent.GetComponent<DragonflyBehavior>() != null)
                {
                    Destroy(transform.parent.gameObject.GetComponent<DragonflyBehavior>());
                }
                if (transform.parent.GetComponent<WaterStriderBehavior>() != null)
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

            if(GetComponent<Rigidbody2D>() != null)
            {
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                GetComponent<Rigidbody2D>().gravityScale = 0;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            }
            if (transform.parent.GetComponent<Rigidbody2D>() != null)
            {
                transform.parent.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                transform.parent.GetComponent<Rigidbody2D>().gravityScale = 0;
                transform.parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            }
        }
    }
}
