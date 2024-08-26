using UnityEngine;
using System.Collections.Generic;

public class BarrierSpawner : MonoBehaviour
{
    public List<GameObject> smallBarrierPrefabs;   // List of small barrier prefabs
    public List<GameObject> fullBarrierPrefabs;    // List of full barrier prefabs
    public Transform playerTransform;              // Reference to the player's transform
    public float minSpawnDistance = 5f;            // Minimum distance between each barrier spawn
    public float maxSpawnDistance = 15f;           // Maximum distance between each barrier spawn
    public float spawnDistanceFromPlayer = 100f;   // Distance in front of the player where barriers will spawn

    private float lastSpawnZ;                      // The Z position where the last barrier was spawned
    private int[] lanePositions = { -2, 0, 2 };    // Lane positions (assuming lanes are -2, 0, and 2 on the X axis)

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
        // Decide whether to spawn a small barrier or a full barrier
        bool spawnFullBarrier = (Random.value < 0.5f); // 50% chance to spawn a full barrier

        if (spawnFullBarrier && fullBarrierPrefabs.Count > 0)
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
    }
}
