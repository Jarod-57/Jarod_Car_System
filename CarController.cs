using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    public float maxMotorTorque = 5000f;
    public float maxSteeringAngle = 30f;
    public float brakeTorque = 11000f;
    public float accelerationRate = 50f;
    public float decelerationRate = 20000f; 
    public float steeringSpeed = 5f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float currentSteeringAngle = 0f;

    private bool isBraking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.9f, 0);
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheelPoses();
    }

    private void HandleMotor()
    {
        float targetMotor = maxMotorTorque * -Input.GetAxis("Vertical");
        
        if (Input.GetAxis("Vertical") != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetMotor, accelerationRate * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, decelerationRate * Time.deltaTime);
        }

        frontLeftWheel.motorTorque = currentSpeed;
        frontRightWheel.motorTorque = currentSpeed;
        rearLeftWheel.motorTorque = currentSpeed;
        rearRightWheel.motorTorque = currentSpeed;

        if (Input.GetKey(KeyCode.Space))
        {
            isBraking = true;
            ApplyBrakes();
        }
        else
        {
            isBraking = false;
            ReleaseBrakes();
        }
    }

    private void ApplyBrakes()
    {
        frontLeftWheel.brakeTorque = brakeTorque;
        frontRightWheel.brakeTorque = brakeTorque;
        rearLeftWheel.brakeTorque = brakeTorque;
        rearRightWheel.brakeTorque = brakeTorque;
    }

    private void ReleaseBrakes()
    {
        frontLeftWheel.brakeTorque = 0;
        frontRightWheel.brakeTorque = 0;
        rearLeftWheel.brakeTorque = 0;
        rearRightWheel.brakeTorque = 0;
    }

    private void HandleSteering()
    {
        float targetSteering = maxSteeringAngle * Input.GetAxis("Horizontal");
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteering, steeringSpeed * Time.deltaTime);

        frontLeftWheel.steerAngle = currentSteeringAngle;
        frontRightWheel.steerAngle = currentSteeringAngle;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateWheelPose(frontRightWheel, frontRightTransform);
        UpdateWheelPose(rearLeftWheel, rearLeftTransform);
        UpdateWheelPose(rearRightWheel, rearRightTransform);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform trans)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        trans.position = pos;
        trans.rotation = quat;
    }
    
    public void StopVehicle()
    {
        currentSpeed = 0;
        ApplyBrakes();
    }
}
