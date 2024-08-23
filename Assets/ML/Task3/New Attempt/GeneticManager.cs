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
                EvolvePlayers();


                //respawn everything except for health packs
                //reset Players
                foreach (var p in players)
                {
                    p.transform.position = GetRandomLocationOnGround(p.transform.position.y);
                    p.ResetStats();
                    p.gameObject.SetActive(true);
                }
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

        void EvolvePlayers()
        {
            List<PlayerAgent> newPlayers = new();
            for (int i = 0; i < playerPopulation; i++)
            {
                //get 2 parents
                PlayerAgent parent1 = SelectParent();
                PlayerAgent parent2 = SelectParent();

                //set the weights of player based on parents crossover
                players[i].CrossoverWeights(parent1, parent2);
                MutatePlayer(players[i]);
            }
        }

        PlayerAgent SelectParent()
        {
            //select random parent from top of list
            return players[Random.Range(0, Mathf.CeilToInt((float)players.Count * topPercentOfPlayers))];
        }

        public void MutatePlayer(PlayerAgent agent)
        {
            for (int i = 0; i < agent.Weights.Length; i++)
            {
                // Check if this weight should mutate based on the mutation rate
                if (Random.value <= mutationRate)
                {
                    // Apply Gaussian mutation
                    float mutation = Random.Range(-mutationAmount, mutationAmount);
                    agent.Weights[i] += mutation;

                    // Clamp the mutated weight to keep it within a valid range (0 to 1)
                    agent.Weights[i] = Mathf.Clamp(agent.Weights[i], 0f, 1f);
                }
            }
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
                PlayerAgent player = Instantiate(playerPrefab, GetRandomLocationOnGround(playerPrefab.transform.position.y), Quaternion.identity, transform);
                player.groundPlane = groundPlane;
                players.Add(player);
            }
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
