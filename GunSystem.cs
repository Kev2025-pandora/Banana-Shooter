using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [Header("Muzzle Flash")]
    public ParticleSystem MuzzleFlash;

    [Header("Gun Sound")]
    public AudioSource gunAudio;
    public AudioClip gunSound;

    [Header("Gun Settings")]
    public Camera playerCamera;
    public float range = 100f;
    public float fireRate = 0.2f;

    [Header("Recoil Settings")]
    public float recoilUp = 2f;
    public float recoilSide = 0.5f;

    [Header("Gun Animation")]
    public float gunKickBack = 0.08f;
    public float gunKickUp = 5f;
    public float gunReturnSpeed = 12f;

    private float nextFireTime = 0f;

    // Reference to the FPS controller
    private FPSController fpsController;

    // Store the gun's original position and rotation
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Store the current animation position and rotation
    private Vector3 currentPosition;
    private Quaternion currentRotation;

    

    void Start()
    {
        // Automatically find the camera if none is assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Find the FPS controller in the scene
        fpsController = FindFirstObjectByType<FPSController>();

        // Save the gun's original transform
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        currentPosition = originalPosition;
        currentRotation = originalRotation;
    }

    void Update()
    {
        // Check if the player presses the left mouse button
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Smoothly return the gun to its original position
        currentPosition = Vector3.Lerp(
            currentPosition,
            originalPosition,
            gunReturnSpeed * Time.deltaTime
        );

        // Smoothly return the gun to its original rotation
        currentRotation = Quaternion.Slerp(
            currentRotation,
            originalRotation,
            gunReturnSpeed * Time.deltaTime
        );

        // Apply the animation
        transform.localPosition = currentPosition;
        transform.localRotation = currentRotation;
    }

    void Shoot()
    {
        // Create a ray from the center of the camera
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        // Store information about the object hit by the ray
        RaycastHit hit;

        // Cast the ray and check if it hits an object
        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(
                "Hit: " + hit.collider.gameObject.name
            );

            Debug.Log(
                "Hit Point: " + hit.point
            );

            // Try to find a TargetHealth component
            TargetHealth targetHealth =
                hit.collider.GetComponent<TargetHealth>();

            // Check if the object has a TargetHealth component
            if (targetHealth != null)
            {
                // Deal 25 damage to the target
                targetHealth.TakeDamage(25f);
            }
            else
            {
                Debug.Log("TargetHealth not found!");
            }
        }
        else
        {
            Debug.Log("Shot missed.");
        }

        // Add camera recoil
        if (fpsController != null)
        {
            fpsController.AddRecoil(
                recoilUp,
                recoilSide
            );
        }

        // Play the gun animation
        PlayGunAnimation();

        if (MuzzleFlash != null)
        {
            MuzzleFlash.Play();
        }

        // Play the gunshot sound
    if (gunAudio != null && gunSound != null)
        {
            gunAudio.PlayOneShot(gunSound);
        }
    }

    void PlayGunAnimation()
    {
        // Move the gun backward
        currentPosition += new Vector3(
            0f,
            0f,
            -gunKickBack
        );

        // Rotate the gun upward
        currentRotation *= Quaternion.Euler(
            -gunKickUp,
            0f,
            0f
        );
    }
}