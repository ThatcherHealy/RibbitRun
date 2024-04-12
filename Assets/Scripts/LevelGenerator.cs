using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> bogLevelPartList;    
    [SerializeField] private List<Transform> AmazonLevelPartList;
    [SerializeField] private List<Transform> preyList;
    [SerializeField] private Transform[] bogRiverbedPrefabs;    
    [SerializeField] private Transform[] cypressRiverbedPrefabs;   
    [SerializeField] private Transform[] amazonRiverbedPrefabs;
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
    public Vector3 playerRefEndPoint;

    private Vector3 lastLevelEndPosition;
    private Vector3 lastPreyEndPosition;
    private Vector3 lastRiverbedEndPosition;
    private Vector3 lastCattailEndPosition;

    private int swarmSize;
    private int currentRiverbedIndex;
    private int levelPartCalc;
    Transform lastPreyTransform;
    Transform lastRiverbedTransform;
    Transform lastCattailTransform;
    Transform lastBiomeTransitionTransform;
    float waterLevel = -3.44f;

    bool switchPoints;
    public bool spawnTransitionRamp;
    bool biomeSwapSpawned;

    const float bogOffset = 0, cypressOffset = 9.3f, amazonOffset = -4.5f;
    float currentOffset;
    public enum Biome {Bog,Cypress,Amazon};
    public Biome biomeSpawning;
    public Biome playerBiome;
    [SerializeField] Biome startBiome;

    bool treeSpawned;

    private void Awake()
    {
        InitializeBiome();
        
        endPoint = startEndPoint.position;
        playerRefEndPoint = startEndPoint.position;
        biomeSpawning = startBiome;
        playerBiome = startBiome;

        SpawnStartPoints();
    }
    private void Start()
    {
        SpawnTransition();

        //Adjust the camera view to match the biome
        if (biomeSpawning == Biome.Bog)
        {
            cameraScript.lowerBound = 26;
        }
        else if (biomeSpawning == Biome.Cypress)
        {
            cameraScript.lowerBound = 17;
            cameraScript.mudLevel += cypressOffset;
        }
        else
        {
            cameraScript.lowerBound = 31f;
            cameraScript.mudLevel += amazonOffset;
        }
    }
    private void Update()
    {
        if (pc.changeBiome)
        {
            //Change the biome when the player hits a biomeswapper
            ChangeBiome();
            spawnTransitionRamp = true;
            pc.changeBiome = false;
        }

        if (pc.transitionCamera)
        {
            //Set the player reference endpoint to the current endpoint since the player has moved to the next biome
            playerRefEndPoint = endPoint;

            //Change the camera view when the player hits a cameratransition
            cameraScript.baseHeight -= 68;
            if (biomeSpawning == Biome.Bog)
            {
                cameraScript.lowerBound = 26;
                cameraScript.mudLevel = cameraScript.mudLevel - 68 - currentOffset + bogOffset;
            }
            else if (biomeSpawning == Biome.Cypress)
            {
                cameraScript.lowerBound = 17;
                cameraScript.mudLevel = cameraScript.mudLevel - 68 - currentOffset + cypressOffset;
            }
            else
            {
                cameraScript.lowerBound = 31f;
                cameraScript.mudLevel = cameraScript.mudLevel - 68 - currentOffset + amazonOffset;
            }

            pc.transitionCamera = false;
        }

        if(switchPoints)
        {
            switchPoints = false;
            waterLevel -= 68;
            SpawnStartPoints();
        }

        //Display the biome which the player is in, not the one that is being generated
        SetPlayerBiome();
    }
    private void SetPlayerBiome()
    {
        if (pc.biomeIn.Equals("Bog"))
        {
            playerBiome = Biome.Bog;
        }
        if (pc.biomeIn.Equals("Cypress"))
        {
            playerBiome = Biome.Cypress;
        }
        if (pc.biomeIn.Equals("Amazon"))
        {
            playerBiome = Biome.Amazon;
        }
    }
    private void ChangeBiome() 
    {
        int chance = UnityEngine.Random.Range(1, 3);
        if (biomeSpawning == Biome.Bog)
        {
            currentOffset = bogOffset;

            if (chance == 1)
                biomeSpawning = Biome.Cypress;
            else
                biomeSpawning = Biome.Amazon;
        }
        else if (biomeSpawning == Biome.Cypress)
        {
            currentOffset = cypressOffset;

            if (chance == 1)
                biomeSpawning = Biome.Bog;
            else
                biomeSpawning = Biome.Amazon;
        }
        else if (biomeSpawning == Biome.Amazon)
        {
            currentOffset = amazonOffset;

            if (chance == 1)
                biomeSpawning = Biome.Bog;
            else
                biomeSpawning = Biome.Cypress;
        }

        treeSpawned = false;
    }
    private void SpawnStartPoints() 
    {
        //Spawns the starting level
        lastLevelEndPosition = endPoint;
        lastPreyEndPosition = endPoint;
        lastRiverbedEndPosition = endPoint - new Vector3(41f, 45.7f, 0);
        lastCattailEndPosition = endPoint - new Vector3(40.34617f, -0.645f,0);
    }
    private void FixedUpdate()
    {
        //Spawns objects when the player reaches a specified distance away from the last one
        if(!pc.dead)
        {
            if (math.distance(player.transform.position.x, lastRiverbedEndPosition.x) < spawnDistance)
            {
                SpawnRiverbeds();
            }
            if (math.distance(player.transform.position.x, lastLevelEndPosition.x) < spawnDistance)
            {
                if (playerBiome != Biome.Cypress)
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
            if (biomeSpawning != playerBiome && !biomeSwapSpawned)
            {
                SpawnTransition();
            }
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
            lastCattailTransform = Instantiate(emptyTransformPrefab, cattailSpawnPosition, Quaternion.identity);
        }
        else
        {
            lastCattailTransform = Instantiate(cattail, cattailSpawnPosition, Quaternion.identity);
        }

        return lastCattailTransform;
    }
    private void SpawnTransition()
    {
        Transform lastTransitionTransform;
        int minTransitionXOffset, maxTransitionXOffset;

        //Spawns the next biome transition at a random distance between values
        //Set min and max to determine biome length
        if (biomeSpawning == Biome.Bog)
        {
            minTransitionXOffset = 400; 
            maxTransitionXOffset = 600;
        }
        else if (biomeSpawning == Biome.Cypress)
        {
            minTransitionXOffset = 200;
            maxTransitionXOffset = 400;
        }
        else
        {
            minTransitionXOffset = 300;
            maxTransitionXOffset = 600;
        }


        int transitionXOffset = UnityEngine.Random.Range(minTransitionXOffset, maxTransitionXOffset);


        lastTransitionTransform = SpawnTransition(biomeSwapperPrefab, new Vector3(lastRiverbedEndPosition.x + transitionXOffset, endPoint.y + 10));
        biomeSwapSpawned = true;
    }
    private Transform SpawnTransition(Transform transition, Vector3 transitionSpawnPosition)
    {
        lastBiomeTransitionTransform = Instantiate(transition, transitionSpawnPosition, Quaternion.identity);
        return lastBiomeTransitionTransform;
    }
    private void SpawnRiverbeds()
    {
        Transform lastRiverbedTransform;

        Vector2 mudOffset = new Vector2(82f, 0);

        if (spawnTransitionRamp) //Spawn a biome transition
        {
            lastRiverbedTransform = SpawnRiverbed(transitionRamps[0], new Vector3(lastRiverbedEndPosition.x + mudOffset.x + 36, endPoint.y - 67, 0));
            endPoint = lastRiverbedTransform.GetChild(0).position;
            spawnTransitionRamp = false;
            switchPoints = true;
            biomeSwapSpawned = false;
        }
        else //Spawn a normal riverbed
        {
            if (biomeSpawning == Biome.Bog)
            {
                int chosen = UnityEngine.Random.Range(0, bogRiverbedPrefabs.Length);
                while (chosen == currentRiverbedIndex) //Repeat choosing which riverbed to spawn until a new one is chosen
                {
                    chosen = UnityEngine.Random.Range(0, bogRiverbedPrefabs.Length);
                }

                SpawnRiverbed(bogRiverbedPrefabs[chosen], (Vector2)lastRiverbedEndPosition + mudOffset);
                currentRiverbedIndex = chosen;
            }
            if (biomeSpawning == Biome.Cypress)
            {
                int chosen;

                if (treeSpawned)
                    chosen = UnityEngine.Random.Range(0, cypressRiverbedPrefabs.Length - 1);
                else
                {
                    chosen = UnityEngine.Random.Range(0, cypressRiverbedPrefabs.Length );
                }

                while (chosen == currentRiverbedIndex) //Repeat choosing which riverbed to spawn until a new one is chosen
                {
                    chosen = UnityEngine.Random.Range(0, cypressRiverbedPrefabs.Length);
                }

                if (cypressRiverbedPrefabs[chosen] == cypressRiverbedPrefabs[5])
                {
                    treeSpawned = true;
                }

                SpawnRiverbed(cypressRiverbedPrefabs[chosen], (Vector2)lastRiverbedEndPosition + mudOffset);
                currentRiverbedIndex = chosen;
            }
            if (biomeSpawning == Biome.Amazon)
            {
                int chosen = UnityEngine.Random.Range(0, amazonRiverbedPrefabs.Length);
                while (chosen == currentRiverbedIndex) //Repeat choosing which riverbed to spawn until a new one is chosen
                {
                    chosen = UnityEngine.Random.Range(0, amazonRiverbedPrefabs.Length);
                }

                SpawnRiverbed(amazonRiverbedPrefabs[chosen], (Vector2)lastRiverbedEndPosition + mudOffset);
                currentRiverbedIndex = chosen;
            }
        }
    }
    private Transform SpawnRiverbed(Transform riverbed, Vector3 riverbedSpawnPosition)
    {
        if (lastRiverbedTransform != null)
            if (lastRiverbedTransform.GetComponent<BoxCollider2D>() != null)
                lastRiverbedTransform.GetComponent<BoxCollider2D>().enabled = false;

        lastRiverbedTransform = Instantiate(riverbed, riverbedSpawnPosition, Quaternion.identity);
        lastRiverbedEndPosition = riverbedSpawnPosition;
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

        //Range of the possible distances between a water lily and the last spawned level part
        int minWaterLilyXOffset = 20, maxWaterLilyXOffset = 35, minWaterLilyYOffset = 0, maxWaterLilyYOffset = 2;
        int waterLilyXOffset = UnityEngine.Random.Range(minWaterLilyXOffset, maxWaterLilyXOffset), waterLilyYOffset = UnityEngine.Random.Range(minWaterLilyYOffset, maxWaterLilyYOffset);
        Vector2 waterLilyOffset = new Vector2(waterLilyXOffset, waterLilyYOffset);

        //Range of the possible distances between a log and the last spawned level part
        int minLogXOffset = 25, maxLogXOffset = 35, minLogYOffset = -2, maxLogYOffset = 0;
        int logXOffset = UnityEngine.Random.Range(minLogXOffset, maxLogXOffset), logYOffset = UnityEngine.Random.Range(minLogYOffset, maxLogYOffset);
        Vector2 logOffset = new Vector2(logXOffset, logYOffset);

        //Range of the possible distances between an amazon log and the last spawned level part
        int minAmazonLogXOffset = 30, maxAmazonLogXOffset = 45, minAmazonLogYOffset = -2, maxAmazonLogYOffset = 0;
        int amazonLogXOffset = UnityEngine.Random.Range(minAmazonLogXOffset, maxAmazonLogXOffset), amazonLogYOffset = UnityEngine.Random.Range(minAmazonLogYOffset, maxAmazonLogYOffset);
        Vector2 amazonLogOffset = new Vector2(amazonLogXOffset, amazonLogYOffset);

        levelPartCalc = UnityEngine.Random.Range(0, 101);
        if (playerBiome == Biome.Bog)
        {
            if (levelPartCalc <= 75)
            {
                //75% chance to spawn a lilypad
                float lilypadVariant = UnityEngine.Random.Range(0, 100);
                if (lilypadVariant <= 90)
                {
                    chosenLevelPart = bogLevelPartList[0]; //90% chance to spawn normal lilypad
                }
                else
                {
                    //10% chance to flower
                    float color = UnityEngine.Random.Range(0, 100);
                    if (color <= 33.3f)
                        chosenLevelPart = bogLevelPartList[2]; //33% chance to spawn pink
                    else if (color > 33.3f && color <= 66.6f)
                        chosenLevelPart = bogLevelPartList[3]; //33% chace to spawn red
                    else
                        chosenLevelPart = bogLevelPartList[4]; //33% chance to spawn white
                }

                offset = lilypadOffset;
            }
            else
            {
                //25% chance to spawn a log
                chosenLevelPart = bogLevelPartList[1];
                offset = logOffset;
            }
        }
        else //Amazon
        {
            if (levelPartCalc <= 85)
            {
                //85% chance to spawn a water lily
                float waterLilyVariant = UnityEngine.Random.Range(0, 100);
                if (waterLilyVariant <= 50)
                {
                    chosenLevelPart = AmazonLevelPartList[0]; //50% chance to spawn normal size waterlily
                }
                else if (waterLilyVariant <= 75)
                {
                    chosenLevelPart = AmazonLevelPartList[1]; //25% chance to spawn large size waterlily
                }
                else
                {
                    chosenLevelPart = AmazonLevelPartList[2]; //50% chance to spawn small size waterlily
                }
                offset = waterLilyOffset;
            }
            else
            {
                //15% chance to spawn a log
                chosenLevelPart = AmazonLevelPartList[3];
                offset = amazonLogOffset;
            }
        }
            lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, new Vector2(lastLevelEndPosition.x, waterLevel) + offset);
            lastLevelEndPosition = lastLevelPartTransform.Find("EndPosition").position;
    }

    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPosition)
    {
        //Spawns a level part and then returns that parts transform
        Transform levelpartTransform = Instantiate(levelPart, spawnPosition, Quaternion.identity);
        if (levelPartCalc >= 85 && biomeSpawning == Biome.Bog) //60% chance to spawn one slug on a log
        {
            int doubleChance = UnityEngine.Random.Range(1, 5);
            Instantiate(slugPrefab, spawnPosition + new Vector3(0, UnityEngine.Random.Range(2,3), 0), Quaternion.identity);
            if (doubleChance == 1) //25% chance to spawn another after the first one is spawned
                Instantiate(slugPrefab, spawnPosition + new Vector3(0, -UnityEngine.Random.Range(2, 3), 0), Quaternion.identity);
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
    void InitializeBiome() 
    {
        //Start you in the biome you died in last
        if (PlayerPrefs.GetString("StartBiome") == "Bog")
        {
            startBiome = Biome.Bog;
        }
        else if (PlayerPrefs.GetString("StartBiome") == "Cypress")
        {
            startBiome = Biome.Cypress;
        }
        else if (PlayerPrefs.GetString("StartBiome") == "Amazon")
        {
            startBiome = Biome.Amazon;
        }
        else
            startBiome = Biome.Bog;
    }
}