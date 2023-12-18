using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LilypadController : MonoBehaviour
{
    public GameObject stemPrefab;
    private GameObject stem;
    public StemController stemController;
    private int range = 40;
    void Start()
    {
        stem = Instantiate(stemPrefab, transform.position, Quaternion.identity);
    }
    private void Update()
    {
        //Locks the angle of the lilypad between -range and range
        Vector3 eulerAngles = transform.eulerAngles;
        if (!((eulerAngles.z > 360 - range && eulerAngles.z <= 360) || eulerAngles.z < range)) 
        {
            if (eulerAngles.z > range && eulerAngles.z < 180)
                transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, range);
            else if (!(eulerAngles.z > 360 - range && eulerAngles.z <= 360))
                transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 360 - range);
        }
    }
}