using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TequilaSunrise.Motorcycle
{
    public class MotorcycleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody motorcycleRigidbody;
        [SerializeField] private Transform motorcycleBody;
        [SerializeField] private Transform frontWheel;
        [SerializeField] private Transform rearWheel;
        [SerializeField] private Transform handlebar;
        [SerializeField] private Transform frontSuspension;
        
        [Header("Engine Settings")]
        [SerializeField] private float enginePower = 1500f;
        [SerializeField] private float brakeForce = 2000f;
        [SerializeField] private float maxSpeed = 100f; // km/h
        [SerializeField] private AnimationCurve powerCurve;
        
        [Header("Steering Settings")]
        [SerializeField] private float steeringSpeed = 2.0f;
        [SerializeField] private float maxSteeringAngle = 30f;
        [SerializeField] private float leanAngle = 30f;
        [SerializeField] private float counterSteeringFactor = 0.5f;
        
        [Header("Suspension")]
        [SerializeField] private float suspensionHeight = 0.2f;
        [SerializeField] private float suspensionSpringStrength = 200f;
        [SerializeField] private float suspensionDamping = 20f;
        [SerializeField] private float wheelRadius = 0.3f;
        [SerializeField] private LayerMask groundLayers;
        
        [Header("Stability")]
        [SerializeField] private float downforce = 100f;
        [SerializeField] private float uprightTorque = 50f;
        [SerializeField] private float angularDragGround = 5f;
        [SerializeField] private float angularDragAir = 0.5f;
        
        // Input values
        private float throttleInput = 0f;
        private float brakeInput = 0f;
        private float steeringInput = 0f;
        
        // Runtime variables
        private float currentSteeringAngle = 0f;
        private float currentLeanAngle = 0f;
        private bool frontWheelGrounded = false;
        private bool rearWheelGrounded = false;
        
        private void Start()
        {
            if (motorcycleRigidbody == null)
            {
                motorcycleRigidbody = GetComponent<Rigidbody>();
            }
            
            // Set physics settings
            motorcycleRigidbody.centerOfMass = new Vector3(0, -0.2f, 0);
        }
        
        private void FixedUpdate()
        {
            ApplyDownforce();
            CheckWheelsGrounded();
            UpdateSuspension();
            UpdateStabilizers();
            
            HandleSteering();
            ApplyMotorTorque();
            ApplyBraking();
            ApplyLean();
            
            UpdateWheelMeshes();
        }
        
        public void SetInputs(float throttle, float brake, float steering)
        {
            throttleInput = Mathf.Clamp01(throttle);
            brakeInput = Mathf.Clamp01(brake);
            steeringInput = Mathf.Clamp(steering, -1f, 1f);
        }
        
        private void HandleSteering()
        {
            // Apply counter-steering at higher speeds
            float normalizedSpeed = motorcycleRigidbody.linearVelocity.magnitude / maxSpeed;
            float targetAngle = steeringInput * maxSteeringAngle;
            
            // Reduce steering angle at higher speeds
            targetAngle *= Mathf.Lerp(1.0f, 0.5f, normalizedSpeed);
            
            // Smoothly interpolate current steering angle
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, Time.fixedDeltaTime * steeringSpeed);
            
            // Apply steering to handlebar and front wheel
            if (handlebar != null)
            {
                handlebar.localRotation = Quaternion.Euler(0, currentSteeringAngle, 0);
            }
            
            // Apply steering force
            if (frontWheelGrounded)
            {
                Vector3 steeringDir = Quaternion.Euler(0, currentSteeringAngle, 0) * transform.forward;
                float steeringForce = Mathf.Lerp(20f, 5f, normalizedSpeed);
                motorcycleRigidbody.AddForce(steeringDir * steeringForce, ForceMode.Acceleration);
            }
        }
        
        private void ApplyMotorTorque()
        {
            // Calculate current speed in km/h
            float speed = motorcycleRigidbody.linearVelocity.magnitude * 3.6f; // Convert to km/h
            
            // Apply motor torque to rear wheel
            if (speed < maxSpeed)
            {
                // Get power based on current speed
                float normalizedSpeed = speed / maxSpeed;
                float powerFactor = powerCurve.Evaluate(normalizedSpeed);
                
                // Calculate forward force
                Vector3 forwardForce = transform.forward * enginePower * throttleInput * powerFactor;
                
                // Only apply if rear wheel is grounded
                if (rearWheelGrounded)
                {
                    motorcycleRigidbody.AddForce(forwardForce, ForceMode.Force);
                }
            }
            
            // Spin wheels for visual effect
            if (rearWheel != null)
            {
                rearWheel.Rotate(Vector3.right, motorcycleRigidbody.linearVelocity.magnitude * Time.fixedDeltaTime * 40f, Space.Self);
            }
        }
        
        private void ApplyBraking()
        {
            if (brakeInput > 0.1f && motorcycleRigidbody.linearVelocity.magnitude > 0.1f)
            {
                // Calculate brake force in the opposite direction of travel
                Vector3 brakeForceVector = -motorcycleRigidbody.linearVelocity.normalized * brakeForce * brakeInput;
                
                // Apply brake force when wheels are grounded
                if (frontWheelGrounded || rearWheelGrounded)
                {
                    motorcycleRigidbody.AddForce(brakeForceVector, ForceMode.Force);
                }
            }
        }
        
        private void ApplyLean()
        {
            // Calculate lean based on steering and speed
            float speed = motorcycleRigidbody.linearVelocity.magnitude;
            float targetLean = -steeringInput * leanAngle * (speed / maxSpeed);
            
            // Apply lean to motorcycle body
            if (motorcycleBody != null)
            {
                currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, Time.fixedDeltaTime * 2.0f);
                motorcycleBody.localRotation = Quaternion.Euler(0, 0, currentLeanAngle);
            }
        }
        
        private void ApplyDownforce()
        {
            // Apply downforce to keep motorcycle grounded
            motorcycleRigidbody.AddForce(-transform.up * downforce * motorcycleRigidbody.linearVelocity.magnitude);
        }
        
        private void UpdateWheelMeshes()
        {
            if (frontWheel != null)
            {
                frontWheel.Rotate(Vector3.right, motorcycleRigidbody.linearVelocity.magnitude * Time.fixedDeltaTime * 40f, Space.Self);
            }
        }
        
        private void CheckWheelsGrounded()
        {
            // Front wheel ground check
            if (frontWheel != null)
            {
                frontWheelGrounded = Physics.CheckSphere(frontWheel.position - new Vector3(0, wheelRadius, 0), 
                                                      wheelRadius, groundLayers, QueryTriggerInteraction.Ignore);
            }
            
            // Rear wheel ground check
            if (rearWheel != null)
            {
                rearWheelGrounded = Physics.CheckSphere(rearWheel.position - new Vector3(0, wheelRadius, 0),
                                                      wheelRadius, groundLayers, QueryTriggerInteraction.Ignore);
            }
            
            // Set angular drag based on grounding
            motorcycleRigidbody.angularDrag = (frontWheelGrounded || rearWheelGrounded) ? angularDragGround : angularDragAir;
        }
        
        private void UpdateSuspension()
        {
            // Simple suspension for front wheel
            if (frontWheel != null && frontSuspension != null)
            {
                RaycastHit hit;
                Vector3 rayStart = frontWheel.position + Vector3.up * 0.1f;
                
                if (Physics.Raycast(rayStart, Vector3.down, out hit, suspensionHeight + 0.1f, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    float suspensionTravel = suspensionHeight - (hit.distance - 0.1f);
                    frontSuspension.localPosition = new Vector3(0, -suspensionTravel * 0.5f, 0);
                }
                else
                {
                    frontSuspension.localPosition = Vector3.zero;
                }
            }
        }
        
        private void UpdateStabilizers()
        {
            // Apply auto-upright torque based on current tilt
            Vector3 currentUp = transform.up;
            Vector3 worldUp = Vector3.up;
            
            // Calculate the torque needed to align with world up
            Vector3 torqueDirection = Vector3.Cross(currentUp, worldUp);
            
            // Only apply when grounded and not intentionally leaning
            if ((frontWheelGrounded || rearWheelGrounded) && Mathf.Abs(steeringInput) < 0.1f)
            {
                motorcycleRigidbody.AddTorque(torqueDirection * uprightTorque, ForceMode.Acceleration);
            }
        }
        
        public float GetSpeed()
        {
            return motorcycleRigidbody.linearVelocity.magnitude * 3.6f; // Return speed in km/h
        }
    }
} 