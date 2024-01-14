using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSwoopBehavior : MonoBehaviour
{
    [SerializeField] Transform flyAwayPosition;
    int speed = 50;
    private Transform player;
    private bool facingLeft;
    private void Start()
    {
        player = GameObject.Find("Frog").transform;
        FaceTheRightWay();

    }
    void FaceTheRightWay()
    {
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
        Swoop();
    }
    void Swoop()
    {
        //Move
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        Vector3 lerpedPosition;
        int iSpeed = 1;

        //When the bird passes the frog, it flies away
        if ((facingLeft && flyAwayPosition.position.x < player.position.x) || (!facingLeft && flyAwayPosition.position.x > player.position.x))
        {
            lerpedPosition = Vector3.Lerp(transform.position, new Vector3(0,30,0), Time.deltaTime * iSpeed);
        }

        else
        {
            //Aligns the y position of the beak exactly to the frog's y position when the frog is above the bird
            if (player.position.y > transform.position.y)
            {
                lerpedPosition = Vector3.Lerp(transform.position, player.position, Time.deltaTime * iSpeed + 100);
            }
            //Slowly moves the y position of the beak  to the frog's y position when the frog is below the bird
            else
            {
                lerpedPosition = Vector3.Lerp(transform.position, player.position, Time.deltaTime * iSpeed);
            }
        }

        //The bird can't go under the water
        if (lerpedPosition.y < -1)
            lerpedPosition.y = -1;

        //Sets the birds yPosition
        transform.position = new Vector3(transform.position.x, lerpedPosition.y, 0);
    }
}