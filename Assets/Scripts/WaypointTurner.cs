using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTurner : MonoBehaviour
{
    public bool hitSlideLeft;
    public bool hitSlideRight;
    public bool hitMud;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.layer == 14) 
            {
                if (collision.transform.position.x < transform.position.x)
                {
                    hitSlideLeft = true;
                }
                else
                {
                    hitSlideRight = true;
                }
            }
            if (collision.gameObject.layer == 3)
            {
                hitMud = true;
            }
        }
    }
}
