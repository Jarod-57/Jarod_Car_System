using UnityEngine;

public class VehicleCamera : MonoBehaviour
{
    public Transform vehicle; 
    public float distance = 5.0f;
    public float height = 2.0f;
    public float rotationSpeed = 3.0f;

    private float currentX = 0.0f; 
    private float currentY = 0.0f;
    private float minY = -20f; 
    private float maxY = 80f; 

    void Update()
    {
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, minY, maxY);
    }

    void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = vehicle.position + rotation * direction + Vector3.up * height;
        transform.LookAt(vehicle.position + Vector3.up * height);
    }
}
