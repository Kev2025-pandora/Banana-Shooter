using UnityEngine;
using TMPro;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health UI")]
    public TMP_Text healthText;

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Hurt Sound")]
    public AudioSource hurtAudio;
    public AudioClip hurtSound;

    private float currentHealth;
    private bool isDead = false;
    public bool IsDead => isDead;

    void Start()
    {
        // Set the player's health to the maximum health
        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
{
    // Do nothing if the player is already dead
    if (isDead)
    {
        return;
    }

    // Reduce the player's health
    currentHealth -= damage;

    // Prevent health from going below zero
    currentHealth = Mathf.Max(currentHealth, 0f);

    Debug.Log("Player Health: " + currentHealth);

    // Update the health UI
    UpdateHealthUI();

    // Play the hurt sound
    if (hurtAudio != null && hurtSound != null)
    {
        hurtAudio.PlayOneShot(hurtSound);
    }

    // Shake the camera
    FPSController controller =
        GetComponent<FPSController>();

    if (controller != null)
    {
        controller.DamageShake();
    }

    // Check if the player has died
    if (currentHealth <= 0f)
    {
        Die();
    }
}
   

    
void Die()
{
    // Prevent the player from taking damage after death
    isDead = true;

    Debug.Log("Player died!");

    // Start the death camera animation
    FPSController controller = GetComponent<FPSController>();

    if (controller != null)
    {
        controller.StartDeathAnimation();
    }

    // Disable the gun
    GunSystem gun = GetComponentInChildren<GunSystem>();

    if (gun != null)
    {
        gun.enabled = false;
    }

    // Unlock the cursor
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    // Show the Game Over screen
    GameManager gameManager = FindFirstObjectByType<GameManager>();

    if (gameManager != null)
    {
        gameManager.GameOver();
    }
    else
    {
        Debug.LogError("GameManager not found in the scene!");
    }
}

void UpdateHealthUI()
{
    if (healthText != null)
    {
        healthText.text =
            "Health: " + Mathf.CeilToInt(currentHealth);
    }
}
public void Heal(float amount)
{
    // Increase the player's health
    currentHealth += amount;

    // Prevent health from exceeding maximum health
    currentHealth = Mathf.Min(
        currentHealth,
        maxHealth
    );

    Debug.Log("Player healed: " + currentHealth);

    // Update the health UI
    UpdateHealthUI();
}
}