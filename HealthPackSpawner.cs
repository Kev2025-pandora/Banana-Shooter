using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    [Header("Health Pack")]
    public GameObject healthPackPrefab;

    [Header("Random Spawn Area")]
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;

    [Header("Spawn Settings")]
    public float spawnDelay = 10f;

    private GameObject currentHealthPack;
    private float nextSpawnTime = 0f;

    void Start()
    {
        // Spawn the first health pack immediately
        SpawnHealthPack();
    }

    void Update()
    {
        // Check if the health pack has been picked up
        if (currentHealthPack == null)
        {
            // Start counting after the health pack is picked up
            if (nextSpawnTime == 0f)
            {
                nextSpawnTime = Time.time + spawnDelay;
            }

            // Spawn a new health pack after 10 seconds
            if (Time.time >= nextSpawnTime)
            {
                SpawnHealthPack();

                // Reset the timer
                nextSpawnTime = 0f;
            }
        }
    }

    void SpawnHealthPack()
    {
        // Generate a random position
        float randomX =
            Random.Range(-spawnRangeX, spawnRangeX);

        float randomZ =
            Random.Range(-spawnRangeZ, spawnRangeZ);

        Vector3 randomPosition =
            transform.position +
            new Vector3(randomX, 0f, randomZ);

        // Create the health pack
        currentHealthPack = Instantiate(
            healthPackPrefab,
            randomPosition,
            Quaternion.identity
        );
    }
}