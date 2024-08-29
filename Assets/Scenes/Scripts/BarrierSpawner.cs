using UnityEngine;
using System.Collections.Generic;

public class BarrierSpawner : MonoBehaviour
{
    public List<GameObject> smallBarrierPrefabs;   // List of small barrier prefabs
    public List<GameObject> fullBarrierPrefabs;    // List of full barrier prefabs
    public List<GameObject> environmentPrefabs;    // List of environment prefabs
    public GameObject goldPrefab;                  // Gold prefab for spawning gold
    public Transform playerTransform;              // Reference to the player's transform
    public float minSpawnDistance = 5f;            // Minimum distance between each barrier spawn
    public float maxSpawnDistance = 15f;           // Maximum distance between each barrier spawn
    public float spawnDistanceFromPlayer = 100f;   // Distance in front of the player where barriers will spawn
    public float goldSpacing = 1.5f;               // Spacing between gold objects

    private float lastSpawnZ;                      // The Z position where the last barrier was spawned
    public int[] lanePositions = { -2, 0, 2 };     // Lane positions (assuming lanes are -2, 0, and 2 on the X axis)

    void Start()
    {
        // Initialize the last spawn position as being behind the player so the first spawn happens immediately
        lastSpawnZ = playerTransform.position.z - maxSpawnDistance;
    }

    void Update()
    {
        // Check if the player has moved far enough to spawn a new barrier
        if (playerTransform.position.z > lastSpawnZ + Random.Range(minSpawnDistance, maxSpawnDistance))
        {
            SpawnBarrier();
            lastSpawnZ = playerTransform.position.z;
        }
    }

    void SpawnBarrier()
    {
        // Decide whether to spawn an environment object, a small barrier, or a full barrier
        float spawnChance = Random.value; // Random value between 0 and 1

        if (spawnChance < 0.2f && environmentPrefabs.Count > 0) // 20% chance to spawn an environment
        {
            // Spawn an environment object
            int randomIndex = Random.Range(0, environmentPrefabs.Count);
            GameObject selectedEnvironment = environmentPrefabs[randomIndex];
            Vector3 spawnPosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);

            // Instantiate the selected environment object at the determined position
            Instantiate(selectedEnvironment, spawnPosition, Quaternion.identity);
        }
        else if (spawnChance < 0.7f && fullBarrierPrefabs.Count > 0) // 50% chance to spawn a full barrier
        {
            // Spawn a full barrier in the middle lane
            int randomIndex = Random.Range(0, fullBarrierPrefabs.Count);
            GameObject selectedBarrier = fullBarrierPrefabs[randomIndex];
            Vector3 spawnPosition = new Vector3(0, 0, playerTransform.position.z + spawnDistanceFromPlayer);

            // Instantiate the selected full barrier at the determined position
            Instantiate(selectedBarrier, spawnPosition, Quaternion.identity);
        }
        else if (smallBarrierPrefabs.Count > 0)
        {
            // Spawn a small barrier in a random lane
            int randomIndex = Random.Range(0, smallBarrierPrefabs.Count);
            GameObject selectedBarrier = smallBarrierPrefabs[randomIndex];
            int laneIndex = Random.Range(0, lanePositions.Length);
            float lanePosition = lanePositions[laneIndex];
            Vector3 spawnPosition = new Vector3(lanePosition, 0, playerTransform.position.z + spawnDistanceFromPlayer);

            // Instantiate the selected small barrier at the determined position
            Instantiate(selectedBarrier, spawnPosition, Quaternion.identity);
        }

        // Randomly decide whether to spawn gold (independent spawn logic)
        if (Random.value < 0.3f) // 30% chance to spawn a gold cluster
        {
            SpawnGoldCluster();
        }
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
