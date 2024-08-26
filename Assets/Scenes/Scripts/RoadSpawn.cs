using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject roadPrefab;     // The road segment prefab to spawn
    public float roadLength = 10f;    // Length of each road segment
    public float spawnDistance = 30f; // Distance from the spawner at which new road segments are spawned
    public float moveSpeed = 5f;      // Speed at which the spawner moves forward

    private float spawnZ = 0f;        // Z position of the next road segment to spawn

    void Start()
    {
        // Initialize the spawn position based on the roadLength
        spawnZ = transform.position.z;
    }

    void Update()
    {
        // Move the spawner forward at a constant speed
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;

        // Check if it's time to spawn a new road segment
        if (transform.position.z + spawnDistance > spawnZ)
        {
            SpawnRoad();
        }
    }

    void SpawnRoad()
    {
        // Calculate the global position where the road segment will be spawned
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, spawnZ);

        // Instantiate a new road segment at the calculated global position
        Instantiate(roadPrefab, spawnPosition, Quaternion.identity);

        // Update the position for the next road segment
        spawnZ += roadLength;
    }
}
