using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSaver : MonoBehaviour
{
    PlayerController pc;
    private void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.gameObject.name.Equals("Frog") && !pc.eaten)
        {
            collision.transform.position = new Vector3(collision.transform.position.x + 15, collision.transform.position.y, collision.transform.position.z);
        }
    }
}
