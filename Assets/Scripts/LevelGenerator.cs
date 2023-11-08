using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private const float spawnDistance = 200f;
    private const float preySpawnDistance = 150f;
    [SerializeField] private Transform startEndPoint;
    [SerializeField] private Transform startPreyEndPoint;
    [SerializeField] private Transform lilypad1;
    [SerializeField] private List<Transform> levelPartList;
    [SerializeField] private List<Transform> preyList;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform waterLevel;
    private Vector3 lastLevelEndPosition;
    private Vector3 lastPreyEndPosition;
    private int swarmSize;
    Transform lastPreyTransform;

    private void Awake()
    {
        lastLevelEndPosition = (Vector2)startEndPoint.position;
        lastPreyEndPosition = (Vector2)startPreyEndPoint.position;
        int startingSpawnLevelParts = 5;
        for (int i = 0; i<startingSpawnLevelParts; i++)
        {
            SpawnLevelPart();
        }
        int startingSpawnPrey = 6;
        for (int i = 0; i < startingSpawnPrey; i++)
        {
            SpawnPrey();
        }
    }
    private void FixedUpdate()
    {
        if (Vector2.Distance(player.transform.position, lastLevelEndPosition) < spawnDistance)
        {
            SpawnLevelPart();
        }
        if (Vector2.Distance(player.transform.position, lastPreyEndPosition) < preySpawnDistance)
        {
            SpawnPrey();
        }
    }

    private void SpawnPrey()
    {
        Transform chosenPrey;
        Transform lastPreyTransform;

        int minFlyXOffset = 20, maxFlyXOffset = 45, minFlyYOffset = 12, maxFlyYOffset = 20;
        int flyXOffset = Random.Range(minFlyXOffset, maxFlyXOffset), flyYOffset = Random.Range(minFlyYOffset, maxFlyYOffset);

        chosenPrey = preyList[0];

        lastPreyTransform = SpawnPrey(chosenPrey, new Vector2(lastPreyEndPosition.x, waterLevel.position.y) + new Vector2(flyXOffset, flyYOffset));

        lastPreyEndPosition = lastPreyTransform.position;

    }

    private void SpawnLevelPart()
    {
        Transform chosenLevelPart;
        Transform lastLevelPartTransform;

        //Range of the possible distances between a lilypad and the last spawned level part
        int minLilypadXOffset = 10, maxLilypadXOffset = 30, minLilypadYOffset = -2, maxLilypadYOffset = 2;
        int lilypadXOffset = Random.Range(minLilypadXOffset, maxLilypadXOffset), lilypadYOffset = Random.Range(minLilypadYOffset, maxLilypadYOffset);

        //Range of the possible distances between a log and the last spawned level part
        int minLogXOffset = 20, maxLogXOffset = 35, minLogYOffset = -2, maxLogYOffset = 0;
        int logXOffset = Random.Range(minLogXOffset, maxLogXOffset), logYOffset = Random.Range(minLogYOffset, maxLogYOffset);

        int levelPartCalc = Random.Range(0, 100);

        if (levelPartCalc <= 75)
        {
            //75% chance to spawn a lilypad
            chosenLevelPart = levelPartList[0];
        }
        else
        {
            //25% chance to spawn a log
            chosenLevelPart = levelPartList[1];
        }

        if (chosenLevelPart == levelPartList[0]) //Lillypad
        {
            lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, new Vector2(lastLevelEndPosition.x, waterLevel.position.y) + new Vector2(lilypadXOffset, lilypadYOffset));
        }
        else //Log
        {
            lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, new Vector2(lastLevelEndPosition.x, waterLevel.position.y) + new Vector2(logXOffset, logYOffset));
        }

        lastLevelEndPosition = lastLevelPartTransform.Find("EndPosition").position;
    }

    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPosition)
    {
        //Spawns a level part and then returns that parts transform
        Transform levelpartTransform = Instantiate(levelPart, spawnPosition, Quaternion.identity);
        return levelpartTransform;
    }
    private Transform SpawnPrey(Transform prey, Vector3 preySpawnPosition)
    {
        //Chooses a random swarm size
        int swarmSizeGenerator = Random.Range(0, 100); 
        swarmSize = Random.Range(0, 100);
        if (swarmSizeGenerator >= 95)
        {
            swarmSize = 5;
        }
        else if (swarmSizeGenerator >= 85)
        {
            swarmSize = 4;
        }
        else if (swarmSizeGenerator >= 65)
        {
            swarmSize = 3;
        }
        else if (swarmSizeGenerator >= 36)
        {
            swarmSize = 2;
        }
        else
        { 
            swarmSize = 1; 
        }

        //spawns flies equal to the swarm size
        for (int i = 0; i < swarmSize; i++) 
        {
            Vector3 offset = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            lastPreyTransform = Instantiate(prey, preySpawnPosition + offset, Quaternion.identity);
        }
        return lastPreyTransform;
    }
}