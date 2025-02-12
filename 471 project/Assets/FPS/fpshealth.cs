using UnityEngine;
using UnityEngine.SceneManagement;

public class fpshealth : MonoBehaviour
{
    public int health = 100;

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
        SceneManager.LoadScene("FPSGAMEOVER");
    }
}
