using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BarrierSettings
{
    public GameObject prefab;  // The barrier prefab
    public int priority;       // Priority level (higher means it spawns less frequently at the beginning)
}

[System.Serializable]
public class EnvironmentObject
{
    public GameObject prefab;     // The environment prefab
    public float spawnDistance;   // Distance to the next environment object in the z-axis
}

[System.Serializable]
public class RoadEnvironment
{
    
    public GameObject roadPrefab;                         // The road prefab
    public float roadLength;  
    public List<EnvironmentObject> fullEnvironments;
    public List<EnvironmentObject> leftSideEnvironments; // List of left-side environment objects
    public List<EnvironmentObject> rightSideEnvironments;// List of right-side environment objects
}

public class AllSpawner : MonoBehaviour
{
    public List<BarrierSettings> smallBarrierSettings;  // Small barriers
    public List<BarrierSettings> fullBarrierSettings;   // Full barriers
    public List<RoadEnvironment> roadEnvironments;      // List of road environments (with specific environments)
    public GameObject goldPrefab;                       // Gold prefab
    public GameObject gatePrefab;                       // Gate prefab for road transition

    public Transform playerTransform;                   // Player's transform

                        // Length of each road segment
    public float spawnDistanceFromPlayer = 200f;        // Distance ahead of the player to spawn objects
    public float goldSpacing = 1.5f;                    // Spacing between gold objects

    public float initialMinDistance = 5f;               // Initial min distance between barrier spawns
    public float initialMaxDistance = 15f;              // Initial max distance between barrier spawns
    public float initialSpawnRate = 0.5f;               // Initial spawn rate

    public float hardTimeMinDistance = 3f;              // Hard mode min distance
    public float hardTimeMaxDistance = 10f;             // Hard mode max distance
    public float maxSpawnRate = 1.0f;                   // Max spawn rate
    public float difficultyRampUpTime = 120f;           // Time to reach max difficulty
    public float hardModeTime = 60f;                    // Time to reach hard mode
    public float mapSwitchInterval = 120f;              // Time after which to switch to the next map

    private float lastSpawnZ;                           // Last spawn position
    private float spawnZ;                               // Next road segment spawn position
    private float elapsedTime = 0f;                     // Time elapsed since game start
    public int[] lanePositions = { -2, 0, 2 };         // Lane positions for barriers
    public float laneOffset=2;
    private float moveSpeed = 5f;                       // Spawner's move speed
    private int currentMapIndex = 0;                    // Track the current map index
    private float lastMapSwitchTime = 0f;               // Last time a map switch occurred

private Vector3 lastLeftEnvironmentPosition;
private Vector3 lastRightEnvironmentPosition;

public float maxSpawnDistanceForEnvironment = 400.0f;  




    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        lastSpawnZ = playerTransform.position.z - initialMaxDistance;
        spawnZ = transform.position.z;
        
    }

    void Update()
    {
        if(playerTransform== null){
            GameObject playerObject=GameObject.FindWithTag("Player");
            if(playerObject==null){return;}
                  playerTransform = playerObject.transform;
        lastSpawnZ = playerTransform.position.z - initialMaxDistance;
        spawnZ = transform.position.z;
        }
        elapsedTime += Time.deltaTime;

        // Move the spawner forward at the player's speed
        moveSpeed = playerTransform.GetComponent<PlayerController>().forwardSpeed;
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        // Check if it's time to spawn a new road segment
        if (transform.position.z + spawnDistanceFromPlayer > spawnZ)
        {
            SpawnRoad();
        }

        // Check if the player has moved far enough to spawn a new barrier
        if (playerTransform.position.z > lastSpawnZ + GetCurrentSpawnDistance())
        {
            SpawnBarrier();
            lastSpawnZ = playerTransform.position.z;
        }

        // Check if it's time to switch to the next map
        if (elapsedTime - lastMapSwitchTime >= mapSwitchInterval)
        {
            SwitchToNextMap();
            lastMapSwitchTime = elapsedTime;
        }
    }

    void SpawnRoad()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, spawnZ);

        // Spawn the current road prefab
        Instantiate(roadEnvironments[currentMapIndex].roadPrefab, spawnPosition, Quaternion.identity);
        spawnZ += roadEnvironments[currentMapIndex].roadLength;
    }

    void SwitchToNextMap()
    {
        // Switch to the next map in the list
        currentMapIndex = (currentMapIndex + 1) % roadEnvironments.Count;

        // Spawn a gate to mark the transition
        Vector3 gatePosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);
        Instantiate(gatePrefab, gatePosition, Quaternion.identity);
    }

    void SpawnBarrier()
    {
        float spawnChance = GetAdjustedSpawnRate();
        // float spawnChance = Random.value;
    
            SpawnEnvironment();
        
        
        if (Random.value < 0.5f )
        {
            SpawnFullBarrier();
        }
        else
        {
            SpawnSmallBarrier();
        }

        if (Random.value < 0.4f * spawnChance) // 40% chance to spawn gold
        {
            SpawnGoldCluster();
        }
    }

    float GetAdjustedSpawnRate()
    {
        return Mathf.Lerp(initialSpawnRate, maxSpawnRate, elapsedTime / difficultyRampUpTime);
    }

    float GetCurrentSpawnDistance()
    {
        float minDistance = Mathf.Lerp(initialMinDistance, hardTimeMinDistance, elapsedTime / hardModeTime);
        float maxDistance = Mathf.Lerp(initialMaxDistance, hardTimeMaxDistance, elapsedTime / hardModeTime);

        return Random.Range(minDistance, maxDistance);
    }

void SpawnEnvironment()
{
    float spawnChance = Random.value;  // Random value between 0.0 and 1.0

    RoadEnvironment currentEnvironment = roadEnvironments[currentMapIndex];

    if (currentEnvironment.fullEnvironments.Count > 0 && spawnChance <= 0.5f){

        int Index = currentMapIndex % currentEnvironment.fullEnvironments.Count;
        EnvironmentObject Environment = currentEnvironment.fullEnvironments[Index];
        Vector3 spawnPosition;
        if(lastRightEnvironmentPosition.z>lastLeftEnvironmentPosition.z){
         spawnPosition= lastRightEnvironmentPosition + new Vector3(0, 0, Environment.spawnDistance);
         }else{
            spawnPosition= lastLeftEnvironmentPosition + new Vector3(0, 0, Environment.spawnDistance);
         }

        // Check distance from player
        if (Vector3.Distance(playerTransform.position, spawnPosition) <= maxSpawnDistanceForEnvironment)
        {
            Instantiate(Environment.prefab, spawnPosition, Quaternion.identity);
            lastRightEnvironmentPosition = spawnPosition;  // Update the last position for the right side
            lastLeftEnvironmentPosition = spawnPosition;  // Update the last position for the left side
        }

    }else
{
        // Spawn the next left-side environment sequentially
        if (currentEnvironment.leftSideEnvironments.Count > 0)
        {
            int leftIndex = currentMapIndex % currentEnvironment.leftSideEnvironments.Count;
            EnvironmentObject leftEnvironment = currentEnvironment.leftSideEnvironments[leftIndex];

            Vector3 spawnPosition = lastLeftEnvironmentPosition + new Vector3(0, 0, leftEnvironment.spawnDistance);

            // Check distance from player
            if (Vector3.Distance(playerTransform.position, spawnPosition) <= maxSpawnDistanceForEnvironment)
            {
                Instantiate(leftEnvironment.prefab, spawnPosition, Quaternion.identity);
                lastLeftEnvironmentPosition = spawnPosition;  // Update the last position for the left side
            }
        }

        // Spawn the next right-side environment sequentially
        if (currentEnvironment.rightSideEnvironments.Count > 0)
        {
            int rightIndex = currentMapIndex % currentEnvironment.rightSideEnvironments.Count;
            EnvironmentObject rightEnvironment = currentEnvironment.rightSideEnvironments[rightIndex];

            Vector3 spawnPosition = lastRightEnvironmentPosition + new Vector3(0, 0, rightEnvironment.spawnDistance);

            // Check distance from player
            if (Vector3.Distance(playerTransform.position, spawnPosition) <= maxSpawnDistanceForEnvironment)
            {
                Instantiate(rightEnvironment.prefab, spawnPosition, Quaternion.identity);
                lastRightEnvironmentPosition = spawnPosition;  // Update the last position for the right side
            }
        }
    }
    // Update to the next environment for the next call
    // currentMapIndex = (currentMapIndex + 1) % roadEnvironments.Count;
}




    void SpawnFullBarrier()
    {
        BarrierSettings selectedBarrier = SelectBarrier(fullBarrierSettings);
        Vector3 spawnPosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);

        Instantiate(selectedBarrier.prefab, spawnPosition, Quaternion.identity);
    }

    void SpawnSmallBarrier()
    {
        BarrierSettings selectedBarrier = SelectBarrier(smallBarrierSettings);
        int laneIndex = Random.Range(0, lanePositions.Length);
        float lanePosition = lanePositions[laneIndex]*(laneOffset/2);
        Vector3 spawnPosition = new Vector3(lanePosition, 0, playerTransform.position.z + spawnDistanceFromPlayer);

        Instantiate(selectedBarrier.prefab, spawnPosition, Quaternion.identity);
    }

    BarrierSettings SelectBarrier(List<BarrierSettings> barrierSettings)
    {
        int totalWeight = 0;
        foreach (var barrier in barrierSettings)
        {
            int adjustedPriority = Mathf.Max(1, barrier.priority - (int)(elapsedTime / difficultyRampUpTime * (barrier.priority - 1)));
            totalWeight += adjustedPriority;
        }

        int randomWeight = Random.Range(0, totalWeight);
        foreach (var barrier in barrierSettings)
        {
            int adjustedPriority = Mathf.Max(1, barrier.priority - (int)(elapsedTime / difficultyRampUpTime * (barrier.priority - 1)));
            if (randomWeight < adjustedPriority)
            {
                return barrier;
            }
            randomWeight -= adjustedPriority;
        }

        return barrierSettings[0];
    }

    void SpawnGoldCluster()
    {
        int goldCount = Random.Range(10, 21); 
        int laneIndex = Random.Range(0, lanePositions.Length); 
        float lanePosition = lanePositions[laneIndex]*(laneOffset/2);

        for (int i = 0; i < goldCount; i++)
        {
            Vector3 spawnPosition = new Vector3(lanePosition, 0, playerTransform.position.z + spawnDistanceFromPlayer + (i * goldSpacing));
            Instantiate(goldPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
