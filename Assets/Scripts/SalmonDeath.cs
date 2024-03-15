using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalmonDeath : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PredatorGrab grab;

    private void Update()
    {
        if (grab.poisoned)
        {
            Transform[] grabs = transform.parent.GetComponentsInChildren<Transform>();
            foreach (Transform grab in grabs)
            {
                if (grab.name == "Grab Box")
                {
                    if (grab.parent != transform) 
                    {
                        grab.tag = "Untagged";
                        Destroy(grab.GetComponent<PredatorGrab>());
                    }
                }
            }
        }
        if (grab.dead) //Makes the predator die and float to the surface when it gets poisoned
        {
            GetComponentInChildren<PolygonCollider2D>().gameObject.layer = 6;
            GetComponentInChildren<PolygonCollider2D>().gameObject.tag = "Grapplable";
            gameObject.layer = 6; //Ground
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 20;
            rb.gravityScale = 1;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, 1); // Flip the sprite
            Destroy(GetComponent<FishBehavior>());
            Destroy(this);
        }
    }
}
