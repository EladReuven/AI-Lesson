using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitScript : AnimalScript
{
    RabbitStats stats;

    // Start is called before the first frame update
    void Start()
    {
        // Coloring
        mateColor = Color.cyan;       // cyan
        huntColor = Color.magenta;       // magenta
        passiveColor = Color.blue;    // blue
        GetComponent<Renderer>().material.color = passiveColor;
        
        // Set the variables that need to be set unique to animal
        maxHunger = 100;
        hunger = maxHunger;
        mateWaitingPeriod = false;
        eatWaitingPeriod = true;
        preyTag = "Bush";
        predatorTag = "Fox";

        // General variables to be set 
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;
        timeToFlee = timeToFlee * frameRate;
    }
    
    public void SetStats(float fov, float vDis, float speed, float MaxHunger)
    {
        stats.FOV = fov;
        stats.viewDistance = vDis;
        stats.speed = speed;
        stats.maxHunger = MaxHunger;
    }

    public RabbitStats GetStats => stats;
}

public struct RabbitStats
{
    public float FOV;
    public float viewDistance;
    public float speed;
    public float maxHunger;    // in speed per second
}