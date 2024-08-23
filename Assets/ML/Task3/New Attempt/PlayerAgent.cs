using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FuzzyLogicAndGeneticAlgorithm
{
    public class PlayerAgent : MonoBehaviour
    {
        public MeshFilter groundPlane;

        [Header("Player Stats")]
        [SerializeField] float speed;
        [SerializeField] int maxHealth;
        [SerializeField] int currentHealth;
        [SerializeField] LayerMask healthPackLayer;

        [Header("Fuzzy Logic Affected by Genetic Algorithm")]
        [SerializeField][Range(0f,1f)] float healthPackPriority = 0.5f;
        [SerializeField] float[] weights; // 0 - healthLow Weight, 1 - enemy Near weight , 2 - healthpack near weight
        [SerializeField] FitnessStats fitnessStats;

        EnemyForFuzzyGenetic closestEnemy;
        HealthPackSpawner closestHealthPackSpawner;
        float decisionValue;


        Vector3 targetLocation;
        bool dead = false;
        public bool Dead => dead;
        public FitnessStats FitnessStats => fitnessStats;
        public float[] Weights => weights;

        private void Start()
        {
            InitializeWeights();
            ResetStats();
        }

        private void Update()
        {
            if (!GeneticManager.instance.started)
                return;

            if (dead) return;

            fitnessStats.timeSurvived += Time.deltaTime;
            float healthStatus = EvaluateHealth(currentHealth);
            float enemyProximity = EvaluateEnemyProximity();
            float healthPackProximity = EvaluateHealthPackProximity();

            decisionValue = MakeDecision(healthStatus, enemyProximity, healthPackProximity);
            ApplyDecision(decisionValue);
        }

        

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("HealthPack"))
                return;

            HealthPack healthPack = other.GetComponent<HealthPack>();
            if(healthPack == null) return;

            //how much of the heal amount was used without overflowing.
            float hpBeforeHeal = currentHealth;
            Heal(healthPack.HealAmount);
            float percentHealUsed = (currentHealth - hpBeforeHeal)/healthPack.HealAmount;
            fitnessStats.healthPacksUsed++;
            CalculateHealthpackEfficiency(percentHealUsed);
            Destroy(other.gameObject);

        }

        void CalculateHealthpackEfficiency(float percentOfHealUsed)
        {
            //all heals used/total used

            //total sum + new number 
            fitnessStats.healthPackEfficiency += percentOfHealUsed;
            //divide by amount of healthpacks
            fitnessStats.healthPackEfficiency /= fitnessStats.healthPacksUsed;
        }

        float EvaluateHealth(int health)
        {
            return 1 - ((float)health / maxHealth); // lower hp, higher need for pack
        }

        float EvaluateEnemyProximity()
        {
            float shortestDistance = Mathf.Infinity;
            foreach(var e in GeneticManager.instance.Enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, e.transform.position);
                if(distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    closestEnemy = e;
                }
            }

            return shortestDistance <= 0 ? 1 : 1 / shortestDistance; //the closer the enemy, the higher the number
        }


        float EvaluateHealthPackProximity()
        {
            float shortestDistance = Mathf.Infinity;
            foreach (var hPack in GeneticManager.instance.HealthPacks)
            {
                if (!hPack.HealthPackAvailable) continue;
                float distanceToHPack = Vector3.Distance(transform.position, hPack.transform.position);
                if (distanceToHPack < shortestDistance)
                {
                    shortestDistance = distanceToHPack;
                    closestHealthPackSpawner = hPack;
                }
            }

            if (shortestDistance == Mathf.Infinity) return 0;

            return 1 / shortestDistance; //the closer the hp pack, the higher the number
        }

        

        void InitializeWeights()
        {
            weights = new float[3]; // Example: 3 weights
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Random.Range(0f, 1f);
            }
        }

        /// <summary>
        /// set new weights based by crossing over 2 parent agents' weights.
        /// </summary>
        /// <param name="partner1"></param>
        /// <param name="partner2"></param>
        public void CrossoverWeights(PlayerAgent partner1, PlayerAgent partner2)
        {
            int crossoverPoint = Random.Range(0, weights.Length);

            for(int i = 0; i < weights.Length; i++)
            {
                weights[i] = i < crossoverPoint ? partner1.Weights[i] : partner2.Weights[i];
            }
        }


        float MakeDecision(float healthStatus, float enemyProximity, float healthPackProximity)
        {
            // Weights that determine the influence of each factor
            float healthWeight = weights[0];
            float enemyWeight = weights[1];
            float packWeight = weights[2];

            // Combine the fuzzy inputs with their respective weights
            float newValue = (healthWeight * healthStatus) + (enemyWeight * enemyProximity) + (packWeight * healthPackProximity);

            return newValue;
        }

        void ApplyDecision(float decisionValue)
        {
            if (closestHealthPackSpawner != null && decisionValue > healthPackPriority && closestHealthPackSpawner.HealthPackAvailable)
            {
                //go to health pack
                transform.position = Vector3.MoveTowards(transform.position, closestHealthPackSpawner.transform.position, speed * Time.deltaTime);
            }
            else
            {
                Wander();
            }
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                dead = true;
                KillPlayer();
            }
        }

        public void Wander()
        {
            if (Vector3.Distance(transform.position, targetLocation) <= 0.1f)
            {
                //pick new random location
                GetRandomTargetLocation();
            }

            transform.position = Vector3.MoveTowards(transform.position, targetLocation, speed * Time.deltaTime);
        }

        private void GetRandomTargetLocation()
        {
            Vector3 newlocation = groundPlane.transform.position;
            float minX = newlocation.x - groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float maxX = newlocation.x + groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float minZ = newlocation.z - groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;
            float maxZ = newlocation.z + groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;

            targetLocation = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        }

        public void KillPlayer()
        {
            fitnessStats.CalculateTotalFitness();
            //send player fitness and weights to geneticManager
            //GeneticManager.instance.OnPlayerDeath.Invoke(fitnessStats.totalFitness, weights);
            
            gameObject.SetActive(false);
        }

        public void ResetStats()
        {
            currentHealth = maxHealth;
            fitnessStats.InitializeFitness();
            GetRandomTargetLocation();
        }

    }

    [Serializable]
    public struct FitnessStats
    {
        public float totalFitness;

        public float timeSurvived;
        public int healthPacksUsed; //amount of packs picked up
        public float healthPackEfficiency; //how much of the heal was converted to hp without overflowing

        public void InitializeFitness()
        {
            totalFitness = 0f;
            timeSurvived = 0f;
            healthPacksUsed = 0;
            healthPackEfficiency = 0f;
        }

        public void CalculateTotalFitness()
        {
            totalFitness = 0f;

            totalFitness += timeSurvived;
            totalFitness += healthPacksUsed * 10f;
            totalFitness += healthPackEfficiency * 100f;
        }
    }
}
