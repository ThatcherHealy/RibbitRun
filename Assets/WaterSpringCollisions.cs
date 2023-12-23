using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterSpringCollisions : MonoBehaviour
{
    public WaterSpring waterSpring;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player") && !waterSpring.cooldown)
        {
            waterSpring.cooldown = true;
            StartCoroutine(waterSpring.Cooldown());
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            var speed = rb.velocity;

            waterSpring.velocity += speed.y / waterSpring.resistance;
        } 
    }
}
