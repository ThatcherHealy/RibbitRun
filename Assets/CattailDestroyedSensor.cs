using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CattailDestroyedSensor : MonoBehaviour
{
    public bool destroyed = false;
    private TongueLauncher tongueLauncher;
    private TongueLine tongueLine;
    private void Start()
    {
        tongueLauncher = FindObjectOfType<TongueLauncher>();
        tongueLine = FindObjectOfType<TongueLine>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && tongueLine.isGrappling && tongueLauncher.grappleTarget != null)
        {
            destroyed = true;
        }
    }
}
