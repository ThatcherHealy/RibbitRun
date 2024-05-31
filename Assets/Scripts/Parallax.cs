using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float length, startPos;
    public GameObject cam;
    public float parallaxEffect;
    [SerializeField] float height;
    float defaultOffset = 23;
    LevelGenerator lg;
    PlayerController pc;
    Transform player;
    
    void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
        player = pc.transform;
        lg = FindFirstObjectByType<LevelGenerator>();
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + dist, height, transform.position.z);

        if(temp > startPos + length) 
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }

        if(!pc.transitioningBiome && !(player.position.y > ((lg.playerRefEndPoint.y + defaultOffset))))
        {
            height = lg.playerRefEndPoint.y + defaultOffset;
        }
        else
        {
            if(player.position.y > ((lg.playerRefEndPoint.y + defaultOffset) + 15))
            {
                height = (lg.playerRefEndPoint.y + defaultOffset) + (player.position.y - (lg.playerRefEndPoint.y + defaultOffset + 15));
            }
            else if (pc.transitioningBiome)
            {
                height = player.position.y - 20;
            }
        }
    }
}
