using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> levelPartList;
    [SerializeField] private List<Transform> preyList;
    [SerializeField] private Transform[] riverbedPrefabs;
    [SerializeField] private Transform[] transitionRamps;

    [SerializeField] private Transform startEndPoint;
    [SerializeField] private Transform lilypad1;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController pc;
    [SerializeField] private CameraFollow cameraScript;
    [SerializeField] private Transform cattailPrefab;
    [SerializeField] private Transform emptyTransformPrefab;
    [SerializeField] private Transform slugPrefab;
    [SerializeField] private Transform waterStriderPrefab;
    [SerializeField] private Transform dragonflyPrefab;
    [SerializeField] private Transform biomeSwapperPrefab;

    private const float spawnDistance = 100;

    public Vector3 endPoint;

    private Vector3 lastLevelEndPosition;
    private Vector3 lastPreyEndPosition;
    private Vector3 lastRiverbedEndPosition;
    private Vector3 lastCattailEndPosition;
    private Vector3 lastBiomeTransitionEndPosition;

    private int swarmSize;
    private int currentRiverbedIndex;
    private int levelPartCalc;
    Transform lastPreyTransform;
    Transform lastRiverbedTransform;
    Transform lastCattailTransform;
    Transform lastBiomeTransitionTransform;
    float waterLevel = -3.44f;
    bool changeBiome;
    bool switchPoints;
    public enum Biome {Bog,Cypress,Amazon};
    Biome currentBiome;

    private void Awake()
    {
        endPoint = startEndPoint.position;
        currentBiome = Biome.Bog;

        SpawnStartPoints();

        int startingSpawnMud = 1;
        for (int i = 0; i < startingSpawnMud; i++)
        {
            SpawnRiverbeds();
        }
    }
    private void Update()
    {
        if (pc.changeBiome)
        {
            //Change the biome when the player hits a biomeswapper
            pc.changeBiome = false;
            ChangeBiome();
        }

        if (pc.transitionCamera)
        {
            //Change the camera view when the player hits a cameratransition
            cameraScript.baseHeight -= 68;
            cameraScript.mudLevel -= 68;
            pc.transitionCamera = false;
        }

        if(switchPoints)
        {
            switchPoints = false;
            waterLevel -= 68;
            SpawnStartPoints();
        }
    }
    private void ChangeBiome() 
    {
        int chance = UnityEngine.Random.Range(1, 3);
        if (currentBiome == Biome.Bog)
        {
            if (chance == 1)
                currentBiome = Biome.Cypress;
            else
                currentBiome = Biome.Amazon;
        }
        else if (currentBiome == Biome.Cypress)
        {
            if (chance == 1)
                currentBiome = Biome.Bog;
            else
                currentBiome = Biome.Amazon;
        }
        else if (currentBiome == Biome.Amazon)
        {
            if (chance == 1)
                currentBiome = Biome.Bog;
            else
                currentBiome = Biome.Cypress;
        }
        changeBiome = true;
    }
    private void SpawnStartPoints() 
    {
        //Spawns the starting level
        lastLevelEndPosition = endPoint;
        lastPreyEndPosition = endPoint;
        lastRiverbedEndPosition = endPoint - new Vector3(41f, 45.7f, 0);
        lastCattailEndPosition = endPoint - new Vector3(40.34617f, -0.645f,0);
        lastBiomeTransitionEndPosition = endPoint;
    }
    private void FixedUpdate()
    {
        //Spawns objects when the player reaches a specified distance away from the last one
        if (math.distance(player.transform.position.x, lastRiverbedEndPosition.x) < spawnDistance)
        {
            SpawnRiverbeds();
        }
        if (math.distance(player.transform.position.x, lastLevelEndPosition.x) < spawnDistance)
        {
            SpawnLevelPart();
        }
        if (math.distance(player.transform.position.x, lastPreyEndPosition.x) < spawnDistance)
        {
            SpawnFlies();
        }
        if (math.distance(player.transform.position.x, lastCattailEndPosition.x) < spawnDistance)
        {
            SpawnCattail();
        }
        if (math.distance(player.transform.position.x, lastBiomeTransitionEndPosition.x) < spawnDistance)
        {
            SpawnTransition();
        }
    }
    private void SpawnCattail()
    {
        Transform lastCattailTransform;

        int minCattailXOffset = 150, maxCattailXOffset = 250;
        int cattailXOffset = UnityEngine.Random.Range(minCattailXOffset, maxCattailXOffset);
        float minCattailHeight = endPoint.y + 14.485f, maxCattailHeight = endPoint.y + 22.485f;
        float cattailHeight = UnityEngine.Random.Range(minCattailHeight, maxCattailHeight);


        lastCattailTransform = SpawnCattail(cattailPrefab, new Vector3(lastCattailEndPosition.x + cattailXOffset, cattailHeight));
        lastCattailEndPosition = lastCattailTransform.position;
    }
    private Transform SpawnCattail(Transform cattail, Vector3 cattailSpawnPosition)
    {
        //There is a 50% chance for the cattail to actually spawn
        int chance = UnityEngine.Random.Range(0, 100);
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
    private void SpawnTransition()
    {
        Transform lastTransitionTransform;

        int minTransitionXOffset = 250, maxTransitionXOffset = 300;
        int transitionXOffset = UnityEngine.Random.Range(minTransitionXOffset, maxTransitionXOffset);


        lastTransitionTransform = SpawnTransition(biomeSwapperPrefab, new Vector3(lastBiomeTransitionEndPosition.x + transitionXOffset, endPoint.y + 10));
        lastBiomeTransitionEndPosition = lastTransitionTransform.position;
    }
    private Transform SpawnTransition(Transform transition, Vector3 transitionSpawnPosition)
    {
        //There is a 50% chance for the transition to actually spawn
        int chance = UnityEngine.Random.Range(0, 100);
        if (chance >= 50)
        {
            lastBiomeTransitionTransform = Instantiate(transition, transitionSpawnPosition, Quaternion.identity);
        }
        else
        {
            lastBiomeTransitionTransform = Instantiate(emptyTransformPrefab, transitionSpawnPosition, Quaternion.identity);
        }

        return lastBiomeTransitionTransform;
    }
    private void SpawnRiverbeds()
    {
        Transform lastMudTransform;

        Vector2 mudOffset = new Vector2(82f, 0);
        int chosen = UnityEngine.Random.Range(0, riverbedPrefabs.Length);

        while (chosen == currentRiverbedIndex) //Repeat choosing which riverbed to spawn until a new one is chosen
        {
            chosen = UnityEngine.Random.Range(0, riverbedPrefabs.Length);
        }

        if (!changeBiome) //Spawn a normal riverbed
        {
            lastMudTransform = SpawnRiverbed(riverbedPrefabs[chosen], (Vector2)lastRiverbedEndPosition + mudOffset);
            lastRiverbedEndPosition = lastMudTransform.position;
        }

        else //Spawn a biome transition
        {
            lastMudTransform = Instantiate(transitionRamps[0], new Vector3(lastRiverbedEndPosition.x + mudOffset.x + 36, endPoint.y - 67, 0), Quaternion.identity);
            endPoint = lastMudTransform.GetChild(0).position;
            changeBiome = false;
            switchPoints = true;
        }
        currentRiverbedIndex = chosen;
    }
    private Transform SpawnRiverbed(Transform mud, Vector3 mudSpawnPosition)
    {
        lastRiverbedTransform = Instantiate(mud, mudSpawnPosition, Quaternion.identity);
        return lastRiverbedTransform;
    }
    private void SpawnFlies()
    {
        Transform chosenPrey;
        Transform lastPreyTransform;

        float minFlyXOffset = 40, maxFlyXOffset = 60, minFlyYOffset = 11, maxFlyYOffset = 17;
        float flyXOffset = UnityEngine.Random.Range(minFlyXOffset, maxFlyXOffset), flyYOffset = UnityEngine.Random.Range(minFlyYOffset, maxFlyYOffset);
        Vector2 flyOffset = new Vector2(flyXOffset, flyYOffset);

        chosenPrey = preyList[0];

        lastPreyTransform = SpawnFlies(chosenPrey, new Vector2(lastPreyEndPosition.x, waterLevel) + flyOffset);

        lastPreyEndPosition = lastPreyTransform.position;
    }
    private Transform SpawnFlies(Transform prey, Vector3 preySpawnPosition)
    {
        //Chooses a random swarm size
        int swarmSizeGenerator = UnityEngine.Random.Range(0, 101);
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
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), 0);
            lastPreyTransform = Instantiate(prey, preySpawnPosition + offset, Quaternion.identity);
        }

        int dragonflySpawnChance = UnityEngine.Random.Range(1, 3);
        if (dragonflySpawnChance == 1)
            SpawnDragonfly(preySpawnPosition);

        return lastPreyTransform;
    }
    void SpawnDragonfly(Vector3 position)
    {
        float minDragonflyXOffset = -30, maxDragonflyXOffset = 30, minHighDragonflyYOffset = 10, maxHighDragonflyYOffset = 14, minLowDragonflyYOffset = -2, maxLowDragonflyYOffset = 5;
        float flyXOffset = UnityEngine.Random.Range(minDragonflyXOffset, maxDragonflyXOffset);
        float flyHighYOffset = UnityEngine.Random.Range(minHighDragonflyYOffset, maxHighDragonflyYOffset);
        float flyLowYOffset = UnityEngine.Random.Range(minLowDragonflyYOffset, maxLowDragonflyYOffset);

        int highChance = UnityEngine.Random.Range(1, 5);
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
        int lilypadXOffset = UnityEngine.Random.Range(minLilypadXOffset, maxLilypadXOffset), lilypadYOffset = UnityEngine.Random.Range(minLilypadYOffset, maxLilypadYOffset);
        Vector2 lilypadOffset = new Vector2(lilypadXOffset,lilypadYOffset);

        //Range of the possible distances between a log and the last spawned level part
        int minLogXOffset = 25, maxLogXOffset = 35, minLogYOffset = -2, maxLogYOffset = 0;
        int logXOffset = UnityEngine.Random.Range(minLogXOffset, maxLogXOffset), logYOffset = UnityEngine.Random.Range(minLogYOffset, maxLogYOffset);
        Vector2 logOffset = new Vector2(logXOffset, logYOffset);

        levelPartCalc = UnityEngine.Random.Range(0, 101);

        if (levelPartCalc <= 75)
        {
            //75% chance to spawn a lilypad
            float lilypadVariant = UnityEngine.Random.Range(0, 100);
            if(lilypadVariant <= 90)
            {
                chosenLevelPart = levelPartList[0]; //90% chance to spawn normal lilypad
            }
            else
            {
                //10% chance to flower
                float color = UnityEngine.Random.Range(0, 100);
                if (color <= 33.3f)
                    chosenLevelPart = levelPartList[2]; //33% chance to spawn pink
                else if (color > 33.3f && color <= 66.6f)
                    chosenLevelPart = levelPartList[3]; //33% chace to spawn red
                else
                    chosenLevelPart = levelPartList[4]; //33% chance to spawn white
            }

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
            int doubleChance = UnityEngine.Random.Range(1, 5);
            Instantiate(slugPrefab, spawnPosition + new Vector3(0, UnityEngine.Random.Range(2,6), 0), Quaternion.identity);
            if (doubleChance == 1) //25% chance to spawn another after the first one is spawned
                Instantiate(slugPrefab, spawnPosition + new Vector3(0, -UnityEngine.Random.Range(2, 6), 0), Quaternion.identity);
        }

        int striderChance = UnityEngine.Random.Range(1, 8);
        if (levelPartCalc <= 75 && striderChance >= 7)
        {
            //Chooses a random swarm size
            int swarmSizeGenerator = UnityEngine.Random.Range(0, 101);
            if (swarmSizeGenerator >= 50)
                swarmSize = 2;
            else
                swarmSize = 1;

            //spawns water striders equal to the swarm size
            for (int i = 0; i < swarmSize; i++)
            {
                Vector3 offset = new Vector3(UnityEngine.Random.Range(4, 9), UnityEngine.Random.Range(1, 3), 0);
                Instantiate(waterStriderPrefab, spawnPosition + offset, Quaternion.identity);
            }
        }

        return levelpartTransform;
    }
}