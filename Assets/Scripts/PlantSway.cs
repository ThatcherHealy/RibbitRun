using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSway : MonoBehaviour
{
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision != null && collision.attachedRigidbody != null && rb != null)
        {
            rb.AddForce(collision.attachedRigidbody.velocity.normalized, ForceMode2D.Impulse);
        }
    }
}