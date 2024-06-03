using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TutorialPredatorEvent : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    [SerializeField] ScoreController sc;
    [SerializeField] LevelGenerator lg;
    [SerializeField] GameObject fishSwarmPrefab;
    [SerializeField] GameObject arapaimaPrefab;
    [SerializeField] GameObject heronPrefab;
    [SerializeField] GameObject falconPrefab;
    [SerializeField] GameObject warningPrefab;
    [SerializeField] Transform player;


    private Vector3 predatorSpawnPosition;
    public bool birdEvent;
    public int lowerScoreLimit = 50;
    bool spawned;
    bool falcon;

    private int warningTime = 3;
    public bool warningActive = false;
    private GameObject currentPredator;
    private GameObject warning;
    private Rect cameraRect;
    private Rect shrunkCameraRect;
    public IEnumerator BirdEvent()
    {
        birdEvent = true;
        int birdCooldownTime = 0;
        StartCoroutine(Cooldown(birdCooldownTime));

        SetSpawnPosition();

        Warning();
        yield return new WaitForSeconds(warningTime);

        currentPredator = Instantiate(heronPrefab, predatorSpawnPosition, Quaternion.identity);
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
        SetSpawnPosition();
        SetWarningPosition();

        if (pc.drowned || pc.poisoned) //Deactivate warning and predator when player drowns
        {
            if (warning != null)
                warning.SetActive(false);

            if (currentPredator != null)
                currentPredator.SetActive(false);
        }
    }

    void SetSpawnPosition()
    {
        if (birdEvent)
        {
             predatorSpawnPosition = new Vector2(player.position.x + 130, 6);
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
            if (falcon)
                yShrink = 0.8f;

            float shrunkWidth = cameraRect.width * xShrink;
            float shrunkHeight = cameraRect.height * yShrink;
            shrunkCameraRect = new Rect(smoothedCenter.x - shrunkWidth / 2, smoothedCenter.y - shrunkHeight / 2, shrunkWidth, shrunkHeight);

            //Finally, set the warning position to be where the predator is going to spawn from (before it's spawned), and then to where the predator is. All clamped within the shrunk rectangle
            Vector3 warningTarget;
            if (spawned)
            {
                if(currentPredator != null) 
                {
                    warningTarget = new Vector3(
Mathf.Clamp(currentPredator.transform.position.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
Mathf.Clamp(currentPredator.transform.position.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
                }
                else
                {
                    warningTarget = new Vector3(
                Mathf.Clamp(predatorSpawnPosition.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
                Mathf.Clamp(predatorSpawnPosition.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
                }
            }
            else
            {
                warningTarget = new Vector3(
                Mathf.Clamp(predatorSpawnPosition.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
                Mathf.Clamp(predatorSpawnPosition.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
            }

            //Set Position
            warning.transform.position = Vector3.Lerp(warning.transform.position, warningTarget, 100 * Time.deltaTime);

            //Set Scale
            float scale; float distance;
            if (spawned)
            {
                if(currentPredator != null) 
                {
                    distance = Mathf.Abs(Vector3.Distance(player.position, currentPredator.transform.position));
                }
                else
                    distance = Mathf.Abs(Vector3.Distance(player.position, predatorSpawnPosition));
            }
            else
            {
                distance = Mathf.Abs(Vector3.Distance(player.position, predatorSpawnPosition));
            }
            float normalizedDistance = Mathf.Clamp01(distance / 150f);
            if (falcon)
                normalizedDistance = Mathf.Clamp01(distance / 200f);

            // Use Mathf.Lerp to interpolate between 0.5 and 1 based on the normalized distance
            scale = Mathf.Lerp(0.5f, 1f, 1 - normalizedDistance);
            scale *= 1.5f;
            warning.transform.localScale = new Vector3(scale, scale, 1);

            //When the predator enters the cameraview, stop the warning
            if(currentPredator != null) 
            {
                if ((!falcon && (spawned && currentPredator.transform.position.x < (topRight.x + 5) && currentPredator.transform.position.x > (bottomLeft.x - 5)))
    || (falcon && (spawned && currentPredator.transform.position.y < (topRight.y + 5))))
                {
                    warningActive = false;
                    Destroy(warning);
                }
            }
        }
    }
    IEnumerator Cooldown(int cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        spawned = false;
        birdEvent = false;
        falcon = false;
    }
}
