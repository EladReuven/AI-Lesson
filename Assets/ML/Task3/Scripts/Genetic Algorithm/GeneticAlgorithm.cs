using UnityEngine;

namespace FuzzyLogicAndGeneticAlgorithm
{
    public class GeneticAlgorithm : MonoBehaviour
    {
        public GeneticAgent[] agents;
        public int populationSize = 10;
        public float mutationRate = 0.01f;
        public int numberOfGenerations = 10;
        public GameObject agentPrefab; // Prefab for your GeneticAgent

        private int currentGeneration = 0;

        void Start()
        {
            InitializePopulation();
        }

        void InitializePopulation()
        {
            agents = new GeneticAgent[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                // Instantiate agents and enable genetic algorithm for each
                agents[i] = Instantiate(agentPrefab).GetComponent<GeneticAgent>();
                agents[i].geneticEnabled = true;
            }
        }

        void EvaluateFitness()
        {
            foreach (var agent in agents)
            {
                agent.EvaluateFitness();
            }
        }

        GeneticAgent SelectParent()
        {
            // Example: Roulette wheel selection based on fitness
            float totalFitness = 0f;
            foreach (var agent in agents)
            {
                totalFitness += agent.fitness;
            }

            float randomPoint = Random.value * totalFitness;
            float cumulativeFitness = 0f;

            foreach (var agent in agents)
            {
                cumulativeFitness += agent.fitness;
                if (cumulativeFitness >= randomPoint)
                {
                    return agent;
                }
            }

            return agents[Random.Range(0, agents.Length)]; // Fallback
        }

        void EvolvePopulation()
        {
            GeneticAgent[] newGeneration = new GeneticAgent[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                GeneticAgent parent1 = SelectParent();
                GeneticAgent parent2 = SelectParent();

                GeneticAgent offspring = parent1.Crossover(parent2);
                offspring.Mutate(mutationRate);

                newGeneration[i] = offspring;
            }

            for (int i = 0; i < populationSize; i++)
            {
                Destroy(agents[i].gameObject);
            }

            agents = newGeneration;
        }

        void Update()
        {
            // Condition to trigger evolution, e.g., after a certain time or when agents reach a goal
            if (ShouldEvolve())
            {
                if (currentGeneration < numberOfGenerations)
                {
                    EvaluateFitness();
                    EvolvePopulation();
                    currentGeneration++;
                }
                else
                {
                    Debug.Log("Evolution completed after " + numberOfGenerations + " generations.");
                }
            }
        }

        bool ShouldEvolve()
        {
            // Example condition: evolve when all agents are done (e.g., reach a goal or time limit)
            // You can implement your logic to check if agents have finished their tasks
            // This example checks if all agents are inactive or destroyed
            foreach (var agent in agents)
            {
                if (agent != null && agent.isActiveAndEnabled)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
