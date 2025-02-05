using UnityEngine;
using UnityEngine.SceneManagement;  // Needed to load scenes

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;  // Player health

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took damage! Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Load GameOver scene
        SceneManager.LoadScene("GameOverScene");
    }
}
