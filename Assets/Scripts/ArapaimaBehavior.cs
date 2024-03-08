using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ArapaimaBehavior : MonoBehaviour
{
    private Transform player;
    private LevelGenerator lg;
    float yLevel;
    [SerializeField] float iSpeed = 2;
    Vector3 targetPosition;

    private void Start()
    {
        lg = FindAnyObjectByType<LevelGenerator>();
        player = GameObject.Find("Frog").transform;
    }
    void FixedUpdate()
    {
        yLevel = player.transform.position.y;

        if (yLevel > lg.playerRefEndPoint.y - 10)
        {
            yLevel = lg.playerRefEndPoint.y - 10;
        }

        targetPosition = new Vector3(transform.position.x, yLevel, 0);
        Vector3 lerpedPosition = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * iSpeed);
        transform.position = new Vector3(transform.position.x, lerpedPosition.y, 0);
    }
}
