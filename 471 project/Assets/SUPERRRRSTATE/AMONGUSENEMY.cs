using UnityEngine;
using System.Collections;

public class AMONGUSENEMY : MonoBehaviour
{
    private enum State
    {
        Pace,
        Follow,
        Attack  // New state for when the enemy attacks
    }

    [SerializeField]
    GameObject[] route;
    GameObject target;
    int routeIndex = 0;

    [SerializeField]
    float speed = 3.0f;
    public int enemyHealth = 100;

    private State currentState = State.Pace;
    private float attackRange = 2.0f;  // Distance at which the enemy will attack the player
    private float attackCooldown = 2.0f;  // Time between attacks
    private float lastAttackTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Pace:
                OnPace();
                break;
            case State.Follow:
                OnFollow();
                break;
            case State.Attack:
                OnAttack();
                break;
        }

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void OnPace()
    {
        // The enemy paces through the route
        target = route[routeIndex];
        MoveTo(target);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            routeIndex += 1;
            if (routeIndex >= route.Length) routeIndex = 0;
        }

        // Switch to Follow state if the player is detected
        GameObject obstacle = CheckForward();
        if (obstacle != null && obstacle.CompareTag("Player"))
        {
            target = obstacle;
            currentState = State.Follow;
        }
    }

    void OnFollow()
{
    if (target == null) return;

    // Check if the player is sneaking, if they are, stop following them
    PlayerStateManager playerState = target.GetComponent<PlayerStateManager>();
    if (playerState != null && playerState.IsSneaking)
    {
        currentState = State.Pace; // Switch back to pacing if sneaking
        return;
    }

    // Move towards the player and rotate smoothly
    MoveTo(target);

    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

    // Switch to Attack if within attack range
    if (distanceToTarget < attackRange)
    {
        StartCoroutine(StartAttack());
    }

    // If the player is not detected, switch back to Pace
    GameObject obstacle = CheckForward();
    if (obstacle != null && obstacle.CompareTag("Player"))
    {
        target = obstacle;
        currentState = State.Follow;
    }
}


    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(0.2f);  // Small delay to avoid flickering
        currentState = State.Attack;
    }

    void OnAttack()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget >= attackRange + 1.0f)  // Add buffer zone before switching back
        {
            currentState = State.Follow;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            fpshealth playerHealth = target.GetComponent<fpshealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                lastAttackTime = Time.time;
                Debug.Log("Enemy attacked the player!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that hit the enemy is a bullet
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(10);
            Destroy(other.gameObject);  // Destroy the bullet after impact
            Debug.Log("Enemy hit by a bullet!");
            return;  // Exit to avoid unnecessary checks
        }

        // If a player enters the trigger, switch to follow mode
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
            currentState = State.Follow;
        }
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        Debug.Log("Enemy took damage!");

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    // Move the enemy smoothly towards the target and rotate towards it
    void MoveTo(GameObject t)
    {
        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, t.transform.position, speed * Time.deltaTime);

        // Smooth rotation towards the target
        Vector3 directionToTarget = t.transform.position - transform.position;
        if (directionToTarget.sqrMagnitude > 0.01f)  // Avoid unnecessary rotation when very close
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
    }

   GameObject CheckForward()
{
    RaycastHit hit;
    Debug.DrawRay(transform.position, transform.forward * 10, Color.green);

    if (Physics.Raycast(transform.position, transform.forward, out hit, 10))
    {
        PlayerStateManager player = hit.transform.gameObject.GetComponent<PlayerStateManager>();
        
        // If the player is found and is sneaking, ignore them and do not follow
        if (player != null && !player.IsSneaking)  // Only follow if the player is not sneaking
        {
            return hit.transform.gameObject;
        }
    }

    return null;  // Return null if no player is detected or the player is sneaking
}

}
