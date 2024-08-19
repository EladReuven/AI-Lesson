using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyLogicAndGeneticAlgorithm
{
    public class HealthPackSpawner : MonoBehaviour
    {
        [SerializeField] GameObject healthPackPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float spawnInterval = 10f;

        GameObject spawnedHealthPack;
        private float spawnTimer;
        
        public bool HealthPackAvailable => spawnedHealthPack != null;

        void Start()
        {
            spawnTimer = spawnInterval;
        }

        void Update()
        {
            if (spawnedHealthPack != null)
                return;

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnHealthPack();
                spawnTimer = spawnInterval;
            }
        }

        void SpawnHealthPack()
        {
            spawnedHealthPack = Instantiate(healthPackPrefab, spawnPoint);
        }
    }
}
