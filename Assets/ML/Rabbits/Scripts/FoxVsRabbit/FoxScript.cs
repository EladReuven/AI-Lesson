using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxScript : AnimalScript
{

    // Start is called before the first frame update
    void Start()
    {
        // Coloring
        mateColor = Color.black;       // black
        huntColor = Color.yellow;       // yellow
        passiveColor = Color.red;    // red
        GetComponent<Renderer>().material.color = passiveColor;

        // Set the variables that need to be set unique to animal
        maxHunger = 150;
        hunger = maxHunger;
        mateWaitingPeriod = false;
        eatWaitingPeriod = true;
        preyTag = "Rabbit";
        predatorTag = null;

        // General variables to be set 
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;
        timeToFlee = timeToFlee * frameRate;
    }

}

