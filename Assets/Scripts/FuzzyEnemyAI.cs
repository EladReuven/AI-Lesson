using UnityEngine;

public class FuzzyEnemyAI : MonoBehaviour
{
    public Transform player;
    public float enemyHealth = 100f;
    public float attackDistance = 10f;
    public float retreatDistance = 20f;
    public float speed = 5f;
    public float attackDamage = 10f;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        float playerHealth = player.GetComponent<PlayerHealth>().currentHealth;

        float actionProbability = CalculateFuzzyProbability(distanceToPlayer, playerHealth, enemyHealth);
        
        if (actionProbability > 0.7f)
        {
            Attack();
        }
        else if (actionProbability > 0.4f)
        {
            Defend();
        }
        else if (actionProbability > 0.1f)
        {
            Retreat();
        }
        else
        {
            Idle();
        }
    }

    float CalculateFuzzyProbability(float distance, float playerHealth, float enemyHealth)
    {
        float distanceFactor = Mathf.Clamp01((attackDistance - distance) / attackDistance);
        float retreatFactor = Mathf.Clamp01((retreatDistance - distance) / retreatDistance);
        float healthFactor = Mathf.Clamp01(enemyHealth / 100.0f);
        float playerHealthFactor = 1.0f - Mathf.Clamp01(playerHealth / 100.0f);

        // Example fuzzy inference combining multiple factors
        return (distanceFactor * healthFactor) + (retreatFactor * playerHealthFactor);
    }

    void Attack()
    {
        Debug.Log("Attacking the player!");
        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // Check if close enough to deal damage
        if (Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            // Deal damage to the player
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    void Defend()
    {
        Debug.Log("Defending!");
        // Defend logic (e.g., reduce damage taken or increase armor)
        // Example: Increase enemy's health to simulate defense
        enemyHealth += 0.1f; // Very basic example, usually you'd adjust defensive stats
    }

    void Retreat()
    {
        Debug.Log("Retreating!");
        // Move away from the player
        Vector3 direction = (transform.position - player.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void Idle()
    {
        Debug.Log("Idling...");
        // Idle logic (e.g., stand still, patrol, or look around)
        // Example: Patrolling in a small radius
        Patrol();
    }

    void Patrol()
    {
        // Simple patrol logic
        float patrolRadius = 2f;
        Vector3 patrolPosition = new Vector3(
            Mathf.PingPong(Time.time, patrolRadius) - patrolRadius / 2,
            transform.position.y,
            Mathf.PingPong(Time.time, patrolRadius) - patrolRadius / 2
        );
        transform.position = patrolPosition;
    }
}

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 100f;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Handle player death (e.g., respawn, game over screen)
    }
}
