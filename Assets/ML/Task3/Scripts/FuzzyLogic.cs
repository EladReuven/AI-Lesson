using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FuzzyLogicAndGeneticAlgorithm
{
    public class FuzzyLogic : MonoBehaviour
    {
        public float health;
        public float enemyDistance;

        // Parameters influenced by genetic algorithm
        public float healthLowThreshold = 30f;
        public float enemyNearThreshold = 5f;
        public float healthPackPriority = 0.5f;

        void Update()
        {
            float healthStatus = EvaluateHealth(health);
            float enemyProximity = EvaluateEnemyDistance(enemyDistance);

            float decision = MakeDecision(healthStatus, enemyProximity);
            ApplyDecision(decision);
        }

        float EvaluateHealth(float health)
        {
            if (health < healthLowThreshold) return 1; // Low health
            if (health < healthLowThreshold * 2.33f) return 0.5f; // Medium health
            return 0; // High health
        }

        float EvaluateEnemyDistance(float distance)
        {
            if (distance < enemyNearThreshold) return 1; // Enemy near
            if (distance < enemyNearThreshold * 4f) return 0.5f; // Enemy medium distance
            return 0; // Enemy far
        }

        float MakeDecision(float healthStatus, float enemyProximity)
        {
            // Adjusted decision-making using the healthPackPriority influenced by the genetic algorithm
            return Mathf.Lerp(healthStatus, enemyProximity, healthPackPriority);
        }

        void ApplyDecision(float decision)
        {
            if (decision > 0.5f)
            {
                // High priority to find a health pack
                FindHealthPack();
            }
            else if (decision > 0.25f)
            {
                // Medium priority - maybe attack the enemy or move strategically
                EngageEnemy();
            }
            else
            {
                // Low priority - patrol or do nothing
                Patrol();
            }
        }

        void FindHealthPack()
        {
            GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");
            if (healthPacks.Length == 0) return;

            GameObject nearestHealthPack = null;
            float nearestDistance = Mathf.Infinity;

            foreach (GameObject healthPack in healthPacks)
            {
                float distanceToHealthPack = Vector3.Distance(transform.position, healthPack.transform.position);
                if (distanceToHealthPack < nearestDistance)
                {
                    nearestDistance = distanceToHealthPack;
                    nearestHealthPack = healthPack;
                }
            }

            if (nearestHealthPack != null)
            {
                // Move towards the health pack
                GetComponent<NavMeshAgent>().SetDestination(nearestHealthPack.transform.position);
                Debug.Log("Moving towards health pack");
            }
        }

        void EngageEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;

            GameObject nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < nearestDistance)
                {
                    nearestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                // Engage with the enemy (e.g., move towards or attack)
                GetComponent<NavMeshAgent>().SetDestination(nearestEnemy.transform.position);
                Debug.Log("Engaging enemy");
            }
        }

        void Patrol()
        {
            // Example patrol behavior: move to a random point within a certain range
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10f, 1);
            Vector3 finalPosition = hit.position;

            GetComponent<NavMeshAgent>().SetDestination(finalPosition);
            Debug.Log("Patrolling");
        }
    }
}
