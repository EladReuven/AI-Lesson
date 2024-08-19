using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [Space]
        [SerializeField] bool started = false;
        List<PlayerAgent> players = new();
        List<EnemyForFuzzyGenetic> enemies = new();
        List<HealthPackSpawner> healthPacks = new();

        float currentTimer = 0f;
        int currentGeneration = 1;
        public List<EnemyForFuzzyGenetic> Enemies => enemies;
        public List<HealthPackSpawner> HealthPacks => healthPacks;

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

            if (currentTimer < timePerGeneration)
            {
                currentTimer += Time.deltaTime;
                return;
            }

            //reset timer, add +1 to generation
            currentTimer = 0f;
            currentGeneration++;
            //kill all everything
            //mutate stuff
            //respawn everything mutated
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
                EnemyForFuzzyGenetic enemy = Instantiate(enemyPrefab, GetRandomLocationOnGround(enemyPrefab.transform.position.y), Quaternion.identity, transform);
                enemy.groundPlane = groundPlane;
                enemies.Add(enemy);
            }
            //spawn players
            for (int i = 0; i < playerPopulation; i++)
            {
                PlayerAgent player = Instantiate(playerPrefab, GetRandomLocationOnGround(playerPrefab.transform.position.y), Quaternion.identity, transform);
                player.groundPlane = groundPlane;
                players.Add(player);
            }
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
