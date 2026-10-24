
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 1f;
    public float stopDistance = 10f;

    private Transform player;
    private PlayerHealth playerHealth;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    private float nextAttackTime = 0f;

    void Start()
    {
        // Find the player by tag
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // Stop if the player cannot be found or is dead
        if (player == null || playerHealth == null)
        {
            return;
        }

        if (playerHealth.IsDead)
        {
            return;
        }

        // Calculate the distance to the player
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Move toward the player
        if (distance > stopDistance)
        {
            Vector3 direction =
                (player.position - transform.position).normalized;

            transform.position +=
                direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            Attack();
        }

        // Face the player
        Vector3 lookDirection =
            player.position - transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(lookDirection);
        }
    }

    void Attack()
    {
        // Check the attack cooldown
        if (Time.time < nextAttackTime)
        {
            return;
        }

        // Deal damage to the player
        playerHealth.TakeDamage(attackDamage);

        Debug.Log("Enemy attacked the player!");

        nextAttackTime = Time.time + attackCooldown;
    }
}
