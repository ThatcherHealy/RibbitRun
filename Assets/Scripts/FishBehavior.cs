using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField] int speed = 50;
    private Transform player;
    private bool facingLeft;
    [SerializeField] PredatorTurner turner;
    LevelGenerator lg;
    bool turned;
    private void Start()
    {
        player = GameObject.Find("Frog").transform;
        lg = FindObjectOfType<LevelGenerator>();
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
    private void Update()
    {
        //Turn around when hitting an edge
        if (math.distance(transform.position.x, player.position.x) < 50 || transform.position.y > (lg.playerRefEndPoint.y + 5))
        {
            TurnAround();
            turner.active = true;
        }
        else
            turner.active = false;
    }
    void TurnAround()
    {
        if (turner.turnDirection.Equals("Right")) //If colliding from the left, flip right
        {
            if (!turned)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                turned = true;
            }
        }
        else if (turner.turnDirection.Equals("Left")) //If colliding from the right, flip left
        {
            if (!turned)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                turned = true;

            }
        }
    }

}