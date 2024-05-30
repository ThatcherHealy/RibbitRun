using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlligatorBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] grabBoxes;
    [SerializeField] PlayerController pc;
    [SerializeField] LevelGenerator lg;
    [SerializeField] Transform player;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject destructionPoint;
    public int followDistance = 200;
    bool active;
    void Start()
    {
       sprite.SetActive(false);
       destructionPoint.SetActive(false);
       active = false;
    }
    private void FixedUpdate()
    {
        if (player != null && !pc.dead) 
        {
            Activate();
            SetPosition();
        }

        if (pc.dead) 
        {
            destructionPoint.SetActive(false);
        }
        else if(active)
        {
            destructionPoint.SetActive(true);
        }
    }
    void Activate()
    {
        if (player.position.x > 125 && !active)
        {
            if (sprite != null) 
                sprite.SetActive(true);

            destructionPoint.SetActive(true);
            active = true;
        }
    }
    void SetPosition() 
    {
        if (transform.position.x < player.position.x - followDistance)
        {
            Vector3 targetPosition = new Vector3(player.position.x - followDistance, lg.playerRefEndPoint.y + 4.585f, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }

        //Deactivates hitbox until the player gets close
        if (transform.position.x < player.position.x - (followDistance/4))
        {
            foreach (GameObject box in grabBoxes)
            {
                box.SetActive(false);
            }
        }
        else
        {
            //Activates hitbox when the alligator isn't moving
            foreach (GameObject box in grabBoxes)
            {
                box.SetActive(true);
            }
        }
    }
}