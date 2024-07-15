using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalScript : MonoBehaviour
{
    // User decide / survivability
    public float FOV;
    public float viewDistance;
    public float speed;
    protected float maxHunger = 60;    // in speed per second
    protected float hunger;

    // Growing
    protected bool mature = false;
    protected float size = 0.25f;
    protected float sizePerSecond;
    protected float timeToMature = 10;

    // Hunting
    protected string preyTag;
    protected GameObject huntedPrey = null;
    protected bool eatWaitingPeriod = false;

    // Mating
    protected GameObject mate = null;
    protected float matingHungerThreshold = 0.5f;
    protected bool mateWaitingPeriod = false;

    // Fleeing from predator
    protected string predatorTag = null;
    protected GameObject predator = null;
    protected float timeToFlee = 3;                 // In seconds
    protected float timeToFleeCounter = 0;
    protected bool canFlee = true;                  // Keeps track if running from a predator

    // Waiting
    protected bool waitBool = false;
    protected float waitTotalTime;
    protected float waitCounter;

    // Movement
    protected int frameRate;
    protected int moveTime;
    protected int moveCounter;
    protected Vector3 turnRate;
    protected bool walk = true;

    // Coloring
    protected Color mateColor = Color.blue;       // cyan
    protected Color huntColor = Color.blue;       // magenta
    protected Color passiveColor = Color.blue;    // blue

    // Survival Measurements
    protected float timeSinceAlive = 0f;

    static float highestTimeAlive = 0f;
    static float survivalSpeed = 0f;
    public float SurvivorFOV;
    public float SurvivorviewDistance;
    public float Survivorspeed;
    protected float SurvivormaxHunger = 60;    // in speed per second
    protected float Survivorhunger;




    // Start is called before the first frame update
    void Start()
    {
        hunger = maxHunger;
        frameRate = GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().frameRate;
        sizePerSecond = (1 - size) / timeToMature / frameRate;
        timeToFlee = timeToFlee * frameRate;

        GetComponent<Renderer>().material.color = passiveColor;
    }

    // Update is called once per frame
    protected void Update()
    {
        timeSinceAlive += Time.deltaTime;

        if (!waitBool)
        {
            // If not mature, grow
            if (!mature)
            {
                Grow();
            }

            // Handle hunger
            Hunger();

            // If animal has predator, avoid predator
            if (predatorTag != null && canFlee == true)
            {
                FleePredator();
            }

            // If in sight, look for prey
            huntedPrey = See(preyTag);

            // If in sight and mature, look for a mate
            if (mature)
            {
                mate = See(tag);
            }

            // Move towards mate then prey then randomly
            Move();
        }
        else
        {
            waitCounter++;
            if (waitCounter > waitTotalTime)
            {
                waitCounter = 0;
                waitBool = false;
            }
        }
    }

    // Grow and increase in size   
    protected void Grow()
    {
        size += sizePerSecond;
        transform.localScale = new Vector3(size, size, size);
        if (size >= 1)
        {
            mature = true;
        }
    }

    // Cost hunger, die if starve
    protected void Hunger()
    {
        // hunger decreases at speed per second
        hunger -= speed / frameRate;
        if (hunger < 0)
        {
            Destroy(gameObject);
        }
    }

    // Look out for predator 
    protected void FleePredator()
    {
        GameObject newPredator = See(predatorTag);
        if (newPredator != null)
        {
            // Update variables related to the predator
            predator = newPredator;
            timeToFleeCounter = 0;
            canFlee = false;

            // Look away from predator
            Vector3 predatorVector = new Vector3(predator.transform.position.x, transform.position.y, predator.transform.position.z);
            transform.LookAt(predatorVector);
            transform.RotateAround(transform.position, transform.up, 180f);
        }
    }


    // Find the closest target and if in sight, set them as the target
    protected GameObject See(string searchTag)
    {
        GameObject finalTarget = null;
        float closestFinalTarget = float.MaxValue;

        // Skip this if searching for mate and too hungry
        if (searchTag == tag && hunger <= (maxHunger * matingHungerThreshold))
        {
            return finalTarget;
        }

        // Find target in the right range
        GameObject[] targets = GameObject.FindGameObjectsWithTag(searchTag);
        foreach (GameObject target in targets)
        {
            Vector3 targetDir = target.transform.position - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            float distancetoPrey = Vector3.Distance(transform.position, target.transform.position);

            // If target is self, ignore
            if (target == this.gameObject)
            {
                break;
            }

            // Is in FOV and within sight distance
            if (angle < FOV && distancetoPrey < viewDistance)
            {
                bool closer = distancetoPrey < closestFinalTarget;

                // Based on what it sees
                switch (searchTag)
                {
                    // Prey
                    case var preyTag when preyTag == preyTag:
                        if (closer)
                        {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;

                    // Mate
                    case var tag when tag == tag:
                        bool canSelfMate = hunger > (maxHunger * matingHungerThreshold);
                        bool canTargetMate = target.GetComponent<AnimalScript>().hunger > (target.GetComponent<AnimalScript>().maxHunger * matingHungerThreshold);
                        if (canSelfMate && canTargetMate && closer)
                        {
                            finalTarget = target;
                            closestFinalTarget = distancetoPrey;
                        }
                        break;

                    // default
                    default:
                        break;
                }
            }
        }

        return finalTarget;
    }

    // Move towards prey or look around
    protected void Move()
    {
        // How fast to move forward, scales with size
        var step = speed * size * Time.deltaTime;

        // flee predator, else go to mate, else hunt prey, else passive move
        if (predator != null)
        {
            // Flee for certain time
            if (timeToFleeCounter < timeToFlee)
            {
                timeToFleeCounter++;
                transform.Translate(Vector3.forward * step);
            }
            else
            {
                predator = null;
                canFlee = true;
            }

        }
        else if (mate != null)
        {
            GetComponent<Renderer>().material.color = mateColor;

            // Move towards target
            Vector3 targetVector = new Vector3(mate.transform.position.x, transform.position.y, mate.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);

        }
        else if (huntedPrey != null)
        {
            GetComponent<Renderer>().material.color = huntColor;

            // Move towards prey
            Vector3 targetVector = new Vector3(huntedPrey.transform.position.x, transform.position.y, huntedPrey.transform.position.z);
            transform.LookAt(targetVector);
            transform.position = Vector3.MoveTowards(transform.position, targetVector, step);

        }
        else
        {
            GetComponent<Renderer>().material.color = passiveColor;

            // Once time expires, change walking and turning
            if (moveCounter >= moveTime)
            {
                swapWalkAndTurn();
            }

            moveCounter++;
            // If walking forward, else turning
            if (walk)
            {
                transform.Translate(Vector3.forward * step);
            }
            else
            {
                transform.Rotate(turnRate * Time.deltaTime);
            }
        }
    }

    // If walking, then turn. If turning, then walk
    protected void swapWalkAndTurn()
    {
        walk = !walk;
        moveCounter = 0;

        // If it should walk, go for 1-10 seconds. Else turn for 3-5 seconds. 
        if (walk)
        {
            moveTime = Random.Range(frameRate, 10 * frameRate);
        }
        else
        {
            int minTime = 3;
            int maxTime = 5;
            moveTime = Random.Range(minTime * frameRate, maxTime * frameRate);
            float turningSpeed = Random.Range(40f / maxTime, 360f / maxTime);
            if (Random.Range(0f, 1f) > 0.5)
            {
                turnRate = new Vector3(0, turningSpeed, 0);
            }
            else
            {
                turnRate = new Vector3(0, -turningSpeed, 0);
            }
        }
    }

    // When collide with prey, delete the prey
    protected void OnCollisionEnter(Collision other)
    {
        // If collides with mate
        if (other.gameObject.tag == tag && (mate == other.gameObject || other.gameObject.GetComponent<AnimalScript>().mate == this.gameObject))
        {
            // Subtract hunger and look at each other
            hunger -= (int)(maxHunger * (matingHungerThreshold / 2));
            Vector3 targetVector = new Vector3(other.transform.position.x, 0.5f, other.transform.position.z);
            transform.LookAt(targetVector);

            // Wait
            if (mateWaitingPeriod == true)
            {
                startWait(2f);
            }

            // If lower X position, be the one to spawn the baby
            if (transform.position.x <= other.gameObject.transform.position.x)
            {
                float averageSpeed = (speed + other.gameObject.GetComponent<AnimalScript>().speed) / 2;
                if (Random.Range(0f, 1f) > 0.5)
                {
                    averageSpeed += 1;
                }

                // Spawn in children
                if (tag == "Fox")
                {
                    GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().spawnFox(transform.position.x + 0.75f, transform.position.z + 0.75f, FOV, viewDistance, averageSpeed);
                }
                else if (tag == "Rabbit")
                {
                    GameObject.FindGameObjectWithTag("Plane").GetComponent<PlaneScript>().spawnRabbit(transform.position.x + 0.75f, transform.position.z + 0.75f, FOV, viewDistance, averageSpeed);
                }
            }

            // Look away from each other and continue moving
            transform.RotateAround(transform.position, transform.up, 180f);
            swapWalkAndTurn();

            // If collides with prey
        }
        else if (other.gameObject.tag == preyTag)
        {
            // Increase hunger
            hunger += (int)(other.gameObject.transform.localScale.x * maxHunger);
            if (hunger > maxHunger)
            {
                hunger = maxHunger;
            }

            // Wait to eat
            // NOTE: Find way to destroy prey after the 2 seconds
            if (eatWaitingPeriod == true)
            {
                startWait(0.5f);
            }

            // Actually eat the prey
            Destroy(other.gameObject);

            // If collides with wall
        }
        else if (other.gameObject.tag == "Border")
        {
            // Turn around and look in random location and move forward
            transform.RotateAround(transform.position, transform.up, 180f);
            transform.RotateAround(transform.position, transform.up, Random.Range(-90f, 90f));
            transform.Translate(Vector3.forward * speed * size * Time.deltaTime);

            // start walking
            swapWalkAndTurn();
            if (walk == false)
            {
                swapWalkAndTurn();
            }
        }
    }

    // Time in seconds to wait
    protected void startWait(float time)
    {
        waitBool = true;
        waitTotalTime = time * frameRate;
    }

}
