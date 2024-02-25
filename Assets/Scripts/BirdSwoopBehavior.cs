using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSwoopBehavior : MonoBehaviour
{
    [SerializeField] Transform flyAwayPosition;
    [SerializeField] PredatorGrab hitbox1;
    [SerializeField] PredatorGrab hitbox2;
    LevelGenerator levelGenerator;
    Vector3 endPoint;
    int speed = 50;
    private Transform player;
    private bool facingLeft;
    private void Start()
    {
        player = GameObject.Find("Frog").transform;
        FaceTheRightWay();
        levelGenerator = FindAnyObjectByType<LevelGenerator>();
    }
    private void Update()
    {
        endPoint = levelGenerator.playerRefEndPoint;
    }
    void FixedUpdate()
    {
        Swoop();
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
    void Swoop()
    {
        //Move
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        Vector3 lerpedPosition;
        float iSpeed = 1f;
        if (hitbox1.grabbed || hitbox2.grabbed)
        {
            lerpedPosition = Vector3.Lerp(transform.position, transform.position + new Vector3(0,10,0), Time.deltaTime * (iSpeed));
        }
        //When the bird passes the frog, it flies away
        else if ((facingLeft && flyAwayPosition.position.x < player.position.x) || (!facingLeft && flyAwayPosition.position.x > player.position.x))
        {
            lerpedPosition = Vector3.Slerp(transform.position, new Vector3(transform.position.x, (endPoint.y + 34.585f), 0), Time.deltaTime * 0.5f);
        }
        else
        {
            //Makes the bird swoop down when it is within 40x from the player
            if (Vector3.Distance(new Vector3(transform.position.x,0,0), new Vector3(player.position.x,0,0)) < 50)
            {
                //Aligns the y position of the beak exactly to the frog's y position when the frog is above the bird
                if (player.position.y > transform.position.y)
                {
                    lerpedPosition = Vector3.Lerp(transform.position, player.position, Time.deltaTime * (iSpeed + 2));
                }
                //Slowly moves the y position of the beak  to the frog's y position when the frog is below the bird
                else
                {
                    lerpedPosition = Vector3.Slerp(transform.position, player.position, Time.deltaTime * iSpeed);
                }
            }
            else
            {
                lerpedPosition = transform.position;
            }
        }

        //The bird can't go under the water
        if (lerpedPosition.y < (endPoint.y + 3.585f))
            lerpedPosition.y = (endPoint.y + 3.585f);

        //Sets the birds yPosition
        transform.position = new Vector3(transform.position.x, lerpedPosition.y, 0);
    }
}