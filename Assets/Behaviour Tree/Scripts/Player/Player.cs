using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;              // Maximum movement speed
    public float acceleration = 10f;          // Acceleration rate
    public float deceleration = 10f;          // Deceleration rate

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 targetVelocity;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody has the necessary settings
        rb.constraints = RigidbodyConstraints.FreezeRotation;  // Prevent the Rigidbody from rotating
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth the Rigidbody's movement
    }

    void Update()
    {
        // Get input from WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate target movement direction
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Calculate target velocity based on move direction and speed
        targetVelocity = moveDirection * moveSpeed;
    }

    void FixedUpdate()
    {
        // Get the current velocity
        Vector3 currentVelocity = rb.velocity;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Accelerate towards target velocity
            Vector3 velocityChange = targetVelocity - currentVelocity;
            Vector3 accelerationVector = velocityChange.normalized * acceleration * Time.fixedDeltaTime;
            rb.velocity += Vector3.ClampMagnitude(accelerationVector, velocityChange.magnitude);
        }
        else
        {
            // Decelerate to a stop
            rb.velocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }


}