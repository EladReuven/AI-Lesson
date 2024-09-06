using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public Transform target;
    public float flapTimeInterval = 1f;
    public float wingFlapStrength = 10f;
    public float tailMovementStrength = 10f;


    private NeuralNetWithCopyMono neuralNetwork;
    private Rigidbody rb;
    float flapTimer = 0f;

    public float Fitness { get; private set; }

    //left wing right wing
    //both = transform.forward + transform.up
    //if unbalanced then torque in direction of bigger one on z axis
    //tail x = torque y axis
    //tail y = torque x axis

    //each output should be a number between 0 - 1
    //output for wings will be (output divided by 2) * strength of flap
    //output for tail will be (output * 2 - 1) * strength of flap

    //inputs are - distance to target.
    //transform position
    //velocity
    //MAAAYYYYBE also distance from walls

    //fitness maybe remove points for touching obstacle or for being upside down.

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        neuralNetwork = GetComponent<NeuralNetWithCopyMono>();
        
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

            //// Adjust velocity to match the current forward direction, preserving speed
            //Vector3 currentVelocity = rb.velocity;
            //Vector3 newVelocityDirection = Vector3.Lerp(currentVelocity.normalized, transform.forward, turnDamping);
            //rb.velocity = newVelocityDirection * currentVelocity.magnitude;
        }

        // Update fitness score (inverse of distance to target)
        Fitness = 1 / Vector3.Distance(transform.position, target.position);
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
        //calculate fitness
        //save neural network somehow
        //reproduce.
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
