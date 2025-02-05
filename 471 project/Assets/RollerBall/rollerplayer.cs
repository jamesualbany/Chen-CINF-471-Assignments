using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class rollerplayer : MonoBehaviour
{
    public float fallThreshold = -10f;  // Set this to a value below the platform level where the player will die
    public int playerHealth = 100;

    Vector2 m;
    Rigidbody rb;
    public float jumpForce = 5f;
    private bool isGrounded = true; // Check if the player is on the ground

    void Start()
    {
        m = new Vector2(0, 0);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x_dir = m.x;
        float z_dir = m.y;
        Vector3 actual_movement = new Vector3(x_dir, 0, z_dir);
        print(actual_movement);

        rb.AddForce(actual_movement);

        if (transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    void OnMove(InputValue movement)
    {
        m = movement.Get<Vector2>();
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Prevents double jumping
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void Die()
    {
        Debug.Log("Player has fallen off the platform! Game Over.");
        // Load GameOver scene
        SceneManager.LoadScene("GameOverScene");
    }
}
