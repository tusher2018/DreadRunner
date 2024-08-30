using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BarrierSettings
{
    public GameObject prefab;  // The barrier prefab
    public int priority;       // Priority level (higher means it spawns less frequently at the beginning)
}

public class BarrierSpawner : MonoBehaviour
{
    public List<BarrierSettings> smallBarrierSettings;  // List of small barrier settings
    public List<BarrierSettings> fullBarrierSettings;   // List of full barrier settings
    public List<GameObject> environmentPrefabs;         // List of environment prefabs
    public GameObject goldPrefab;                       // Gold prefab for spawning gold
    public Transform playerTransform;                   // Reference to the player's transform

    // Initial distances
    public float initialMinDistance = 5f;               // Initial minimum distance between each barrier spawn
    public float initialMaxDistance = 15f;              // Initial maximum distance between each barrier spawn

    // Distances when hard mode is reached
    public float hardTimeMinDistance = 3f;              // Minimum distance in hard mode
    public float hardTimeMaxDistance = 10f;             // Maximum distance in hard mode

    public float spawnDistanceFromPlayer = 100f;        // Distance in front of the player where barriers will spawn
    public float goldSpacing = 1.5f;                    // Spacing between gold objects

    public float initialSpawnRate = 0.5f;               // Initial spawn rate (0.0 - 1.0)
    public float maxSpawnRate = 1.0f;                   // Maximum spawn rate over time (0.0 - 1.0)
    public float difficultyRampUpTime = 120f;           // Time in seconds to reach maximum difficulty
    public float hardModeTime = 60f;                    // Time in seconds to reach hard mode

    private float lastSpawnZ;                           // The Z position where the last barrier was spawned
    private int[] lanePositions = { -2, 0, 2 };         // Lane positions (assuming lanes are -2, 0, and 2 on the X axis)
    private float elapsedTime = 0f;                     // Time elapsed since the start of the game

    void Start()
    {
        // Initialize the last spawn position as being behind the player so the first spawn happens immediately
        lastSpawnZ = playerTransform.position.z - initialMaxDistance;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check if the player has moved far enough to spawn a new barrier
        if (playerTransform.position.z > lastSpawnZ + GetCurrentSpawnDistance())
        {
            SpawnBarrier();
            lastSpawnZ = playerTransform.position.z;
        }
    }

    void SpawnBarrier()
    {
        float spawnChance = GetAdjustedSpawnRate(); // Adjusted spawn rate over time

        // Decide whether to spawn an environment object, a small barrier, or a full barrier
        if (Random.value < 0.2f && environmentPrefabs.Count > 0) // 20% chance to spawn an environment
        {
            // Spawn an environment object
            SpawnEnvironment();
        }
        else if (Random.value < 0.7f * spawnChance && fullBarrierSettings.Count > 0) // Adjusted chance to spawn a full barrier
        {
            // Spawn a full barrier
            SpawnFullBarrier();
        }
        else if (smallBarrierSettings.Count > 0)
        {
            // Spawn a small barrier
            SpawnSmallBarrier();
        }

        // Randomly decide whether to spawn gold (independent spawn logic)
        if (Random.value < 0.3f * spawnChance) // Adjusted chance to spawn a gold cluster
        {
            SpawnGoldCluster();
        }
    }

    float GetAdjustedSpawnRate()
    {
        // Gradually increase spawn rate over time, starting from initialSpawnRate and reaching maxSpawnRate
        return Mathf.Lerp(initialSpawnRate, maxSpawnRate, elapsedTime / difficultyRampUpTime);
    }

    float GetCurrentSpawnDistance()
    {
        // Adjust the spawn distances based on elapsed time
        float minDistance = Mathf.Lerp(initialMinDistance, hardTimeMinDistance, elapsedTime / hardModeTime);
        float maxDistance = Mathf.Lerp(initialMaxDistance, hardTimeMaxDistance, elapsedTime / hardModeTime);

        // Return a random value between the current min and max distances
        return Random.Range(minDistance, maxDistance);
    }

    void SpawnEnvironment()
    {
        int randomIndex = Random.Range(0, environmentPrefabs.Count);
        GameObject selectedEnvironment = environmentPrefabs[randomIndex];
        Vector3 spawnPosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);

        // Instantiate the selected environment object at the determined position
        Instantiate(selectedEnvironment, spawnPosition, Quaternion.identity);
    }

    void SpawnFullBarrier()
    {
        BarrierSettings selectedBarrier = SelectBarrier(fullBarrierSettings);
        Vector3 spawnPosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);

        // Instantiate the selected full barrier at the determined position
        Instantiate(selectedBarrier.prefab, spawnPosition, Quaternion.identity);
    }

    void SpawnSmallBarrier()
    {
        BarrierSettings selectedBarrier = SelectBarrier(smallBarrierSettings);
        int laneIndex = Random.Range(0, lanePositions.Length);
        float lanePosition = lanePositions[laneIndex];
        Vector3 spawnPosition = new Vector3(lanePosition, 0, playerTransform.position.z + spawnDistanceFromPlayer);

        // Instantiate the selected small barrier at the determined position
        Instantiate(selectedBarrier.prefab, spawnPosition, Quaternion.identity);
    }

    BarrierSettings SelectBarrier(List<BarrierSettings> barrierSettings)
    {
        // Calculate total weight based on priority and adjusted spawn rate
        int totalWeight = 0;
        foreach (var barrier in barrierSettings)
        {
            // As difficulty increases, all barriers should have an equal chance to spawn
            int adjustedPriority = Mathf.Max(1, barrier.priority - (int)(elapsedTime / difficultyRampUpTime * (barrier.priority - 1)));
            totalWeight += adjustedPriority;
        }

        // Select a barrier based on weighted random
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

        return barrierSettings[0]; // Default to the first barrier if something goes wrong
    }

    void SpawnGoldCluster()
    {
        int goldCount = Random.Range(10, 21); // Random number of golds between 10 and 20
        int laneIndex = Random.Range(0, lanePositions.Length); // Randomly select a lane
        float lanePosition = lanePositions[laneIndex];

        for (int i = 0; i < goldCount; i++)
        {
            Vector3 spawnPosition = new Vector3(lanePosition, 0, playerTransform.position.z + spawnDistanceFromPlayer + (i * goldSpacing));
            Instantiate(goldPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
