using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ArapaimaBehavior : MonoBehaviour
{
    [SerializeField] PredatorGrab grab;
    [SerializeField] Rigidbody2D rb;
    private Transform player;
    private LevelGenerator lg;
    float yLevel;
    [SerializeField] float iSpeed = 2;
    Vector3 targetPosition;

    private void Start()
    {
        lg = FindAnyObjectByType<LevelGenerator>();
        player = GameObject.Find("Frog").transform;
        FindFirstObjectByType<SFXManager>().PlaySFX("Fish Spawn");
    }
    private void Update()
    {
        if (grab.dead) //Makes the predator die and float to the surface when it gets poisoned
        {
            GetComponentInChildren<PolygonCollider2D>().gameObject.layer = 6;
            GetComponentInChildren<PolygonCollider2D>().gameObject.tag = "Grapplable";
            gameObject.layer = 6; //Ground
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 300;
            rb.gravityScale = 1;
            transform.localScale = new Vector3(transform.localScale.x, -1, 1); // Flip the sprite
            Destroy(GetComponent<FishBehavior>());
            Destroy(this);
        }
    }
    void FixedUpdate()
    {
        yLevel = player.transform.position.y;

        if (yLevel > lg.playerRefEndPoint.y - 10)
        {
            yLevel = lg.playerRefEndPoint.y - 10;
        }

        if (yLevel < lg.playerRefEndPoint.y - 32)
        {
            yLevel = lg.playerRefEndPoint.y - 32;
        }

        targetPosition = new Vector3(transform.position.x, yLevel, 0);
        Vector3 lerpedPosition = Vector3.Slerp(transform.position, targetPosition, Time.deltaTime * iSpeed);
        transform.position = new Vector3(transform.position.x, lerpedPosition.y, 0);
    }
}
