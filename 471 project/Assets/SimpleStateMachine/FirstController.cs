using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FIRSTCONTROLLER : MonoBehaviour
{
    Vector2 movement;
    Vector2 mouseMovement;
    float cameraUpRotation = 0;
    CharacterController controller;

    [SerializeField] float speed = 18f;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float sprintMultiplier = 3f;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject BulletSpawner;
    [SerializeField] GameObject Bullet;

    public float fallThreshold = -10f;
    Vector3 velocity;
    bool isGrounded;
    public int playerHealth = 100;

    private bool isSprinting = false;
    private bool isFiring = false;
    private float fireRate = 0.1f;  // Interval between bullets
    private float lastFireTime = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Look around
        float lookX = mouseMovement.x * Time.deltaTime * mouseSensitivity;
        float lookY = mouseMovement.y * Time.deltaTime * mouseSensitivity;

        cameraUpRotation -= lookY;
        cameraUpRotation = Mathf.Clamp(cameraUpRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(cameraUpRotation, 0, 0);
        transform.Rotate(Vector3.up * lookX);

        // Movement
        float moveX = movement.x;
        float moveZ = movement.y;

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        Vector3 actual_movement = (transform.forward * moveZ) + (transform.right * moveX);
        controller.Move(actual_movement * Time.deltaTime * speed);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (transform.position.y < fallThreshold)
        {
            Die();
        }

        // Check if left-click is being held down
        if (Input.GetMouseButton(0)) 
        {
            if (!isFiring) 
            {
                isFiring = true; 
                FireBullet();  
            }

            if (Time.time > lastFireTime + fireRate)  
            {
                FireBullet();
                lastFireTime = Time.time;
            }
        }
        else
        {
            isFiring = false;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    void OnMove(InputValue moveVal)
    {
        movement = moveVal.Get<Vector2>();
    }

    void OnLook(InputValue lookVal)
    {
        mouseMovement = lookVal.Get<Vector2>();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void FireBullet()
{
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    RaycastHit hit;

    Vector3 shootDirection;
    if (Physics.Raycast(ray, out hit))
    {
        shootDirection = (hit.point - BulletSpawner.transform.position).normalized;
    }
    else
    {
        shootDirection = cam.transform.forward;  // Shoot straight ahead if no hit
    }

    GameObject newBullet = Instantiate(Bullet, BulletSpawner.transform.position, Quaternion.identity);
    Rigidbody rb = newBullet.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.linearVelocity = shootDirection * 20f;  // Directly set the velocity to the shoot direction
    }
}


    void Die()
    {
        Debug.Log("Player has fallen off the platform!");
        SceneManager.LoadScene("YOU WIN");
    }
}
