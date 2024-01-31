using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SlugMovement : MonoBehaviour
{

    private GameObject targetObject; // Reference to the target GameObject
    public float gravityStrength = 1f; // Strength of gravity force
    public GameObject sprite;
    public PolygonCollider2D col;
    private Vector2 targetPoint;
    private Vector2 closestPoint;
    private Vector2 transformToPoint;
    private int directionChance;
    private float speed;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Finds all objects in the scene with the name "Log(Clone)" then makes the closest one targetObject
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject[] logs = allGameObjects.Where(obj => obj.name == "Log(Clone)").ToArray();

       if (logs.Length > 0 )
            GetClosestLog(logs);

        directionChance = Random.Range(1, 3);
        speed = Random.Range(4, 6);
    }
    GameObject GetClosestLog(GameObject[] logs) //Finds the closest log in the scene
    { 
        float minDist = Mathf.Infinity;
        foreach (GameObject log in logs)
        {
            float dist = Vector3.Distance(log.transform.position, transform.position);
            if (dist < minDist)
            {
                targetObject = log;
                minDist = dist;
            }
        }
        return targetObject;
    }
    private void FixedUpdate()
    {
        FixSpriteRotation();
        StickToTargetPoint();
        RotateTowardsTargetCenter();
        Movement();
    }
    void FixSpriteRotation() 
    {
        if (directionChance == 1)
            sprite.transform.localEulerAngles = Vector3.zero;
        if (directionChance == 2)
            sprite.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    void Movement() 
    {
        if (targetObject != null && directionChance == 1)
            rb.velocity = transform.right * speed; //Move right
        else
            rb.velocity = -transform.right * speed; //Move left
    }

    void StickToTargetPoint()
    {
        if (targetObject != null)
        {
            Collider2D targetCollider = targetObject.GetComponent<PolygonCollider2D>();
            closestPoint = targetCollider.ClosestPoint(transform.position);

            // Cast a ray downwards to find the point on the target
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, closestPoint  - (Vector2)transform.position);

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.transform.gameObject == targetObject)  //Returns true when the target is found
                {
                    targetPoint = hit[i].point;
                    transformToPoint = (targetPoint - (Vector2)transform.position);

                    rb.AddForceAtPosition(transformToPoint.normalized * gravityStrength, targetPoint, ForceMode2D.Force);
                }
            }
        }
    }
    void RotateTowardsTargetCenter()
    {
        Collider2D targetCollider = targetObject.GetComponent<PolygonCollider2D>();
        if (targetObject != null)
        {
            // Calculate the direction towards the target
            Vector2 targetDirection = targetPoint - (Vector2)transform.position;
            
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

           transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetRotation.eulerAngles.z);
        } 
    }
}