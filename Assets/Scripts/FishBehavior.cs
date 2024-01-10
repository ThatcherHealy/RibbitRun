using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    int speed = 25;
    private Transform player;
    private bool facingLeft;
    public Transform[] sprites;
    private void Start()
    {
        player = GameObject.Find("Frog").transform;
        if (transform.position.x > player.position.x)
            facingLeft = true;
        else
            facingLeft = false;
        if (!facingLeft)
        {
           transform.eulerAngles = new Vector3(0, 180, 0);
        }

    }
    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}