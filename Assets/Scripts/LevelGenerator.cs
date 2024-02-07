using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform startEndPoint;
    [SerializeField] private Transform startPreyEndPoint;
    [SerializeField] private Transform lilypad1;
    [SerializeField] private List<Transform> levelPartList;
    [SerializeField] private List<Transform> preyList;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform waterPrefab;
    [SerializeField] private Transform[] riverbedPrefabs;
    [SerializeField] private Transform cattailPrefab;
    [SerializeField] private Transform emptyTransformPrefab;
    [SerializeField] private Transform slugPrefab;
    [SerializeField] private Transform waterStriderPrefab;
    [SerializeField] private Transform dragonflyPrefab;

    private const float levelPartDistance = 100;
    private const float preySpawnDistance = 100;
    private const float waterSpawnDistance = 100;
    private const float riverbedSpawnDistance = 100;
    private const float cattailSpawnDistance = 100;

    private Vector3 lastLevelEndPosition;
    private Vector3 lastPreyEndPosition;
    private Vector3 lastWaterEndPosition;
    private Vector3 lastRiverbedEndPosition;
    private Vector3 lastCattailEndPosition;

    private int swarmSize;
    private int currentRiverbedIndex;
    private int levelPartCalc;
    Transform lastPreyTransform;
    Transform lastWaterTransform;
    Transform lastRiverbedTransform;
    Transform lastCattailTransform;
    const float waterLevel = -3.44f;
    public enum Biome {Bog,Cypress,Polluted};
    Biome currentBiome;

    private void Awake()
    {
        //Spawns the starting level
        lastLevelEndPosition = (Vector2)startEndPoint.position;
        lastPreyEndPosition = (Vector2)startPreyEndPoint.position;
        lastWaterEndPosition = new Vector2(startEndPoint.position.x - 158.235f, waterLevel);
        lastRiverbedEndPosition = (Vector2)startEndPoint.position - new Vector2(41f, 45.7f);
        lastCattailEndPosition = Vector3.zero;

        int startingSpawnMud = 1;
        for (int i = 0; i < startingSpawnMud; i++)
        {
            SpawnRiverbeds();
        }
        int startingSpawnWater = 1;
        for (int i = 0; i < startingSpawnWater; i++)
        {
            SpawnWater();
        }
    }
    private void FixedUpdate()
    {
        //Spawns objects when the player reaches a specified distance away from the last one
        if (Vector2.Distance(player.transform.position, lastRiverbedEndPosition) < riverbedSpawnDistance)
        {
            SpawnRiverbeds();
        }
        if (Vector2.Distance(player.transform.position, lastWaterEndPosition) < waterSpawnDistance)
        {
            SpawnWater();
        }
        if (Vector2.Distance(player.transform.position, lastLevelEndPosition) < levelPartDistance)
        {
            SpawnLevelPart();
        }
        if (Vector2.Distance(player.transform.position, lastPreyEndPosition) < preySpawnDistance)
        {
            SpawnFlies();
        }
        if (Vector2.Distance(player.transform.position, lastCattailEndPosition) < cattailSpawnDistance)
        {
            SpawnCattail();
        }
    }
    private void SpawnCattail()
    {
        Transform lastCattailTransform;

        int minCattailXOffset = 150, maxCattailXOffset = 250;
        int cattailXOffset = Random.Range(minCattailXOffset, maxCattailXOffset);
        int minCattailHeight = 10, maxCattailHeight = 18;
        int cattailHeight = Random.Range(minCattailHeight, maxCattailHeight);


        lastCattailTransform = SpawnCattail(cattailPrefab, new Vector3(lastCattailEndPosition.x + cattailXOffset, cattailHeight));
        lastCattailEndPosition = lastCattailTransform.position;
    }
    private Transform SpawnCattail(Transform cattail, Vector3 cattailSpawnPosition)
    {
        //There is a 50% chance for the cattail to actually spawn
        int chance = Random.Range(0, 100);
        if (chance > 50)
        {
            lastCattailTransform = Instantiate(cattail, cattailSpawnPosition, Quaternion.identity);
        }
        else
        {
            lastCattailTransform = Instantiate(emptyTransformPrefab, cattailSpawnPosition, Quaternion.identity);
        }

        return lastCattailTransform;
    }
    private void SpawnRiverbeds()
    {
        Transform lastMudTransform;

        Vector2 mudOffset = new Vector2(82f, 0);
        int chosen = Random.Range(0, riverbedPrefabs.Length);

        while (chosen == currentRiverbedIndex) //Repeat choosing which riverbed to spawn until a new one is chosen
        {
            chosen = Random.Range(0, riverbedPrefabs.Length);
        }

        lastMudTransform = SpawnRiverbed(riverbedPrefabs[chosen], (Vector2)lastRiverbedEndPosition + mudOffset);
        lastRiverbedEndPosition = lastMudTransform.position;

        currentRiverbedIndex = chosen;
    }
    private Transform SpawnRiverbed(Transform mud, Vector3 mudSpawnPosition)
    {
        lastRiverbedTransform = Instantiate(mud, mudSpawnPosition, Quaternion.identity);
        return lastRiverbedTransform;
    }
    private void SpawnWater()
    {
        Transform lastWaterTransform;

        Vector2 waterOffset = new (232.5f,0);

        lastWaterTransform = SpawnWater(waterPrefab, new Vector2(lastWaterEndPosition.x, waterLevel - 18.05f) + waterOffset);
        lastWaterEndPosition = lastWaterTransform.position;
    }
    private Transform SpawnWater(Transform water, Vector3 waterSpawnPosition)
    {
        lastWaterTransform = Instantiate(water, waterSpawnPosition, Quaternion.identity);
        return lastWaterTransform;
    }
    private void SpawnFlies()
    {
        Transform chosenPrey;
        Transform lastPreyTransform;

        float minFlyXOffset = 40, maxFlyXOffset = 60, minFlyYOffset = 11, maxFlyYOffset = 17;
        float flyXOffset = Random.Range(minFlyXOffset, maxFlyXOffset), flyYOffset = Random.Range(minFlyYOffset, maxFlyYOffset);
        Vector2 flyOffset = new Vector2(flyXOffset, flyYOffset);

        chosenPrey = preyList[0];

        lastPreyTransform = SpawnFlies(chosenPrey, new Vector2(lastPreyEndPosition.x, waterLevel) + flyOffset);

        lastPreyEndPosition = lastPreyTransform.position;
    }
    private Transform SpawnFlies(Transform prey, Vector3 preySpawnPosition)
    {
        //Chooses a random swarm size
        int swarmSizeGenerator = Random.Range(0, 101);
        if (swarmSizeGenerator >= 95)
            swarmSize = 5;
        else if (swarmSizeGenerator >= 85)
            swarmSize = 4;
        else if (swarmSizeGenerator >= 65)
            swarmSize = 3;
        else if (swarmSizeGenerator >= 36)
            swarmSize = 2;
        else
            swarmSize = 1;

        //spawns flies equal to the swarm size
        for (int i = 0; i < swarmSize; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            lastPreyTransform = Instantiate(prey, preySpawnPosition + offset, Quaternion.identity);
        }

        int dragonflySpawnChance = Random.Range(1, 3);
        if (dragonflySpawnChance == 1)
            SpawnDragonfly(preySpawnPosition);

        return lastPreyTransform;
    }
    void SpawnDragonfly(Vector3 position)
    {
        float minDragonflyXOffset = -30, maxDragonflyXOffset = 30, minHighDragonflyYOffset = 10, maxHighDragonflyYOffset = 14, minLowDragonflyYOffset = -2, maxLowDragonflyYOffset = 5;
        float flyXOffset = Random.Range(minDragonflyXOffset, maxDragonflyXOffset);
        float flyHighYOffset = Random.Range(minHighDragonflyYOffset, maxHighDragonflyYOffset);
        float flyLowYOffset = Random.Range(minLowDragonflyYOffset, maxLowDragonflyYOffset);

        int highChance = Random.Range(1, 5);
        Vector3 offset;

        if (highChance != 1) //80% chance to spawn above flies

        {
            offset = new Vector3(flyXOffset, flyHighYOffset);
        }
        else //20% chance to spawn with flies
        {
            offset = new Vector3(flyXOffset, flyLowYOffset);
        }

        Instantiate(dragonflyPrefab, position + offset, Quaternion.identity);
    }

    private void SpawnLevelPart()
    {
        Transform chosenLevelPart;
        Transform lastLevelPartTransform;
        Vector2 offset;

        //Range of the possible distances between a lilypad and the last spawned level part
        int minLilypadXOffset = 15, maxLilypadXOffset = 30, minLilypadYOffset = 0, maxLilypadYOffset = 2;
        int lilypadXOffset = Random.Range(minLilypadXOffset, maxLilypadXOffset), lilypadYOffset = Random.Range(minLilypadYOffset, maxLilypadYOffset);
        Vector2 lilypadOffset = new Vector2(lilypadXOffset,lilypadYOffset);

        //Range of the possible distances between a log and the last spawned level part
        int minLogXOffset = 25, maxLogXOffset = 35, minLogYOffset = -2, maxLogYOffset = 0;
        int logXOffset = Random.Range(minLogXOffset, maxLogXOffset), logYOffset = Random.Range(minLogYOffset, maxLogYOffset);
        Vector2 logOffset = new Vector2(logXOffset, logYOffset);

        levelPartCalc = Random.Range(0, 101);

        if (levelPartCalc <= 75)
        {
            //75% chance to spawn a lilypad
            chosenLevelPart = levelPartList[0];
            offset = lilypadOffset;
        }
        else
        {
            //25% chance to spawn a log
            chosenLevelPart = levelPartList[1];
            offset = logOffset;
        }

        lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, new Vector2(lastLevelEndPosition.x, waterLevel) + offset);

        lastLevelEndPosition = lastLevelPartTransform.Find("EndPosition").position;
    }

    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPosition)
    {
        //Spawns a level part and then returns that parts transform
        Transform levelpartTransform = Instantiate(levelPart, spawnPosition, Quaternion.identity);
        if (levelPartCalc >= 85) //60% chance to spawn one slug on a log
        {
            int doubleChance = Random.Range(1, 5);
            Instantiate(slugPrefab, spawnPosition + new Vector3(0, Random.Range(2,6), 0), Quaternion.identity);
            if (doubleChance == 1) //25% chance to spawn another after the first one is spawned
                Instantiate(slugPrefab, spawnPosition + new Vector3(0, -Random.Range(2, 6), 0), Quaternion.identity);
        }

        int striderChance = Random.Range(1, 8);
        if (levelPartCalc <= 75 && striderChance >= 7)
        {
            //Chooses a random swarm size
            int swarmSizeGenerator = Random.Range(0, 101);
            if (swarmSizeGenerator >= 50)
                swarmSize = 2;
            else
                swarmSize = 1;

            //spawns water striders equal to the swarm size
            for (int i = 0; i < swarmSize; i++)
            {
                Vector3 offset = new Vector3(Random.Range(4, 9), Random.Range(1, 3), 0);
                Instantiate(waterStriderPrefab, spawnPosition + offset, Quaternion.identity);
            }
        }

        return levelpartTransform;
    }
}