
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    [Header("Recoil")]
    public float recoilRecoverySpeed = 8f;

    [Header("Damage Camera Shake")]
    public float damageShakeAmount = 2f;
    public float damageShakeDuration = 0.15f;

    [Header("Death Camera")]
    public float deathFallDuration = 1.2f;
    public float deathFallDistance = 1.2f;
    public float deathFallAngle = 70f;

    private bool isDying = false;
    private bool gameStopped = false;

    private float deathTimer = 0f;
    private Vector3 deathStartPosition;
    private Quaternion deathStartRotation;

    private float damageShakeTimer = 0f;

    private CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f;

    // Store the current recoil rotation
    private Vector2 recoilRotation;

    // Store the target recoil added by the gun
    private Vector2 targetRecoil;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
void Update()
{
    // Always allow the death animation to finish
    if (isDying)
    {
        HandleDeathAnimation();

        // Stop gameplay after the death animation finishes
        if (deathTimer >= deathFallDuration)
        {
            gameStopped = true;
        }

        return;
    }

    // Stop normal gameplay after the game ends
    if (gameStopped)
    {
        return;
    }

    Move();
    Look();
    HandleRecoil();
    HandleDamageShake();
}


    void Move()
    {
        // Get keyboard input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x
                     + transform.forward * z;

        // Check if the player is grounded
        bool grounded = controller.isGrounded;

        // Reset downward velocity when grounded
        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // Jump when Space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = move * moveSpeed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X")
                     * mouseSensitivity;

        float mouseY = Input.GetAxis("Mouse Y")
                     * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        xRotation -= mouseY;

        float finalXRotation = xRotation + recoilRotation.x;

        finalXRotation = Mathf.Clamp(
            finalXRotation,
            -90f,
            90f
        );

        // Calculate damage camera shake
        float shakeX = 0f;
        float shakeY = 0f;

        if (damageShakeTimer > 0f)
        {
            shakeX = Random.Range(
                -damageShakeAmount,
                damageShakeAmount
            );

            shakeY = Random.Range(
                -damageShakeAmount,
                damageShakeAmount
            );
        }

        // Apply camera rotation, recoil and damage shake
        playerCamera.localRotation = Quaternion.Euler(
            finalXRotation + shakeX,
            recoilRotation.y + shakeY,
            0f
        );
    }

    void HandleRecoil()
    {
        // Gradually recover the target recoil
        targetRecoil = Vector2.Lerp(
            targetRecoil,
            Vector2.zero,
            recoilRecoverySpeed * Time.deltaTime
        );

        // Smoothly apply the recoil recovery
        recoilRotation = Vector2.Lerp(
            recoilRotation,
            targetRecoil,
            recoilRecoverySpeed * Time.deltaTime
        );
    }

    void HandleDamageShake()
    {
        // Reduce the damage shake timer
        if (damageShakeTimer > 0f)
        {
            damageShakeTimer -= Time.deltaTime;
        }
    }

    public void AddRecoil(float recoilUp, float recoilSide)
    {
        // Add upward recoil
        targetRecoil.x -= recoilUp;

        // Add random horizontal recoil
        targetRecoil.y += Random.Range(
            -recoilSide,
            recoilSide
        );
    }

    public void DamageShake()
    {
        // Start the damage camera shake
        damageShakeTimer = damageShakeDuration;
    }

    public void StartDeathAnimation()
    {
        // Prevent the animation from starting twice
        if (isDying || gameStopped)
        {
            return;
        }

        isDying = true;
        deathTimer = 0f;

        // Save the starting camera position and rotation
        deathStartPosition = playerCamera.localPosition;
        deathStartRotation = playerCamera.localRotation;
    }

    void HandleDeathAnimation()
    {
        deathTimer += Time.deltaTime;

        // Calculate animation progress
        float progress = Mathf.Clamp01(
            deathTimer / deathFallDuration
        );

        // Smooth the animation
        float smoothProgress = Mathf.SmoothStep(
            0f,
            1f,
            progress
        );

        // Calculate the final camera position
        Vector3 targetPosition = deathStartPosition
            + Vector3.down * deathFallDistance;

        // Move the camera downward
        playerCamera.localPosition = Vector3.Lerp(
            deathStartPosition,
            targetPosition,
            smoothProgress
        );

        // Calculate the final camera rotation
        Quaternion targetRotation = deathStartRotation
            * Quaternion.Euler(deathFallAngle, 0f, 0f);

        // Tilt the camera during the death animation
        playerCamera.localRotation = Quaternion.Slerp(
            deathStartRotation,
            targetRotation,
            smoothProgress
        );
    }

    public void StopGame()
    {
        // Stop player controls when the game ends
        gameStopped = true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Only lock the cursor while gameplay is active
        if (hasFocus && !gameStopped)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
