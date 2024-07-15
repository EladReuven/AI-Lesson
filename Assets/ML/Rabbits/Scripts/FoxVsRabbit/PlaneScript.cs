using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlaneScript : MonoBehaviour
{
    // General stats
    public int frameRate;
    private float xScale;
    private float zScale;
    private float area;

    // Fox stats
    public GameObject foxModel;
    public float foxNumberPerTile;
    public float foxFOV;
    public float foxViewDistance;
    public float foxSpeed;

    // Rabbit stats
    public GameObject rabbitModel;
    public float rabbitNumberPerTile;
    public float rabbitFOV;
    public float rabbitViewDistance;
    public float rabbitSpeed;

    public List<RabbitScript> allLivingRabbits;
    
    // Bush stats
    public GameObject bushModel;
    public float bushNumberPerTile;
    public float bushMaxSize;
    public float timeBetweenBushSpawn;    // Seconds
    private int bushSpawnCounter;

    // Texts
    public TextMeshProUGUI rabbitStatsText;
    public TextMeshProUGUI foxStatsText;

    // Start is called before the first frame update
    void Start()
    {
        // Set the framerate
        Application.targetFrameRate = frameRate;

        // Get the scales and square
        xScale = (GetComponent<MeshRenderer>().bounds.size.x / 2) - 1;
        zScale = (GetComponent<MeshRenderer>().bounds.size.z / 2) - 1;

        // Create the walls
        createWalls(transform.localScale.x, transform.localScale.z);

        // Find how many of the foxes, rabbits and bushes need to be spawned
        area = transform.localScale.x * transform.localScale.z;
        foxNumberPerTile = foxNumberPerTile * area;
        rabbitNumberPerTile = rabbitNumberPerTile * area;
        bushNumberPerTile = bushNumberPerTile * area;

        // Spawn foxes
        for (int i = 0; i < foxNumberPerTile; i++) {
            spawnFox(Random.Range(-xScale, xScale), Random.Range(-zScale, zScale), foxFOV, foxViewDistance, foxSpeed);
        }

        // Spawn rabbits
        for (int i = 0; i < rabbitNumberPerTile; i++) {
            spawnRabbit(Random.Range(-xScale, xScale), Random.Range(-zScale, zScale), rabbitFOV, rabbitViewDistance, rabbitSpeed);
        }

        // Spawn bushed
        for (int i = 0; i < bushNumberPerTile; i++) {
            spawnBush(Random.Range(-xScale, xScale), Random.Range(-zScale, zScale), bushMaxSize);  
        }

        // Set the texts color
        rabbitStatsText.color = Color.blue;
        foxStatsText.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn in new bushes
        bushSpawnCounter++;
        if (bushSpawnCounter > (timeBetweenBushSpawn * frameRate)) {
            bushSpawnCounter = 0;
            spawnBush(Random.Range(-xScale, xScale), Random.Range(-zScale, zScale), bushMaxSize);
        }

        // Update animal stats
        updateAnimalStats(foxStatsText, "Fox");
        updateAnimalStats(rabbitStatsText, "Rabbit");
    }

    public void createWalls(float xLoc, float zLoc) { 
        float xTransformLoc = xLoc * 5;
        float zTransformLoc = zLoc * 5;

        createWall(-xTransformLoc, 0, 0, -90, 1, zLoc);
        createWall(xTransformLoc, 0, 0, 90, 1, zLoc);
        createWall(0, -zTransformLoc, 90, 0, xLoc, 1);
        createWall(0, zTransformLoc, -90, 0, xLoc, 1);
    }

    public void createWall(float xLoc, float zLoc, float xRotation, float zRotation, float xScale, float zScale) {
        // Create a new plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        // Add it's tag
        plane.tag = "Border";

        // Move the planes position
        plane.transform.position = new Vector3(xLoc, 5f, zLoc);

        // Move the planes rotation
        plane.transform.rotation = Quaternion.Euler(new Vector3(xRotation, 0f, zRotation));

        // Change the plans scale
        plane.transform.localScale = new Vector3(xScale, 1f, zScale);

        // Set the color of the walls
        plane.GetComponent<Renderer>().material.color = Color.black;
    }

    public void spawnFox(float xLoc, float zLoc, float foxF, float foxV, float foxS) {
        // Create a new fox
        GameObject fox = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag and layer
        fox.tag = "Fox";
        fox.layer = LayerMask.NameToLayer("Fox");

        // Start the fox at a random position
        fox.transform.position = new Vector3(xLoc, 0.5f, zLoc);

        // Add the script with variables
        fox.AddComponent<FoxScript>().FOV = foxF;
        fox.GetComponent<FoxScript>().viewDistance = foxV;
        fox.GetComponent<FoxScript>().speed = foxS;

        // Add the rigidbody so that it can collide
        fox.AddComponent<Rigidbody>();
    }

    public void spawnRabbit(float xLoc, float zLoc, float rabbitF, float rabbitV, float rabbitS) {
        // Create a new rabbit
        GameObject newAnimal = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag and layer
        newAnimal.tag = "Rabbit";
        newAnimal.layer = LayerMask.NameToLayer("Rabbit");

        // Add the script with variables
        RabbitScript rabbit = newAnimal.AddComponent<RabbitScript>();
        rabbit.FOV = rabbitF;
        rabbit.viewDistance = rabbitV;
        rabbit.speed = rabbitS;

        // Start the rabbit at a random position
        rabbit.transform.position = new Vector3(xLoc, 0.5f, zLoc);
        
        // Add the rigidbody so that it can collide
        rabbit.gameObject.AddComponent<Rigidbody>();

        allLivingRabbits.Add(rabbit);
    }

    public void spawnBush(float xLoc, float zLoc, float bushM) {
        // Create a new bush
        GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Add it's tag
        bush.tag = "Bush";
        bush.layer = LayerMask.NameToLayer("Bush");

        // Add the script
        bush.AddComponent<BushScript>().maxSize = bushM;

        // Start the rabbit at a random position and small size
        bush.transform.position = new Vector3(xLoc, 0.5f, zLoc);
        bush.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Add the rigidbody so that it can collide
        bush.AddComponent<Rigidbody>();
    }

    public void updateAnimalStats(TextMeshProUGUI textBox, string animalTag) {
        // Variables we will need
        float textFOV = 0;
        float textVD = 0;
        float textSpeed = 0;
        
        // Get all animals and add up stats
        GameObject[] animals = GameObject.FindGameObjectsWithTag(animalTag);
        foreach (GameObject animal in animals) {
            AnimalScript script = animal.GetComponent<AnimalScript>();
            textFOV += script.FOV;
            textVD += script.viewDistance;
            textSpeed += script.speed;
        }

        // Find the stat averages
        textFOV = textFOV / animals.Length;
        textVD = textVD / animals.Length;
        textSpeed = textSpeed / animals.Length;
        
        // Update the text with the stats
        textBox.text = animalTag + " Averages \nView Distance: " + textVD.ToString("F2") + "\nFOV: " + textFOV.ToString("F2") + "\nSpeed: " + textSpeed.ToString("F2");
    }

}
