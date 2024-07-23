using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureControl : MonoBehaviour
{
    public int numberOfRays = 6;
    public float radius = 5f;
    public float killDistance = 0.62f;
    public Rigidbody rb;
    public Material material;
    public LayerMask wallLayer;
    public Checkpoint currentCheckpoint;

    public Transform startingPoint;

    public bool drawGizmos = true;

    bool dead = false;


    public bool mutateMutations = true;

    public float[] distances;
    public GameObject agentPrefab;
    public float mutationAmount = 0.8f;
    public float mutationChance = 0.2f;
    public NeuralNetWithCopyMono nn;


    // Start is called before the first frame update
    void Awake()
    {
        distances = new float[numberOfRays];
        material = new Material(material);
        rb = GetComponent<Rigidbody>();
        nn = gameObject.GetComponent<NeuralNetWithCopyMono>();
        //call mutate function to mutate the neural network
        MutateCreature();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCheckpoint == null)
        {
            print("reached end");
            return;
        }

        if(dead) return;
        // collect sensors data
        // if its distances info we will use distances...
        //raycast in x directions around the controller. collect distance to wall
        CollectDistances();
        // Setup inputs for the neural network
        float[] inputsToNN = distances;

        // Get outputs from the neural network (let the brain make a decision) 
        float[] outputsFromNN = nn.Brain(inputsToNN);

        // use the outputsFromNN to make an action...
        float leftright = outputsFromNN[0];
        float forwardback = outputsFromNN[1];
        rb.AddForce(new Vector3(leftright, 0, forwardback));
    }

    void CollectDistances()
    {
        float angleStep = 360f / numberOfRays;  // Angle step based on the number of rays
        Vector3 origin = transform.position;    // The position of the object casting the rays

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, radius, wallLayer))
            {
                float distance = hit.distance;
                if(distance <= killDistance)
                {
                    Die();
                    return;
                }
                distances[i] = distance;
                Debug.Log($"Ray {i}: Hit at distance {distance}");
            }
            else
            {
                distances[i] = -1;
                Debug.Log($"Ray {i}: No hit within radius {radius}");
            }

            Debug.DrawRay(origin, direction * radius, Color.red);
        }
    }


    private void MutateCreature()
    {
        material.color = Random.ColorHSV();
        if (mutateMutations)
        {
            mutationAmount += Random.Range(-1.0f, 1.0f) / 100;
            mutationChance += Random.Range(-1.0f, 1.0f) / 100;
        }

        //make sure mutation amount and chance are positive using max function
        mutationAmount = Mathf.Max(mutationAmount, 0);
        mutationChance = Mathf.Max(mutationChance, 0);

        nn.MutateNetwork(mutationAmount, mutationChance);
    }


    // for evolution sim - in other cases we will use fitness 
    public void Reproduce(int numberOfChildren)
    {
        //replicate
        for (int i = 0; i < numberOfChildren; i++)
        {
            //create a new agent, and set its position to the parent's position + a random offset in the x and z directions (so they don't all spawn on top of each other)
            GameObject child = Instantiate(gameObject);
            //copy the parent's neural network to the child
            child.GetComponent<NeuralNetWithCopyMono>().layers = GetComponent<NeuralNetWithCopyMono>().copyLayers();
        }
    }

    public void Die()
    {
        // calculate fitness test 
        //check distance to next checkpoint
        print("distance from next checkpoint: " + Vector3.Distance(transform.position, currentCheckpoint.transform.position));
        dead = true;
        Reproduce(1);
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        float angleStep = 360f / numberOfRays;  // Angle step based on the number of rays
        Vector3 origin = transform.position;    // The position of the object casting the rays

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Draw the rays as lines in the Scene view
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + direction * radius);
        }

        // Draw the circle outline
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, radius);
    }
}