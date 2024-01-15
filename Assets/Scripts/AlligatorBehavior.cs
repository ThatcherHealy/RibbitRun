using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlligatorBehavior : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject destructionPoint;
    bool active;
    void Start()
    {
       sprite.SetActive(false);
       destructionPoint.SetActive(false);
       active = false;
    }
    private void FixedUpdate()
    {
        if (player != null) 
        {
            Activate();
            SetPosition();
        }
    }
    void Activate()
    {
        if (player.position.x > 200 && !active)
        {
            sprite.SetActive(true);
            destructionPoint.SetActive(true);
            active = true;
        }
    }
    void SetPosition() 
    {
        if (transform.position.x < player.position.x - 200)
        {
            Vector3 targetPosition = new Vector3(player.position.x - 200, 0, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
    }
}