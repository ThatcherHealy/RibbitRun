using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SnailMovement : MonoBehaviour
{
    [SerializeField] float gravityStrength = 3f; // Strength of gravity force
    [SerializeField] GameObject sprite;
    [SerializeField] PolygonCollider2D col;
    public int direction;

    private GameObject targetObject; // Reference to the target GameObject
    Collider2D targetCollider;
    private Vector2 targetPoint;
    private Vector2 closestPoint;
    private Vector2 transformToPoint;
    private float speed;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        FindRock();

        direction = Random.Range(1, 3);
        speed = Random.Range(1, 4);
    }
    void FindRock()
    {
        //Finds all objects in the scene with the name tag "Rock" then makes the closest one targetObject
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        GameObject[] rocks = allGameObjects.Where(obj => obj.CompareTag("Rock")).ToArray();

        if (rocks.Length > 0)
            GetClosestRock(rocks);
    }
    GameObject GetClosestRock(GameObject[] rocks) //Finds the closest rock in the scene
    {
        float minDist = Mathf.Infinity;
        foreach (GameObject rock in rocks)
        {
            float dist = Vector3.Distance(rock.transform.position, transform.position);
            if (dist < minDist)
            {
                targetObject = rock;
                minDist = dist;
            }
        }
        return targetObject;
    }
    private void FixedUpdate()
    {
        if(targetObject != null)
        {
            FixSpriteRotation();
            StickToTargetPoint();
            RotateTowardsTargetCenter();
            Movement();
        }
        else
        { 
            FindRock(); 
        }
    }
    void FixSpriteRotation()
    {
        if (direction == 1)
        {
            sprite.transform.localEulerAngles = Vector3.zero;
        }
        if (direction == 2)
        {
            sprite.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    void Movement()
    {
        if (targetObject != null)
        {
            if (direction == 1)
            {
                rb.velocity = transform.right * speed; //Move right
            }
            else
            {
                rb.velocity = -transform.right * speed; //Move left
            }
        }
    }

    void StickToTargetPoint()
    {
        if (targetObject != null)
        {
            targetCollider = targetObject.GetComponent<PolygonCollider2D>();
            if (targetCollider != null)
                closestPoint = targetCollider.ClosestPoint(transform.position);

                // Cast a ray downwards to find the point on the target
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, closestPoint - (Vector2)transform.position);

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