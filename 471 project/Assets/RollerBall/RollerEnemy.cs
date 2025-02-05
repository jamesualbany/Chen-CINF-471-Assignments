using UnityEngine;
using System.Collections;

public class RollerEnemy : MonoBehaviour
{
    public float speed = 3f;         // Normal movement speed
    public float chargeSpeed = 2f;   // Charge speed
    public float chargeDistance = 5f; // Distance at which enemy charges
    public float sightRange = 8f;   // Range for detecting player
    public float attackRange = 1.5f; // Range for attacking
    public int enemyHealth = 100;    // Enemy health
    private float damageCooldown = 3f; // 3-second cooldown before the player can take damage again

    private Transform player;
    private Rigidbody rb;
    private bool isCharging = false;
    private bool canDamagePlayer = true; // Flag to check if the enemy can damage the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // If no player found, do nothing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chargeDistance)
        {
            ChargeAtPlayer();
        }
        else if (distanceToPlayer <= sightRange)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        isCharging = false; // Stop charging
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
    }

    void ChargeAtPlayer()
    {
        if (!isCharging)
        {
            isCharging = true;
            Vector3 chargeDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector3(chargeDirection.x * chargeSpeed, rb.linearVelocity.y, chargeDirection.z * chargeSpeed);
        }
    }

    void AttackPlayer()
    {
        if (canDamagePlayer)
        {
            Debug.Log("Player taken 20 damage!");
            // Damage the player on contact
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].point.y > transform.position.y + 0.5f) // Check if player is above
            {
                // Player jumps on enemy → Kill enemy
                Die();
            }
            else
            {
                // Normal contact → Damage player
                AttackPlayer();
            }
        }
    }

    // Coroutine to create a cooldown before player can take damage again
    IEnumerator DamageCooldown()
    {
        canDamagePlayer = false;  // Prevent the player from taking damage
        yield return new WaitForSeconds(damageCooldown); // Wait for the cooldown duration (3 seconds)
        canDamagePlayer = true;  // Allow the player to take damage again
    }
}
