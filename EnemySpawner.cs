using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public int startingEnemies = 3;
    public int enemiesIncreasePerWave = 2;
    public int maxWaves = 5;

    [Header("Spawn Area")]
    public float spawnRange = 10f;

    [Header("Spawn Settings")]
    public float spawnDelay = 0.5f;

    [Header("Game Manager")]
    public GameManager gameManager;

    private int currentWave = 0;
    private int enemiesToSpawn;
    private int enemiesAlive;

    private float nextSpawnTime = 0f;
    private bool spawning = false;
    private bool gameWon = false;

    void Start()
    {
        // Start the first wave
        StartNextWave();
    }

    void Update()
    {
        if (gameWon)
        {
            return;
        }

        // Spawn enemies during the current wave
        if (spawning)
        {
            if (enemiesToSpawn > 0 && Time.time >= nextSpawnTime)
            {
                SpawnEnemy();

                enemiesToSpawn--;

                nextSpawnTime =
                    Time.time + spawnDelay;
            }

            // Stop spawning when all enemies have been created
            if (enemiesToSpawn <= 0)
            {
                spawning = false;
            }
        }

        // Check if the current wave is completely finished
        if (!spawning && enemiesAlive <= 0)
        {
            if (currentWave >= maxWaves)
            {
                WinGame();
            }
            else
            {
                StartNextWave();
            }
        }
    }

    void StartNextWave()
    {
        currentWave++;

        // Calculate how many enemies this wave has
        enemiesAlive = startingEnemies +
                       (currentWave - 1) *
                       enemiesIncreasePerWave;

        enemiesToSpawn = enemiesAlive;

        spawning = true;

        // Update the game UI
        if (gameManager != null)
        {
            gameManager.UpdateGameUI(
                currentWave,
                enemiesAlive
            );
        }

        Debug.Log(
            "Wave " + currentWave +
            " started! Enemies: " +
            enemiesAlive
        );
    }

    void SpawnEnemy()
    {
        // Generate a random spawn position
        Vector3 randomPosition =
            transform.position +
            new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );

        GameObject enemy = Instantiate(
            enemyPrefab,
            randomPosition,
            transform.rotation
        );

        // Get the enemy's health component
        TargetHealth targetHealth =
            enemy.GetComponent<TargetHealth>();

        if (targetHealth != null)
        {
            // Tell the enemy spawner when this enemy dies
            targetHealth.SetSpawner(this);
        }
    }

    public void EnemyDied()
    {
        // Reduce the number of living enemies
        enemiesAlive--;

        // Update the game UI
        if (gameManager != null)
        {
            gameManager.UpdateGameUI(
                currentWave,
                enemiesAlive
            );
        }

        Debug.Log(
            "Enemies remaining: " +
            enemiesAlive
        );
    }

    void WinGame()
{
    gameWon = true;

    if (gameManager != null)
    {
        gameManager.WinGame();
    }

    Debug.Log("YOU WIN!");
}
    public int GetCurrentWave()
    {
        return currentWave;
    }

    public int GetEnemiesAlive()
    {
        return enemiesAlive;
    }
}