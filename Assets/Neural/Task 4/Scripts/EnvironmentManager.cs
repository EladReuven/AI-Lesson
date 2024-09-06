using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
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
        dragonSpawnAreaSize = new Vector3 (spawnXYZ, spawnXYZ, spawnXYZ);
        float obstacleAreaSize = (Vector3.Distance(transform.position, targetLocation.position) - spawnXYZ/2f) * 0.9f;
        obstacleSpawnAreaSize = new Vector3(obstacleAreaSize, obstacleAreaSize, obstacleAreaSize);
        obstacleSpawnAreaLocation = Vector3.Lerp(transform.position, targetLocation.position, 0.5f);
    }

    private void Start()
    {
        //spawn dragons
        for(int i = 0; i < amountOfDragons; i++)
        {
            Vector3 dragonSpawn = new Vector3(Random.Range(-(spawnXYZ / 2f), spawnXYZ/2f),
                                          Random.Range(-(spawnXYZ / 2f), spawnXYZ/2f),
                                          Random.Range(-(spawnXYZ / 2f), spawnXYZ/2f));

            dragonSpawn += transform.position;
            DragonController newDragon = Instantiate(dragonPrefab, dragonSpawn, Quaternion.identity, transform);
            newDragon.target = targetLocation;
        }

        //spawn Obstacles
        //for(int i = 0; i < amountOfObstacles; i++)
        //{
            
        //    Vector3 obstacleSpawnLocation = new Vector3(Random.Range(-obstacleSpawnAreaSize.x / 2f, obstacleSpawnAreaSize.x / 2f),
        //                                  Random.Range(-obstacleSpawnAreaSize.y / 2f, obstacleSpawnAreaSize.y / 2f),
        //                                  Random.Range(-obstacleSpawnAreaSize.z / 2f, obstacleSpawnAreaSize.z / 2f));
        //    obstacleSpawnLocation += obstacleSpawnAreaLocation;

        //    Instantiate(obstaclePrefab, obstacleSpawnLocation, Quaternion.identity, transform);
        //}
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
