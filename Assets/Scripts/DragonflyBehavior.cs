using UnityEngine;

public class DragonflyBehavior : MonoBehaviour
{
    private Vector3[] waypoints = new Vector3[4];
    int currentWaypoint;
    float speed = 25f;

    [SerializeField] WaypointTurner turner;
    Vector3 initialPosition;

    public SpriteRenderer sprite;
    public Sprite green;
    public Sprite blue;
    public Sprite yellow;
    public Sprite red;

    void Start()
    {
        ChooseColor();
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

        if (turner.hitGroundLeft || turner.hitGroundRight)
        {
            SetWaypoints();
        }
    }
    void MoveTowardsWaypoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint], speed * Time.deltaTime);
    }
    void ChooseNextWaypoint()
    {
        int randomAssignment = Random.Range(1,3);
        if (currentWaypoint == 0 || currentWaypoint == 1) //Left Waypoints
        {
            if(randomAssignment == 1)
            {
                currentWaypoint = 2;
            }
            else
            {
                currentWaypoint = 3;
            }
        }
        else //Right Waypoints
        {
            if(randomAssignment == 1) 
            {
                currentWaypoint = 0;
            }
            else
            {
                currentWaypoint = 1;
            }
        }
    }
    void LookAtWaypoint()
    {
        Vector3 direction = waypoints[currentWaypoint] - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust sprite flip based on the direction
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.localScale = new Vector3(1, -1, 1); // Flip the sprite
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.localScale = new Vector3(1, 1, 1);  // Reset the sprite scale
        }
    }
    void SetWaypoints()
    {
        if (!(turner.hitGroundLeft || turner.hitGroundRight))
        {
            initialPosition = transform.position;
        }
        else
        {
            if (turner.hitGroundLeft)
            {
                initialPosition = transform.position + new Vector3(45, 10, 0);
            }
            else if (turner.hitGroundRight)
            {
                initialPosition = transform.position + new Vector3(-45, 10, 0);
            }
            turner.hitGroundLeft = false;
            turner.hitGroundRight = false;
            ChooseNextWaypoint();
        }

        float xOffsetLeft = Random.Range(15, 40); float yOffsetDown = Random.Range(2, 5);
        waypoints[0] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y - yOffsetDown);

        xOffsetLeft = Random.Range(15, 40); float yOffsetUp = Random.Range(1, 3);
        waypoints[1] = new Vector3(initialPosition.x - xOffsetLeft, initialPosition.y + yOffsetUp);

        float xOffsetRight = Random.Range(15, 40); yOffsetUp = Random.Range(1, 3);
        waypoints[2] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y + yOffsetUp);

        xOffsetRight = Random.Range(15, 40); yOffsetDown = Random.Range(2, 5);
        waypoints[3] = new Vector3(initialPosition.x + xOffsetRight, initialPosition.y - yOffsetDown);

        int randomWaypoint = Random.Range(0, waypoints.Length);
        currentWaypoint = randomWaypoint;
    }
    void ChooseColor()
    {
        int randomColor = Random.Range(1, 5);

            if (randomColor == 1)
                sprite.sprite = green;
            else if (randomColor == 2)
                sprite.sprite = blue;
            else if (randomColor == 3)
                sprite.sprite = yellow;
            else
                sprite.sprite = red;
        
    }
}