using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    public float chargeSpeed = 10f;
    public float chargeDistance = 5f;
    public float sightRange = 20f;
    public float attackRange = 1.5f;
    public int enemyHealth = 100;
    private float damageCooldown = 3f;

    private Transform player;
    private Rigidbody rb;
    private bool isCharging = false;
    private bool canDamagePlayer = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on the enemy.");
        }
    }

    void Update()
    {
        if (enemyHealth <= 0)
        {
            Die();
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chargeDistance)
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
        if (!isCharging)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
        }
    }

    void ChargeAtPlayer()
    {
        if (!isCharging)
        {
            isCharging = true;
            Vector3 chargeDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector3(chargeDirection.x * chargeSpeed, rb.linearVelocity.y, chargeDirection.z * chargeSpeed);

            StartCoroutine(ResetCharge());
        }
    }

    IEnumerator ResetCharge()
    {
        yield return new WaitForSeconds(1.5f);
        isCharging = false;
    }

    void AttackPlayer(GameObject playerObject)
    {
        if (canDamagePlayer)
        {
            fpshealth playerHealth = playerObject.GetComponent<fpshealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
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
            if (collision.contacts[0].point.y > transform.position.y + 0.5f)
            {
                Die();
            }
            else
            {
                AttackPlayer(collision.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() != null)
        {
            enemyHealth -= 10;
            Destroy(other.gameObject);
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamagePlayer = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamagePlayer = true;
    }
}

