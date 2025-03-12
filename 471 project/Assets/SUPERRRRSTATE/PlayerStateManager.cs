using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerStateManager : MonoBehaviour
{
    [HideInInspector] public PlayerBaseState currentState;
    [HideInInspector] public PlayerIdleState idleState = new PlayerIdleState();
    [HideInInspector] public PlayerWalkState walkState = new PlayerWalkState();
    [HideInInspector] public PlayerSneakState sneakState = new PlayerSneakState();
    [HideInInspector] public PlayerRunState runState = new PlayerRunState();
    [HideInInspector] public PlayerJumpState jumpState = new PlayerJumpState();
    [HideInInspector] public PlayerDoubleJumpState doubleJumpState = new PlayerDoubleJumpState();

    [HideInInspector] public Vector2 movement;
    public Vector2 lookInput;
    public Transform playerCamera;
    public float mouseSensitivity = 30f;
    public float xRotation = 0f;

    public float default_speed = 1;
    public float run_speed = 5;
    public float jump_force = 1.0f;
    public bool IsSneaking = false;
    public bool IsRunning = false;
    public bool IsJumping = false;
    CharacterController controller;
    public Vector3 velocity;
    public float gravity = -15f;
    public bool isGrounded;

    private bool hasDoubleJumped = false;  // Added to track double jump status
    private float lastJumpTime = -100f;  // Timestamp for last jump (added to the PlayerStateManager)

    public float doubleJumpWindow = 0.3f; 

    // Bullet-related variables
    public Transform bulletSpawner;
    public GameObject bulletPrefab;
    private bool isFiring = false;
    private float fireRate = 0.1f;
    private float lastFireTime = 0f;

    public float fallThreshold = -10f;
    public int playerHealth = 100;

    // To track the previous state
    private PlayerBaseState previousState;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchState(idleState);
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            // Check movement to restore the correct state after landing
            if (movement.magnitude > 0.1f)
            {
                if (IsRunning)
                    SwitchState(runState);
                else
                    SwitchState(walkState);
            }
            else
            {
                SwitchState(idleState);
            }
        }

        currentState.UpdateState(this);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleMouseLook();
        HandleShooting();

        if (transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    void OnMove(InputValue moveVal)
    {
        movement = moveVal.Get<Vector2>();
    }

    void OnLook(InputValue lookVal)
    {
        lookInput = lookVal.Get<Vector2>();
    }

    void OnSprint(InputValue sprintVal)
    {
        IsRunning = sprintVal.isPressed;
        if (IsRunning && !IsSneaking)
        {
            SwitchState(runState);
        }
        else if (!IsRunning && !IsSneaking)
        {
            SwitchState(walkState);
        }
    }

    void OnSneak(InputValue sneakVal)
    {
        IsSneaking = sneakVal.isPressed;
        if (IsSneaking && !IsRunning)
        {
            SwitchState(sneakState);
        }
        else if (!IsSneaking && !IsRunning)
        {
            SwitchState(walkState);
        }
    }

   void OnJump()
    {
        if (isGrounded)
        {
            // Perform the normal jump
            JumpPlayer();
            lastJumpTime = Time.time; // Record jump time
            hasDoubleJumped = false;   // Reset double jump flag
        }
        else
        {
            // Double jump logic
            if (Time.time - lastJumpTime <= doubleJumpWindow && !hasDoubleJumped)
            {
                SwitchState(doubleJumpState);  // Switch to double jump state
                doubleJumpState.RecordJumpTime(); // Record jump time for double jump
            }
        }
    }


    void HandleShooting()
    {
        if (Mouse.current.leftButton.isPressed && Time.time > lastFireTime + fireRate)
        {
            FireBullet();
            lastFireTime = Time.time;
        }
    }

    void FireBullet()
    {
        if (bulletPrefab == null || bulletSpawner == null)
        {
            Debug.LogError("❌ Bullet Prefab or Spawner is missing.");
            return;
        }

        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawner.transform.position, bulletSpawner.transform.rotation);
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = bulletSpawner.transform.forward * 20f;
        }

        Destroy(newBullet, 5f);
    }

    public void MovePlayer(float speed)
    {
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * movement.y + right * movement.x;
        controller.Move(moveDirection * Time.deltaTime * speed);
    }

    public void SwitchState(PlayerBaseState newState)
    {
        currentState?.ExitState(this); // Exit the current state
        currentState = newState;
        currentState.EnterState(this); // Enter the new state
    }

    private void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void JumpPlayer()
{
    if (isGrounded)
    {
        velocity.y = Mathf.Sqrt(jump_force * -2f * gravity); // Regular jump
    }
}


    void Die()
    {
        Debug.Log("Player has fallen off the platform!");
        SceneManager.LoadScene("YOU WIN");
    }
}
