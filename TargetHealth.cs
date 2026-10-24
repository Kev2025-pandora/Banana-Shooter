using UnityEngine;
using System.Collections;

public class TargetHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float currentHealth;
    private bool isDead = false;
    private EnemySpawner spawner;

    void Start()
    {
        // Set health to maximum
        currentHealth = maxHealth;
    }

    public void SetSpawner(EnemySpawner newSpawner)
    {
        spawner = newSpawner;
    }

    public void TakeDamage(float damage)
    {
        // Don't take damage after death
        if (isDead)
        {
            return;
        }

        // Reduce health
        currentHealth -= damage;

        Debug.Log("Enemy Health: " + currentHealth);

        // Check if health reaches zero
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Enemy died!");

        // Tell the spawner that this enemy has died
        if (spawner != null)
        {
            spawner.EnemyDied();
        }

        // Stop the enemy AI
        EnemyAI enemyAI =
            GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // Play the death animation
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Save the starting rotation
        Quaternion startRotation = transform.rotation;

        // Calculate the final rotation
        Quaternion endRotation =
            startRotation * Quaternion.Euler(90f, 0f, 0f);

        float duration = 0.5f;
        float timer = 0f;

        // Smoothly rotate the enemy
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = timer / duration;

            // Smooth the animation
            progress = Mathf.SmoothStep(0f, 1f, progress);

            transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    progress
                );

            yield return null;
        }

        // Make sure the final rotation is exact
        transform.rotation = endRotation;

        // Wait before destroying the enemy
        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}