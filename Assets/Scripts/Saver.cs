using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saver : MonoBehaviour
{
    PlayerController pc;
    [SerializeField] Vector2 rescueOffset;

    private void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {        
        if (collision.CompareTag("Player") && (collision.gameObject.name.Equals("Frog") || collision.gameObject.name.Equals("GrapplePointDetector")) && !pc.eaten)
        {
            if(collision.gameObject.name.Equals("Frog"))
                collision.transform.position = new Vector3(collision.transform.position.x + rescueOffset.x, collision.transform.position.y + rescueOffset.y, collision.transform.position.z);
            else
                collision.GetComponentInParent<PlayerController>().transform.position = new Vector3(collision.transform.position.x + rescueOffset.x, collision.transform.position.y + rescueOffset.y, collision.transform.position.z);
        }
    }
}
