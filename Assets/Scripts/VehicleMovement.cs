using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public float speed;                     // forward speed of the vehicle

    [Header("Drive Settings")]
    public float driveForce = 30f;          // force applied to move the vehicle
    public float slowingVelFactor = .99f;   // percentage of velocity vehicle maintains when not thrusting
    public float angleOfRoll = 30f;         // angle vehicle banks into a turn

    [Header("Hover Settings")]
    public float hoverHeight = 1.5f;        // height we want to keep above the ground
    public float maxGroundDist = 5f;        // how far the vehicle is allowed to be above the ground before falling
    public float hoverForce = 300f;         // force applied to keep the vehicle hovering at the hover height
    public LayerMask groundLayer;           // layermask used to identify what is considered the ground
    public PIDController hoverPID;          // PID controller used to smooth the hovering of the vehicle

    [Header("Physics Settings")]
    public Transform vehicleBody;              // reference to the vehicle's body for cosmetics
    public float terminalVelocity = 100f;    // max speed the vehicle can go
    public float hoverGravity = 5f;        // gravity applied to the vehicle while it is on the ground
    public float fallGravity = 40f;         // gravity applied to the vehicle while it is falling

    Rigidbody vehicleRigidBody;             // reference to the vehicle rigidbody
    PlayerInput input;                      // reference to the player input script
    float drag;                             // drag of the vehicle rigidbody when moving forward
    bool isOnGround;                        // whether the vehicle is currently on the ground

    void Start()
    {
        // Get the rigidbody component
        vehicleRigidBody = GetComponent<Rigidbody>();

        // Get the player input script
        input = GetComponent<PlayerInput>();

        // Calculate the drag value
        drag = driveForce / terminalVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Calculate current speed using dot product. This will tell us how much vehicles velocity is in forward direction
        speed = Vector3.Dot(vehicleRigidBody.velocity, transform.forward);

        // Calculate forces applied to vehicle
        CalculateHover();
        CalculatePropulsion();
    }

    void CalculateHover()
    {
        // Variable to hold normal of the ground.
        Vector3 groundNormal;

        // Create a ray that points straight down from the vehicle
        Ray ray = new Ray(transform.position, -transform.up);

        // Hold result of raycast in variable
        RaycastHit hitInfo;

        // Determine if vehicle is on ground by raycasting down from the vehicle to see if it hits any collider on the ground layer.
        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, groundLayer);

        // If the vehicle is on the ground...
        if (isOnGround)
        {
            // Calculate how high off the ground vehicle is
            float height = hitInfo.distance;

            // Save normal of the ground
            groundNormal = hitInfo.normal.normalized;

            // Use PID controller to calculate the amount of force needed to hover
            float forcePercent = hoverPID.Seek(hoverHeight, height);

            // Calculate total hover force by multiplying force percent by the max hover force
            Vector3 force = groundNormal * hoverForce * forcePercent;

            // Calculate the force and direction gravity will stick the vehicle to the track (not always straight down)
            Vector3 gravity = -groundNormal * hoverGravity * height;

            // Apply the hover  and gravity forces to the rigidbody
            vehicleRigidBody.AddForce(force, ForceMode.Acceleration);
            vehicleRigidBody.AddForce(gravity, ForceMode.Acceleration);

            // Calculate the ground normal
            groundNormal = hitInfo.normal;
        }

        else
        {
            // If the vehicle is not on the ground, set the ground normal to the vehicles up vector
            groundNormal = Vector3.up;

            // Apply stronger downward force to vehicle
            Vector3 gravity = -groundNormal * fallGravity;
            vehicleRigidBody.AddForce(gravity, ForceMode.Acceleration);
        }

        // Calculate pitch and roll needed to match orientation with ground.
        // Done by creating a projection then calculating the rotation needed to face that projection
        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

        // Move vehicle over time to match desired rotation with the ground using Lerp to smooth the transition
        vehicleRigidBody.MoveRotation(Quaternion.Lerp(vehicleRigidBody.rotation, rotation, Time.deltaTime * 10f));

        // Calculate angle we want vehicle to bank into a turn
        float angle = angleOfRoll * -input.rudder;

        // Calculate the rotation needed for this new angle
        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);

        // Apple to vehicle body
        vehicleBody.rotation = Quaternion.Lerp(vehicleBody.rotation, bodyRotation, Time.deltaTime * 10f);
    }

    void CalculatePropulsion()
    {
        // Transform angular velocity into local space
        Vector3 localAngulerVelocity = transform.InverseTransformDirection(vehicleRigidBody.angularVelocity).normalized * vehicleRigidBody.angularVelocity.magnitude;

        // Calculate the yaw torque based on rudder and current angular velocity
        float rotationTorque = input.rudder - localAngulerVelocity.y;

        // Apply torque to vehicle Y axis
        vehicleRigidBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);

        // Calculate current sideways speed using dot product
        // This will tell us how much vehicle velocity is in the right or left direction
        float sidewaysSpeed = Vector3.Dot(vehicleRigidBody.velocity, transform.right);

        // Calculate desired amount of friction to apply to side of vehicle.
        // This keeps vehicle from drifting into walls during turns. To add drift, divine Time.fixedDeltaTime by a value
        Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / 10);

        // Apply friction to vehicle
        vehicleRigidBody.AddForce(sideFriction, ForceMode.Acceleration);

        // If not accelerating, slow vehicle down (change/remove if static speed is required)
        if (input.thruster <= 0f)
        {
            vehicleRigidBody.velocity *= slowingVelFactor;
        }

        // Calculate the force needed to move the vehicle forward based on the input and subtract drag
        float propulsion = driveForce * input.thruster - drag * Mathf.Clamp(speed, 0f, terminalVelocity);
        vehicleRigidBody.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
    }

    void OnCollisionStay(Collision collision)
    {
        // If vehicle hits the wall
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Calculate upward impulse generated and add equal force down to hopefully keep vehicle from flying over wall
            Vector3 upwardForceFromCollision = Vector3.Dot(collision.impulse, transform.up) * transform.up;
            vehicleRigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);
        }
    }

    public float GetSpeedPercentage()
    {
        // Calculate the total speed percentage of the vehicle moving forward
        return vehicleRigidBody.velocity.magnitude / terminalVelocity;
    }
}
