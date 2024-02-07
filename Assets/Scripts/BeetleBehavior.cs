using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleBehavior : MonoBehaviour
{
    private Vector3[] waypoints = new Vector3[4];
    int currentWaypoint;
    float speed = 25f;
    void Start()
    {
        SetWaypoints();
    }

    void Update()
    {
        if (waypoints[currentWaypoint] != null && Vector2.Distance(transform.position, waypoints[currentWaypoint]) < 0.1f)
        {
            ChooseNextWaypoint();
        }
    }
    private void FixedUpdate()
    {
        LookAtWaypoint();
        MoveTowardsWaypoint();
    }
    void MoveTowardsWaypoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint], speed * Time.deltaTime);
    }
    void ChooseNextWaypoint()
    {
        int randomAssignment = Random.Range(0, waypoints.Length);
        currentWaypoint = randomAssignment;
    }
    void LookAtWaypoint()
    {
        Vector3 direction = waypoints[currentWaypoint] - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (direction.x < 0)
        {
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angle -90, Vector3.forward);
        } 
    }
    void SetWaypoints()
    {
        Vector3 initialPosition = transform.position;

        float xOffsetLeft = Random.Range(2, 5); float yOffsetDown = Random.Range(2, 5);
        waypoints[0] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y - yOffsetDown);

        xOffsetLeft = Random.Range(2, 5); float yOffsetUp = Random.Range(2, 5);
        waypoints[1] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y + yOffsetUp);

        float xOffsetRight = Random.Range(2, 5); yOffsetUp = Random.Range(2, 5);
        waypoints[2] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y + yOffsetUp);

        xOffsetRight = Random.Range(2, 5); yOffsetDown = Random.Range(2, 5);
        waypoints[3] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y - yOffsetDown);

        int randomWaypoint = Random.Range(0, waypoints.Length);
        currentWaypoint = randomWaypoint;
    }
}
