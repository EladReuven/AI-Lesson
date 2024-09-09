using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    public DragonController dragonPrefab;
    public GameObject obstaclePrefab;
    public Transform targetLocation;
    public float spawnXYZ = 10f;

    public int amountOfDragons = 5;
    public int amountOfObstacles = 20;

    Vector3 dragonSpawnAreaSize;
    Vector3 obstacleSpawnAreaSize;
    Vector3 obstacleSpawnAreaLocation;




    private void Awake()
    {
        Instance = this;
        dragonSpawnAreaSize = new Vector3(spawnXYZ, spawnXYZ, spawnXYZ);
        float obstacleAreaSize = (Vector3.Distance(transform.position, targetLocation.position) - spawnXYZ / 2f) * 0.9f;
        obstacleSpawnAreaSize = new Vector3(obstacleAreaSize, obstacleAreaSize, obstacleAreaSize);
        obstacleSpawnAreaLocation = Vector3.Lerp(transform.position, targetLocation.position, 0.5f);
    }

    private void Start()
    {
        //spawn dragons
        SpawnInitialDragons();
    }

    private void SpawnInitialDragons()
    {
        for (int i = 0; i < amountOfDragons; i++)
        {
            DragonController newDragon = CreateDragon();
            DragonManager.Instance.dragons.Add(newDragon);
        }
    }

    private DragonController CreateDragon()
    {
        Vector3 dragonSpawn = GetRandomSpawnLocation();
        DragonController newDragon = Instantiate(dragonPrefab, dragonSpawn, Quaternion.identity, transform);
        newDragon.SetTarget(targetLocation);

        return newDragon;
    }

    public DragonController CreateDragon(NeuralNetWithCopyMono.Layer[] layers)
    {
        DragonController newDragon = CreateDragon();

        newDragon.NN.layers = layers;
        newDragon.MutateDragon();
        return newDragon;
        
    }

    public void SpawnNewGeneration(List<NeuralNetWithCopyMono.Layer[]> topPercentLayers)
    {
        for (int i = 0; i < amountOfDragons; i++)
        {
            //pick random layers from list
            int randomLayersIndex = Random.Range(0, topPercentLayers.Count);

            
            DragonController newMutatedDragon = CreateDragon(topPercentLayers[randomLayersIndex]);
            DragonManager.Instance.dragons.Add(newMutatedDragon);
        }
    }


    private Vector3 GetRandomSpawnLocation()
    {
        Vector3 dragonSpawn = new Vector3(Random.Range(-(spawnXYZ / 2f), spawnXYZ / 2f),
                                                  Random.Range(-(spawnXYZ / 2f), spawnXYZ / 2f),
                                                  Random.Range(-(spawnXYZ / 2f), spawnXYZ / 2f));

        dragonSpawn += transform.position;
        return dragonSpawn;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnXYZ, spawnXYZ, spawnXYZ));

        Gizmos.color = Color.red;
        float obstacleAreaSize = (Vector3.Distance(transform.position, targetLocation.position) - spawnXYZ / 2f) * 0.9f;
        Gizmos.DrawWireCube(Vector3.Lerp(transform.position, targetLocation.position, 0.5f), new Vector3(obstacleAreaSize, obstacleAreaSize, obstacleAreaSize));
    }
}
