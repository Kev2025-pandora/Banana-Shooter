using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [Header("Health Pack")]
    public float healAmount = 25f;

    [Header("Floating Animation")]
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;

    [Header("Rotation Animation")]
    public float rotationSpeed = 60f;

    [Header("Heal Sound")]
    public AudioClip healSound;
    


    private Vector3 startPosition;

    void Start()
    {
        // Save the original position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the vertical floating movement
        float newY =
            startPosition.y +
            Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Apply the floating position
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // Rotate the health pack
        transform.Rotate(
            Vector3.up * rotationSpeed * Time.deltaTime
        );
    }

    

private void OnTriggerEnter(Collider other)
{
    // Check if the object is the player
    if (other.CompareTag("Player"))
    {
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Heal the player
            playerHealth.Heal(healAmount);

            // Play the heal sound
            if (healSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    healSound,
                    transform.position
                );
            }

            // Destroy the health pack
            Destroy(gameObject);
        }
    }
}
}