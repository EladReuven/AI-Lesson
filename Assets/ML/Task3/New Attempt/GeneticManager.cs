using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;


namespace FuzzyLogicAndGeneticAlgorithm
{
    public class GeneticManager : MonoBehaviour
    {
        public static GeneticManager instance;

        [SerializeField] float timeScale = 1f;
        [Space]
        [Header("Scene Componenets")]
        [SerializeField] MeshFilter groundPlane;
        [SerializeField] PlayerAgent playerPrefab;
        [SerializeField] EnemyForFuzzyGenetic enemyPrefab;
        [SerializeField] HealthPackSpawner healthPackPrefabs;

        [Header("Scene Variables")]
        [SerializeField] int playerPopulation = 5;
        [SerializeField] int enemyPopulation = 5;
        [SerializeField] float timePerGeneration = 30f;
        [SerializeField] int amountOfGenerations = 10;
        [SerializeField] int amountOfHealthPacks = 2;

        [Header("Evolution Variables")]
        [SerializeField][Range(0f, 1f)] float topPercentOfPlayers = 0.3f;
        [SerializeField] float mutationRate = 0.05f;
        [SerializeField] float mutationAmount = 0.1f;

        [Space]
        public bool started = false;
        [SerializeField] List<PlayerAgent> players = new();
        [SerializeField] List<EnemyForFuzzyGenetic> enemies = new();
        List<HealthPackSpawner> healthPacks = new();

        Dictionary<float, float[]> fitnessAndWeights = new();

        public float currentTimer = 0f;
        int currentGeneration = 1;
        public List<EnemyForFuzzyGenetic> Enemies => enemies;
        public List<HealthPackSpawner> HealthPacks => healthPacks;

        [ContextMenu("SetNewTimeScale")]
        public void SetTimeScale()
        {
            Time.timeScale = timeScale;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            InitializeScene();
        }

        private void Update()
        {
            if (!started)
                return;

            currentTimer += Time.deltaTime;
            if (!ShouldEvolve())
                return;
            MakeNewGeneration();
        }

        private void MakeNewGeneration()
        {
            //reset timer, add +1 to generation
            currentTimer = 0f;
            currentGeneration++;
            //kill all everything
            PlayerAgent[] playerArray = players.ToArray();
            foreach (var p in playerArray)
            {
                if (p.gameObject.activeInHierarchy)
                {
                    p.KillPlayer();
                }
            }


            if (currentGeneration < amountOfGenerations)
            {
                //sort players based on fitness
                players.Sort((agent1, agent2) => agent2.FitnessStats.totalFitness.CompareTo(agent1.FitnessStats.totalFitness));
                print("starting new generation: " + currentGeneration);
                //mutate stuff
                //collect weights from top players
                List<float[]> topWeights = new();
                int amountOfTopAgents = Mathf.FloorToInt((float)players.Count * topPercentOfPlayers);
                print("Top Weights: ");
                for (int i = 0; i <= amountOfTopAgents; i++)
                {
                    topWeights.Add(players[i].Weights);
                    print(players[i].Weights[0] + ", " + players[i].Weights[1] + ", " + players[i].Weights[2]);
                }
                

                //Destroy old players and replace with new players.
                foreach (var p in players)
                {
                    Destroy(p.gameObject);
                }
                players.Clear();


                //List<PlayerAgent> newPlayers = new();

                
                //respawn everything except for health packs
                //create new players based on evolved weights
                CreateEvolvedPlayers(topWeights);
                
                //reset enemies
                EnemyForFuzzyGenetic[] enemiesArray = enemies.ToArray();
                foreach (var e in enemiesArray)
                {
                    enemies.Remove(e);
                    Destroy(e.gameObject);
                    CreateEnemy();
                }
            }
            else
            {
                started = false;
                print("end of simulation");

            }
        }

        void CreateEvolvedPlayers(List<float[]> topWeights)
        {
            //List<PlayerAgent> EvolvedPlayers = new();
            for (int i = 0; i < playerPopulation; i++)
            {
                //create new player
                PlayerAgent newAgent = CreatePlayer();

                //set weights from top weights list
                int randTopWeight1 = Random.Range(0, topWeights.Count);
                int randTopWeight2 = Random.Range(0, topWeights.Count);
                newAgent.CrossoverWeights(topWeights[randTopWeight1], topWeights[randTopWeight2]);
                
                //mutate weights based on chance
                newAgent.SetWeights(MutateWeights(newAgent.Weights));

            }
        }

        PlayerAgent SelectFitAgent()
        {
            //select random parent from top of list
            return players[Random.Range(0, Mathf.CeilToInt((float)players.Count * topPercentOfPlayers))];
        }

        public float[] MutateWeights(float[] weightsToMutate)
        {
            float[] newWeights = weightsToMutate;
            for (int i = 0; i < weightsToMutate.Length; i++)
            {
                // Check if this weight should mutate based on the mutation rate
                if (Random.value <= mutationRate)
                {
                    // Apply Gaussian mutation
                    float mutation = Random.Range(-mutationAmount, mutationAmount);
                    newWeights[i] += mutation;

                    // Clamp the mutated weight to keep it within a valid range (0 to 1)
                    newWeights[i] = Mathf.Clamp(newWeights[i], 0f, 1f);
                }
            }
            return newWeights;
        }

        void InitializeScene()
        {

            //spawn hp packs
            for (int i = 0; i < amountOfHealthPacks; i++)
            {
                HealthPackSpawner healthPackPF = Instantiate(healthPackPrefabs, GetRandomLocationOnGround(healthPackPrefabs.transform.position.y), Quaternion.identity, transform);
                healthPacks.Add(healthPackPF);
            }
            //spawn enemies
            for (int i = 0; i < enemyPopulation; i++)
            {
                CreateEnemy();
            }
            //spawn players
            for (int i = 0; i < playerPopulation; i++)
            {
                PlayerAgent player = CreatePlayer();
                player.InitializeWeights();
            }
        }

        private PlayerAgent CreatePlayer()
        {
            PlayerAgent player = Instantiate(playerPrefab, GetRandomLocationOnGround(playerPrefab.transform.position.y), Quaternion.identity, transform);
            player.groundPlane = groundPlane;
            players.Add(player);
            return player;
        }
        private PlayerAgent CreatePlayer(PlayerAgent newPlayerAgent)
        {
            PlayerAgent player = Instantiate(newPlayerAgent, GetRandomLocationOnGround(playerPrefab.transform.position.y), Quaternion.identity, transform);
            player.groundPlane = groundPlane;
            return player;
        }

        private void CreateEnemy()
        {
            EnemyForFuzzyGenetic enemy = Instantiate(enemyPrefab, GetRandomLocationOnGround(enemyPrefab.transform.position.y), Quaternion.identity, transform);
            enemy.groundPlane = groundPlane;
            enemies.Add(enemy);
        }

        bool ShouldEvolve()
        {
            if (currentTimer >= timePerGeneration)
                return true;

            foreach (var player in players)
            {
                if (player.gameObject.activeInHierarchy)
                    return false;
            }

            return true;
        }

        private Vector3 GetRandomLocationOnGround(float offGroundOffset)
        {
            Vector3 newlocation = groundPlane.transform.position;
            float minX = newlocation.x - groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float maxX = newlocation.x + groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float minZ = newlocation.z - groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;
            float maxZ = newlocation.z + groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;

            return new Vector3(Random.Range(minX, maxX), offGroundOffset, Random.Range(minZ, maxZ));
        }
    }
}
