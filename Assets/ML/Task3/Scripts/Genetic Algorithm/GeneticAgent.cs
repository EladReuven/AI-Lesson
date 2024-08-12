using UnityEngine;

namespace FuzzyLogicAndGeneticAlgorithm
{
    public class GeneticAgent : MonoBehaviour
    {
        public bool geneticEnabled;
        public float[] weights;
        public float fitness;

        // Utility classes
        public HealthPackCollector healthPackCollector;
        public CombatSystem combatSystem;
        public Health health;
        public AgentMovement agentMovement;

        // Fuzzy logic instance
        public FuzzyLogic fuzzyLogic;

        void Start()
        {
            // Initialize utility classes
            healthPackCollector = new HealthPackCollector();
            combatSystem = new CombatSystem();
            health = new Health();
            agentMovement = new AgentMovement();
            fuzzyLogic = new FuzzyLogic();

            if (geneticEnabled)
            {
                InitializeWeights();
                ApplyWeightsToFuzzyLogic(fuzzyLogic);
            }
        }

        void InitializeWeights()
        {
            weights = new float[3]; // Example: 3 weights
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Random.Range(0f, 1f);
            }
        }

        public void ApplyWeightsToFuzzyLogic(FuzzyLogic fuzzyLogic)
        {
            // Assume weights[0] influences healthLowThreshold
            fuzzyLogic.healthLowThreshold = weights[0] * 100f; // Scale to a reasonable value (0 to 100)

            // Assume weights[1] influences enemyNearThreshold
            fuzzyLogic.enemyNearThreshold = weights[1] * 20f; // Scale to a reasonable value (0 to 20)

            // Assume weights[2] influences healthPackPriority (0 to 1)
            fuzzyLogic.healthPackPriority = weights[2];
        }

        public void EvaluateFitness()
        {
            // Initialize fitness score
            fitness = 0f;

            // Factor 1: Survival Time
            float survivalTime = Time.timeSinceLevelLoad;
            fitness += survivalTime * 1f; // Weight of 1 for survival time

            // Factor 2: Health Packs Collected
            fitness += healthPackCollector.healthPacksCollected * 10f; // Higher weight for health packs

            // Factor 3: Enemies Defeated
            fitness += combatSystem.enemiesDefeated * 20f; // Even higher weight for enemies defeated

            // Factor 4: Health Remaining
            fitness += health.currentHealth * 0.5f; // Lower weight for remaining health

            // Factor 5: Distance Covered
            fitness += agentMovement.distanceTraveled * 0.1f; // Small contribution from distance traveled

            Debug.Log("Fitness evaluated: " + fitness);
        }

        public GeneticAgent Crossover(GeneticAgent partner)
        {
            GeneticAgent offspring = Instantiate(this); // Create a new agent as offspring

            // Single-point crossover example
            int crossoverPoint = Random.Range(0, weights.Length);
            for (int i = 0; i < weights.Length; i++)
            {
                offspring.weights[i] = i < crossoverPoint ? this.weights[i] : partner.weights[i];
            }

            return offspring;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                if (Random.value < mutationRate)
                {
                    weights[i] = Random.Range(0f, 1f); // Mutate the weight by assigning a new random value
                    Debug.Log("Mutation occurred at weight " + i);
                }
            }
        }
    }
}
