using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PredatorEvents : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    [SerializeField] ScoreController sc;
    [SerializeField] GameObject fishSwarmPrefab;
    [SerializeField] GameObject birdPrefab;
    [SerializeField] GameObject warningPrefab;
    [SerializeField] Transform player;

    private Vector3 predatorSpawnPosition;
    private bool fishEvent;
    private int directionChance;
    private bool birdEvent;
    public int lowerScoreLimit = 50;
    private int checkInterval = 5;
    int max, startingValue = 4;
    private bool cooldown = false;
    bool spawned;

    private int warningTime = 3;
    private bool warningActive = false;
    private GameObject currentPredator;
    private GameObject warning;
    private Rect cameraRect;
    private Rect shrunkCameraRect;

    void Start()
    {
        StartCoroutine(DetermineSpawnTime());
        max = startingValue;
    }

    IEnumerator DetermineSpawnTime()
    {
        yield return new WaitForSeconds(checkInterval);
        if (sc.score > lowerScoreLimit && !cooldown && !pc.dead)
        {
            int chance = UnityEngine.Random.Range(1, max);
            if (chance == 1) //33% chance
            {
                max = startingValue;
                int eventChosen = UnityEngine.Random.Range(1, 3);
                if (eventChosen == 1)
                {
                    StartCoroutine(FishEvent());
                }
                else if (eventChosen == 2)
                {
                    StartCoroutine(BirdEvent());
                }
            }
            else
            {
                max--;
            }
        }
        //Loop
        StartCoroutine(DetermineSpawnTime());
    }

    private IEnumerator FishEvent()
    {
        fishEvent = true;
        int fishCooldownTime = 20;
        StartCoroutine(Cooldown(fishCooldownTime));

        directionChance = UnityEngine.Random.Range(1, 3);
        SetFishDirection();

        Warning();
        yield return new WaitForSeconds(warningTime);

        currentPredator = Instantiate(fishSwarmPrefab, predatorSpawnPosition, Quaternion.identity);
        spawned = true;

        //Destroy the fish after 15 seconds
        Destroy(currentPredator, 15);
    }
    private IEnumerator BirdEvent()
    {
        birdEvent = true;
        int birdCooldownTime = 20;
        StartCoroutine(Cooldown(birdCooldownTime));

        directionChance = UnityEngine.Random.Range(1, 3);
        SetFishDirection();

        Warning();
        yield return new WaitForSeconds(warningTime);

        currentPredator = Instantiate(birdPrefab, predatorSpawnPosition, Quaternion.identity);
        spawned = true;

        //Destroy the fish after 15 seconds
        Destroy(currentPredator, 15);

    }
    private void Warning()
    {
        warning = Instantiate(warningPrefab, predatorSpawnPosition, Quaternion.identity);
        warningActive = true;
    }

    void FixedUpdate() 
    {
        SetWarningPosition();

        if(pc.drowned) //Deactivate warning and predator when player drowns
        {
            if (warning != null)
                warning.SetActive(false);

            if (currentPredator != null)
                currentPredator.SetActive(false);
        }
    }

    void SetFishDirection() 
    {
        if (fishEvent)
        {
            if (directionChance == 1)
                predatorSpawnPosition = new Vector2(player.position.x + 100, -20);
            else
                predatorSpawnPosition = new Vector2(player.position.x - 100, -20);
        }
        else if (birdEvent)
        {
            if (directionChance == 1)
                predatorSpawnPosition = new Vector2(player.position.x + 100, 6);
            else
                predatorSpawnPosition = new Vector2(player.position.x - 100, 6);
        }
    }
    void SetWarningPosition() 
    {
        if (warningActive && warning != null)
        {
            //First, smooth the centerpoint of the camera
            Vector3 cameraCenter = Camera.main.transform.position;
            Vector3 smoothedCenter = cameraCenter;
            int smoothSpeed = 5;
            smoothedCenter = Vector3.Lerp(smoothedCenter, cameraCenter, smoothSpeed * Time.deltaTime);

            //Then, create a rectangle that matches the smoothed camera view
            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
            cameraRect = new(bottomLeft.x, bottomLeft.y, (topRight.x - bottomLeft.x), (topRight.y - bottomLeft.y));

            //Then, create a new rectangle that is shrunk on the x and y axis
            float xShrink = 0.85f, yShrink = 0.7f;
            float shrunkWidth = cameraRect.width * xShrink;
            float shrunkHeight = cameraRect.height * yShrink;
            shrunkCameraRect = new Rect(smoothedCenter.x - shrunkWidth / 2, smoothedCenter.y - shrunkHeight / 2, shrunkWidth, shrunkHeight);

            //Finally, set the warning position to be where the predator is going to spawn from (before it's spawned), and then to where the predator is. All clamped within the shrunk rectangle
            Vector3 warningTarget;
            if (spawned)
            {
                warningTarget = new Vector3(
              Mathf.Clamp(currentPredator.transform.position.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
              Mathf.Clamp(currentPredator.transform.position.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax),0);
            }
            else
            {
              warningTarget = new Vector3(
              Mathf.Clamp(predatorSpawnPosition.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
              Mathf.Clamp(predatorSpawnPosition.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
            }

            warning.transform.position = Vector3.Lerp(warning.transform.position, warningTarget, 100 * Time.deltaTime);

            //When the predator enters the cameraview, stop the warning
            if (spawned && currentPredator.transform.position.x < (topRight.x + 5) && currentPredator.transform.position.x > (bottomLeft.x - 5))
            {
                warningActive = false;
                Destroy(warning);
            }
        }
    }
    IEnumerator Cooldown(int cooldownTime)
    {
        cooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        spawned = false;
        fishEvent = false;
        birdEvent = false;
        cooldown = false;
    }
}