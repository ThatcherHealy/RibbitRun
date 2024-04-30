using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Predator
{
    public enum PredatorType { FishSwarm, Arapaima, Heron, Falcon }
    private PredatorType type;
    private GameObject linkedWarning;
    private GameObject prefab;
    private GameObject activeObject;
    private int direction;
    private bool spawned;

    public Predator(PredatorType setType, GameObject setPrefab)
    {
        type = setType;
        prefab = setPrefab;
    }
    public PredatorType Type()
    {
        return type;
    }
    public GameObject LinkedWarning() 
    {
        return linkedWarning;
    }
    public GameObject GetPredator()
    {
        return activeObject;
    }
    public void SetDirection(int directionChance)
    {
        direction = directionChance;
    }

    public void SetSpawned(bool spawn)
    {
        spawned = spawn;
    }
    public bool Spawned()
    {
        return spawned;
    }

    public Vector3 SetSpawnPoint(LevelGenerator lg, Transform player)
    {
        if(type == PredatorType.FishSwarm || type == PredatorType.Arapaima)
        {
            if (lg.biomeSpawning != lg.playerBiome)
                direction = 2;

            if (direction == 1)
                return new Vector2(player.position.x + 150, lg.playerRefEndPoint.y - 16.415f);
            else
                return new Vector2(player.position.x - 150, lg.playerRefEndPoint.y - 16.415f);
        }
        else if (type == PredatorType.Falcon)
        {
            return new Vector2(player.position.x, lg.playerRefEndPoint.y + 209.585f);
        }
        else //Heron
        {
            if (direction == 1)
                return new Vector2(player.position.x + 100, lg.playerRefEndPoint.y + 9.585f);
            else
                return new Vector2(player.position.x - 100, lg.playerRefEndPoint.y + 9.585f);
        }
    }
    public void SpawnPredator(Vector3 spawnPoint, Transform player, LevelGenerator lg)
    {
        if (lg.playerBiome == LevelGenerator.Biome.Bog && type == PredatorType.Arapaima)
            type = PredatorType.FishSwarm;
        else if (lg.playerBiome == LevelGenerator.Biome.Amazon && type == PredatorType.FishSwarm)
            type = PredatorType.Arapaima;

        if (type != PredatorType.FishSwarm)
            activeObject = GameObject.Instantiate(prefab, spawnPoint, Quaternion.identity);
        else
        {
            activeObject = GameObject.Instantiate(prefab, spawnPoint, Quaternion.identity);
            FishBehavior[] fishes = activeObject.GetComponentsInChildren<FishBehavior>();
            Transform closestFish = activeObject.GetComponentInChildren<FishBehavior>().transform;
            foreach (FishBehavior fish in fishes)
            {
                if (math.distance(fish.transform.position.x, player.position.x) < math.distance(closestFish.transform.position.x, player.position.x))
                {
                    closestFish = fish.transform;
                }
            }
            activeObject = closestFish.gameObject;
        }
    }
    public void SpawnWarning(GameObject warningPrefab, Vector3 spawnPoint)
    {
        linkedWarning = GameObject.Instantiate(warningPrefab, spawnPoint, Quaternion.identity);
    }

    public IEnumerator DestroyTimer(int time)
    {
        yield return new WaitForSeconds(time);
        GameObject.Destroy(activeObject);
    }
    public bool Destroyed()
    { 
        return activeObject == null;
    }
}
public class PredatorEvents : MonoBehaviour
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
    public int lowerScoreLimit = 50;
    private float checkInterval = 5;
    int max, startingValue = 4;
    private bool cooldown = false;
    bool falconEvent;

    private int warningTime = 3;
    private GameObject warning;
    [SerializeField] List<Predator> predators = new List<Predator>();
    private Rect cameraRect;
    private Rect shrunkCameraRect;

    void Start()
    {
        StartCoroutine(DetermineSpawnTime());
        max = startingValue;
    }

    IEnumerator DetermineSpawnTime()
    {
        //Decrease checkInterval by 0.2 every 200 points, capped out at 1 second
        float multiplier = Mathf.Round(sc.score / 200);
        for (int i = 0; i < multiplier; i++)
        {
            checkInterval -= 0.2f;
        }

        if (checkInterval < 1)
            checkInterval = 1;

        yield return new WaitForSeconds(checkInterval);


        if (sc.score > lowerScoreLimit && !cooldown && !pc.dead)
        {
            int chance = UnityEngine.Random.Range(1, max);
            if (chance == 1) //33% chance
            {
                max = startingValue;
                //Spawns a fish when you're in the water and a bird when you're out of the water
                //Never spawns a fish when you're in cypress
                if (pc.isSwimming && lg.playerBiome != LevelGenerator.Biome.Cypress)
                {
                    StartCoroutine(FishEvent());

                    float doubleChance = UnityEngine.Random.Range(1, 20000);
                    int sameChance = UnityEngine.Random.Range(1, 3);
                    if (sc.score > 1000 && doubleChance <= sc.score)
                    {
                        yield return new WaitForSeconds(1.5f);

                        if(sameChance == 1)
                            StartCoroutine(FishEvent());
                        else
                            StartCoroutine(BirdEvent());
                    }
                }
                else
                {

                    StartCoroutine(BirdEvent());

                    float doubleChance = UnityEngine.Random.Range(1, 20000);
                    int sameChance = UnityEngine.Random.Range(1, 3);
                    if (doubleChance <= sc.score)
                    {
                        yield return new WaitForSeconds(1.5f);

                        if (sameChance == 1 || lg.playerBiome == LevelGenerator.Biome.Cypress)
                            StartCoroutine(BirdEvent());
                        else
                            StartCoroutine(FishEvent());
                    }
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
        int fishCooldownTime = 20;
        StartCoroutine(Cooldown(fishCooldownTime));

        if (lg.playerBiome != LevelGenerator.Biome.Amazon)
        {
            Predator fishSwarm = new Predator(Predator.PredatorType.FishSwarm, fishSwarmPrefab);
            predators.Add(fishSwarm);
            fishSwarm.SetDirection(UnityEngine.Random.Range(1, 3));

            fishSwarm.SetSpawned(false);
            fishSwarm.SpawnWarning(warningPrefab, fishSwarm.SetSpawnPoint(lg, player));

            yield return new WaitForSeconds(warningTime);
            fishSwarm.SpawnPredator(fishSwarm.SetSpawnPoint(lg, player), player, lg);
            fishSwarm.SetSpawned(true);

            fishSwarm.DestroyTimer(15);
        }
        else
        {
            Predator arapaima = new Predator(Predator.PredatorType.Arapaima, arapaimaPrefab);
            predators.Add(arapaima);
            arapaima.SetDirection(UnityEngine.Random.Range(1, 3));

            arapaima.SetSpawned(false);
            arapaima.SpawnWarning(warningPrefab, arapaima.SetSpawnPoint(lg, player));

            yield return new WaitForSeconds(warningTime);
            arapaima.SpawnPredator(arapaima.SetSpawnPoint(lg, player), player, lg);
            arapaima.SetSpawned(true);

            arapaima.DestroyTimer(15);
        }
    }
    private IEnumerator BirdEvent()
    {

        int birdCooldownTime = 20;
        StartCoroutine(Cooldown(birdCooldownTime));

        int falconChance = UnityEngine.Random.Range(1, 4);
        if (falconChance == 1)
        {
            falconEvent = true;
        }

        if (falconEvent)
        {
            //Set falcon to false since it is only needed to determine what to spawn
            falconEvent = false;

            Predator falcon = new Predator(Predator.PredatorType.Falcon, falconPrefab);
            predators.Add(falcon);
            falcon.SetDirection(UnityEngine.Random.Range(1, 3));

            falcon.SetSpawned(false);
            falcon.SpawnWarning(warningPrefab, falcon.SetSpawnPoint(lg, player));

            yield return new WaitForSeconds(warningTime);
            falcon.SpawnPredator(falcon.SetSpawnPoint(lg, player), player,lg);
            falcon.SetSpawned(true);

            falcon.DestroyTimer(15);
        }
        else
        {
            Predator heron = new Predator(Predator.PredatorType.Heron, heronPrefab);
            predators.Add(heron);
            heron.SetDirection(UnityEngine.Random.Range(1, 3));

            heron.SetSpawned(false);
            heron.SpawnWarning(warningPrefab, heron.SetSpawnPoint(lg, player));

            yield return new WaitForSeconds(warningTime);
            heron.SpawnPredator(heron.SetSpawnPoint(lg, player), player, lg);
            heron.SetSpawned(true);

            heron.DestroyTimer(15);
        }


    }

    void FixedUpdate() 
    {
        List<Predator> predatorsCopy = new List<Predator>(predators);
        foreach (Predator predator in predatorsCopy)
        {
            // When the predator has been spawned but it is now destroyed, remove it from the list
            if (predator.Spawned() && predator.Destroyed())
            {
                predators.Remove(predator);
            }

            //Destroy predators in the wrong biome
            if((predator.Type() == Predator.PredatorType.Arapaima && lg.playerBiome != LevelGenerator.Biome.Amazon)
                || predator.Type() == Predator.PredatorType.FishSwarm && lg.playerBiome != LevelGenerator.Biome.Bog)
            {
                if(predator.Type() == Predator.PredatorType.FishSwarm)
                    Destroy(predator.GetPredator().transform.parent.gameObject);
                else
                    Destroy(predator.GetPredator());

                Destroy(predator.LinkedWarning());
                predators.Remove(predator);
            }
        }
        foreach (Predator predator in predators)
        {
            //Update Predator Spawn Position
            predator.SetSpawnPoint(lg, player);
        }

        foreach (Predator predator in predators)
        {
            SetWarningPosition(predator);
        }

        if(pc.drowned || pc.poisoned) //Deactivate warning and predator when player drowns
        {
            foreach (Predator predator in predators)
            {
                if(predator.LinkedWarning() != null)
                    predator.LinkedWarning().SetActive(false);
                if(predator.GetPredator() != null)
                    predator.GetPredator().SetActive(false);
            }
        }
    }
    void SetWarningPosition(Predator predator) 
    {
        if (predator.LinkedWarning() != null)
        {
            //Stop the warning when the predator is destroyed
            if(predator.Spawned() && predator.GetPredator() == null)
            {
                predator.SetSpawned(false);
                Destroy(predator.LinkedWarning());
                return;
            }
            //Stop the warning when the falcon stops diving
            if (predator.Type() == Predator.PredatorType.Falcon && predator.Spawned() && !predator.GetPredator().GetComponent<FalconBehavior>().diving)
            {
                predator.SetSpawned(false);
                Destroy(predator.LinkedWarning());
                return;
            }

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
            if (predator.Type() == Predator.PredatorType.Falcon)
                yShrink = 0.8f;

            float shrunkWidth = cameraRect.width * xShrink;
            float shrunkHeight = cameraRect.height * yShrink;
            shrunkCameraRect = new Rect(smoothedCenter.x - shrunkWidth / 2, smoothedCenter.y - shrunkHeight / 2, shrunkWidth, shrunkHeight);

            //Finally, set the warning position to be where the predator is going to spawn from (before it's spawned), and then to where the predator is. All clamped within the shrunk rectangle
            Vector3 warningTarget;
            if (predator.Spawned())
            {
                    warningTarget = new Vector3(
                    Mathf.Clamp(predator.GetPredator().transform.position.x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
                    Mathf.Clamp(predator.GetPredator().transform.position.y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
            }
            else
            {
              warningTarget = new Vector3(
              Mathf.Clamp(predator.SetSpawnPoint(lg, player).x, shrunkCameraRect.xMin, shrunkCameraRect.xMax),
              Mathf.Clamp(predator.SetSpawnPoint(lg, player).y, shrunkCameraRect.yMin, shrunkCameraRect.yMax), 0);
            }

            //Set Position
            predator.LinkedWarning().transform.position = Vector3.Lerp(predator.LinkedWarning().transform.position, warningTarget, 100 * Time.deltaTime);

            //Set Scale
            float scale; float distance;
            if (predator.Spawned())
            {
                distance = Mathf.Abs(Vector3.Distance(player.position, predator.GetPredator().transform.position));
            }
            else
            {
                distance = Mathf.Abs(Vector3.Distance(player.position, predator.SetSpawnPoint(lg, player)));
            }
            float normalizedDistance = Mathf.Clamp01(distance / 150f);
            if(predator.Type() == Predator.PredatorType.Falcon)
                normalizedDistance = Mathf.Clamp01(distance / 200f);

            // Use Mathf.Lerp to interpolate between 0.5 and 1 based on the normalized distance
            scale = Mathf.Lerp(0.5f, 1f, 1 - normalizedDistance);
            scale *= 1.5f;
            predator.LinkedWarning().transform.localScale = new Vector3(scale,scale,1);

            //When the predator enters the cameraview, stop the warning
            if ((predator.Type() != Predator.PredatorType.Falcon && (predator.Spawned() && predator.GetPredator().transform.position.x < (topRight.x + 5) && predator.GetPredator().transform.position.x > (bottomLeft.x - 5)))
                || (predator.Type() == Predator.PredatorType.Falcon && (predator.Spawned() && predator.GetPredator().transform.position.y < (topRight.y + 5))))
            {
                Destroy(predator.LinkedWarning());
            }
        }
    }
    IEnumerator Cooldown(float cooldownTime)
    {
        cooldown = true;
        checkInterval = 5;

        //Decrease cooldownTime by 0.5 every 175 points, capped out at 5 seconds at 3500
        float multiplier = Mathf.Round(sc.score / 175);
        for (int i = 0; i < multiplier; i++)
        {
            cooldownTime -= 0.5f;
        }

        if(cooldownTime < 5)
            cooldownTime = 5;

        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
    }
}