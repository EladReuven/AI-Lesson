using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public Transform target;
    public float flapTimeInterval = 1f;
    public float wingFlapStrength = 100f;
    public float tailMovementStrength = 50f;


    private NeuralNetWithCopyMono neuralNetwork;
    private Rigidbody rb;
    float flapTimer = 0f;
    bool dead = false;
    [SerializeField] float fitness = 0f;
    public float Fitness => fitness;
    public bool Dead => dead;
    public NeuralNetWithCopyMono NN => neuralNetwork;

    public float distanceFitnessWeight = 10f;
    public float orientationFitnessWeight = 2f;
    public float angularVelFitnessPenaltyWeight = 5f;

    private float timeAlive = 0f;
    private float distanceToTargetAtStart;
    private float orientationScore = 0f;
    private float angularVelocityPenalty = 0f;
    private float collisionPenalty = 0f;

    private void Awake()
    {
        fitness = 0f;
        rb = GetComponent<Rigidbody>();
        neuralNetwork = GetComponent<NeuralNetWithCopyMono>();

    }

    public void SetTarget(Transform T)
    {
        target = T;
        distanceToTargetAtStart = Vector3.Distance(transform.position, target.position);
    }

    public void FlapWings(float leftStrength, float rightStrength)
    {
        //adds impulse force upwards and forward
        Vector3 upForwardForce = transform.forward + transform.up;
        rb.AddForce(upForwardForce * (leftStrength + rightStrength), ForceMode.Impulse);

        //adds rotational force depending on what is stronger.
        float rotationalForce = rightStrength - leftStrength;
        rb.AddTorque(Vector3.forward * rotationalForce, ForceMode.Impulse);
    }

    void Update()
    {
        if (dead) return;

        timeAlive += Time.deltaTime;

        flapTimer += Time.deltaTime;

        if (flapTimer > flapTimeInterval)
        {
            print("flap");
            flapTimer = 0f;

            // Get input data
            float[] inputs = GetInputs();

            // Get outputs from neural network
            float[] outputs = neuralNetwork.Brain(inputs);

            // Control dragon movement based on neural network output
            float leftWingFlap = outputs[0] * wingFlapStrength;
            float rightWingFlap = outputs[1] * wingFlapStrength;
            float horizontalTail = outputs[2] * tailMovementStrength; //affects y torque
            float verticalTail = outputs[3] * tailMovementStrength; //affects x torque

            // Calculate movement and turning forces
            float totalMomentumForce = leftWingFlap + rightWingFlap;
            float horizontalRotationalForce = rightWingFlap - leftWingFlap;

            // Apply lift force vertically
            Vector3 liftForce = transform.up * totalMomentumForce + transform.forward * totalMomentumForce;
            rb.AddForce(liftForce);

            // Apply yaw (horizontal tail movement) for turning left/right
            Vector3 yawTorque = transform.up * horizontalTail;
            rb.AddTorque(yawTorque);

            // Apply pitch (vertical tail movement) for tilting up/down
            Vector3 pitchTorque = transform.right * verticalTail;
            rb.AddTorque(pitchTorque);

            // apply roll for rotating left/right
            Vector3 rollTorque = transform.forward * horizontalRotationalForce;
            rb.AddTorque(rollTorque);

        }

        // Update fitness
        UpdateFitness();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane" || collision.gameObject.tag == "Obstacle")
        {
            collisionPenalty += 10f; // Penalize heavily for hitting walls or ground
            Die();
        }
    }

    private void UpdateFitness()
    {
        // Time Alive boosts fitness
        float timeAliveFactor = timeAlive;

        // Distance to Target: the closer the better
        float distanceFactor = 1f - (Vector3.Distance(transform.position, target.position) / distanceToTargetAtStart);

        // Reward when the dragon's "up" is pointing towards the sky
        if (Vector3.Dot(transform.up, Vector3.up) > 0.1f) // Dot product close to 1 means it's oriented correctly
        {
            orientationScore += Time.deltaTime;
        }

        // Angular Velocity: penalize if spinning too fast
        if (rb.angularVelocity.magnitude > 1f) // Threshold for excessive spinning
        {
            angularVelocityPenalty += Time.deltaTime;
        }

        // Total Fitness Calculation
        fitness = timeAliveFactor + (distanceFactor * distanceFitnessWeight) + (orientationScore * orientationFitnessWeight) - (angularVelocityPenalty * angularVelFitnessPenaltyWeight) - collisionPenalty;
    }


    private float[] GetInputs()
    {
        Vector3 relativePosition = target.position - transform.position;
        float[] inputs = new float[]
        {
            //current pos x, y, z
            transform.position.x,
            transform.position.y,
            transform.position.z,

            //distance to target
            Vector3.Distance(target.position, transform.position),

            //relativePosition.x, relativePosition.y, relativePosition.z,

            //Velocity x, y, z
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z


            //total of 7 inputs currently
        };

        return inputs;
    }

    public void Die()
    {
        if (dead) return;
        dead = true;
        //calculate fitness
        UpdateFitness();
        //save neural network somehow

        //reproduce.

    }

    public void MutateDragon()
    {
        if (Random.Range(0f, 1f) <= NN.dragonMutateChance)
            NN.MutateNetwork(NN.mutationAmount, NN.mutationChance);
    }

    [ContextMenu("TestFlapWings")]
    public void TestFlapWings()
    {
        //adds impulse force upwards and forward
        Vector3 upForwardForce = transform.forward + transform.up;
        rb.AddForce(upForwardForce * 25, ForceMode.Impulse);

        ////adds rotational force depending on what is stronger.
        //float rotationalForce = rightWingFlapStrength - leftWingFlapStrength;
        //rb.AddTorque(Vector3.forward * rotationalForce, ForceMode.Impulse);
    }

    [ContextMenu("TestTorqueX")]
    public void TestAddTorque()
    {
        rb.AddTorque(Vector3.right * 5, ForceMode.Impulse);
    }

}
