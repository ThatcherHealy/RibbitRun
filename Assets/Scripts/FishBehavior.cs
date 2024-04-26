using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField] int speed = 50;
    private Transform player;
    private bool facingLeft;
    FishBehavior[] fishBehaviors = new FishBehavior[6];
    public bool otherFishTurned;
    [SerializeField] PredatorTurner turner;
    LevelGenerator lg;
    bool turned;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
        lg = FindObjectOfType<LevelGenerator>();
        if (transform.position.x > player.position.x)
            facingLeft = true;
        else
            facingLeft = false;
        if (!facingLeft)
        {
           transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if(transform.parent != null)
            fishBehaviors = transform.parent.GetComponentsInChildren<FishBehavior>();
    }
    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed * Time.fixedDeltaTime);
    }
    private void Update()
    {
        if(transform.parent != null && transform.parent.name == "Salmon(Clone)")
        {
            foreach (var fishBehavior in fishBehaviors)
            {
                //Repeatedly checks if any fish has turned
                if (fishBehavior.otherFishTurned)
                {
                    otherFishTurned = true;
                }
            }
        }

        //Turn around when hitting an edge, or if another fish has turned
        if (math.distance(transform.position.x, player.position.x) < 40 || transform.position.y > (lg.playerRefEndPoint.y + 5) || otherFishTurned)
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
                otherFishTurned = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
                turned = true;
            }
        }
        else if (turner.turnDirection.Equals("Left")) //If colliding from the right, flip left
        {
            if (!turned)
            {
                otherFishTurned = true;
                transform.eulerAngles = new Vector3(0, 180, 0);
                turned = true;
            }
        }
    }

}