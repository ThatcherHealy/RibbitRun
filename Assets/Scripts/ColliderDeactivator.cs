using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ColliderDeactivator : MonoBehaviour
{
    [SerializeField] Collider2D[] collliders;
    [SerializeField] float deactivateDistance;
    [SerializeField] bool y;
    Transform player;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
    }

    private void FixedUpdate()
    {
        if (!y)
        {
            // Deactivates hitbox when the player is within the range
            if (math.distance(transform.position.x, player.position.x) > deactivateDistance)
            {
                foreach(Collider2D col in collliders)
                {
                    if(col != null)
                        col.enabled = false;
                }
            }
            else
            {
                //Activates hitbox when the player is within the range
                foreach (Collider2D col in collliders)
                {
                    if (col != null)
                        col.enabled = true;
                }
            }
        }
        else
        {
            // Deactivates hitbox when the player is within the range
            if (math.distance(transform.position.y, player.position.y) > deactivateDistance)
            {
                foreach (Collider2D col in collliders)
                {
                    if (col != null)
                        col.enabled = false;
                }
            }
            else
            {
                //Activates hitbox when the player is within the range
                foreach (Collider2D col in collliders)
                {
                    if (col != null)
                        col.enabled = true;
                }
            }
        }
    }
}
