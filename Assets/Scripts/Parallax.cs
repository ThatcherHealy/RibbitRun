using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] float length, startPos;
    public GameObject cam;
    public float parallaxEffect;
    [SerializeField] float height;
    
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
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
    }
}
