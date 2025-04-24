using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorcycleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody motorcycleRigidbody;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private WheelCollider frontWheelCollider;
    [SerializeField] private WheelCollider rearWheelCollider;
    [SerializeField] private Transform frontWheelMesh;
    [SerializeField] private Transform rearWheelMesh;
    [SerializeField] private Transform handlebarMesh;
    
    [Header("Engine Settings")]
    [SerializeField] private float maxMotorTorque = 800f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float brakeTorque = 500f;
    [SerializeField] private float steeringAngle = 30f;
    
    [Header("Motorcycle Physics")]
    [SerializeField] private float leanAngle = 30f;
    [SerializeField] private float leanSpeed = 5f;
    [SerializeField] private float downforce = 100f;
    
    [Header("Interaction")]
    [SerializeField] private Transform mountPosition;
    [SerializeField] private float interactionDistance = 2f;
    
    // Controls
    private float throttleInput = 0f;
    private float brakeInput = 0f;
    private float steeringInput = 0f;
    
    // State
    private bool isBeingRidden = false;
    private AvatarController currentRider;
    
    private void Awake()
    {
        // Get components if not assigned
        if (motorcycleRigidbody == null)
            motorcycleRigidbody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        // Set center of mass
        if (centerOfMass != null)
            motorcycleRigidbody.centerOfMass = centerOfMass.localPosition;
        
        // Set up wheel colliders
        ConfigureWheelColliders();
    }
    
    private void Update()
    {
        if (isBeingRidden)
        {
            // Get input when being ridden - mobile controls
            GetPlayerInput();
        }
        
        // Update wheel visuals
        UpdateWheelMeshes();
    }
    
    private void FixedUpdate()
    {
        // Apply forces based on input
        ApplyMotorTorque();
        ApplySteering();
        ApplyLean();
        ApplyDownforce();
    }
    
    private void ConfigureWheelColliders()
    {
        // Configure wheel frictions
        WheelFrictionCurve frictionCurve = frontWheelCollider.forwardFriction;
        frictionCurve.stiffness = 1.5f;
        frontWheelCollider.forwardFriction = frictionCurve;
        rearWheelCollider.forwardFriction = frictionCurve;
        
        frictionCurve = frontWheelCollider.sidewaysFriction;
        frictionCurve.stiffness = 1.5f;
        frontWheelCollider.sidewaysFriction = frictionCurve;
        rearWheelCollider.sidewaysFriction = frictionCurve;
    }
    
    private void GetPlayerInput()
    {
        // Get input from virtual joystick or other mobile controls
        // For simplicity, we'll assume a joystick provides values from -1 to 1
        // This should be connected to your mobile UI controls
        throttleInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        brakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f; // Example using Space as brake
    }
    
    private void ApplyMotorTorque()
    {
        // Calculate current speed in km/h
        float speed = motorcycleRigidbody.velocity.magnitude * 3.6f; // Convert to km/h
        
        // Apply motor torque to rear wheel
        if (speed < maxSpeed)
        {
            float torque = maxMotorTorque * throttleInput;
            rearWheelCollider.motorTorque = torque;
        }
        else
        {
            rearWheelCollider.motorTorque = 0f;
        }
        
        // Apply brakes
        frontWheelCollider.brakeTorque = brakeInput * brakeTorque;
        rearWheelCollider.brakeTorque = brakeInput * brakeTorque;
    }
    
    private void ApplySteering()
    {
        // Apply steering to front wheel
        frontWheelCollider.steerAngle = steeringInput * steeringAngle;
        
        // Rotate handlebars to match steering
        if (handlebarMesh != null)
        {
            Vector3 rotation = handlebarMesh.localEulerAngles;
            rotation.y = steeringInput * steeringAngle;
            handlebarMesh.localEulerAngles = rotation;
        }
    }
    
    private void ApplyLean()
    {
        // Calculate lean based on steering and speed
        float speed = motorcycleRigidbody.velocity.magnitude;
        float targetLean = -steeringInput * leanAngle * (speed / maxSpeed);
        
        // Apply lean to motorcycle body
        Vector3 rotation = transform.eulerAngles;
        rotation.z = Mathf.Lerp(rotation.z, targetLean, Time.fixedDeltaTime * leanSpeed);
        transform.eulerAngles = rotation;
    }
    
    private void ApplyDownforce()
    {
        // Apply downforce to keep motorcycle grounded
        motorcycleRigidbody.AddForce(-transform.up * downforce * motorcycleRigidbody.velocity.magnitude);
    }
    
    private void UpdateWheelMeshes()
    {
        // Update front wheel mesh position and rotation
        UpdateWheelMesh(frontWheelCollider, frontWheelMesh);
        
        // Update rear wheel mesh position and rotation
        UpdateWheelMesh(rearWheelCollider, rearWheelMesh);
    }
    
    private void UpdateWheelMesh(WheelCollider collider, Transform wheelMesh)
    {
        if (collider == null || wheelMesh == null)
            return;
            
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }
    
    public void MountRider(AvatarController rider)
    {
        if (isBeingRidden || rider == null)
            return;
            
        isBeingRidden = true;
        currentRider = rider;
        
        // Move rider to mount position
        rider.OnMotorcycleMount(mountPosition);
    }
    
    public void DismountRider()
    {
        if (!isBeingRidden || currentRider == null)
            return;
            
        // Reset inputs
        throttleInput = 0f;
        brakeInput = 0f;
        steeringInput = 0f;
        
        // Dismount rider
        currentRider.OnMotorcycleDismount();
        currentRider = null;
        isBeingRidden = false;
    }
    
    public bool CanInteractWith(Transform playerTransform)
    {
        // Check if player is close enough to interact
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= interactionDistance;
    }
} 