using UnityEngine;
using System.Collections;

public class VehicleInteraction : MonoBehaviour
{
    public GameObject player; 
    public GameObject impostorPlayer;
    public Transform seatPosition; 
    public Camera playerCamera; 
    public Camera vehicleCamera; 

    private bool isNearVehicle = false;
    private bool isDriving = false;

    private CarController carController;
    private Component[] playerComponents;
    private Animator playerAnimator; 

    private float ragdollDuration = 3f; 

    void Start()
    {
        carController = GetComponent<CarController>();
        playerComponents = player.GetComponents<Component>();
        playerAnimator = player.GetComponent<Animator>(); 
        carController.enabled = false;
        vehicleCamera.enabled = false;

        impostorPlayer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isNearVehicle && Input.GetKeyDown(KeyCode.F)) EnterVehicle();
        else if (isDriving && Input.GetKeyDown(KeyCode.F)) ExitVehicle();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) isNearVehicle = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) isNearVehicle = false;
    }

    void EnterVehicle()
    {
        isDriving = true;
        isNearVehicle = false;

        foreach (Component comp in playerComponents)
        {
            if (!(comp is Transform)) 
            {
                if (comp is MonoBehaviour) ((MonoBehaviour)comp).enabled = false;
                else if (comp is Collider) ((Collider)comp).enabled = false;
                else if (comp is Rigidbody) ((Rigidbody)comp).isKinematic = true;
            }
        }

        carController.enabled = true;

        playerCamera.enabled = false;

        impostorPlayer.gameObject.SetActive(true);

        vehicleCamera.enabled = true;

        player.transform.SetParent(seatPosition);
        player.SetActive(false);
    }

    void ExitVehicle()
    {
        isDriving = false;

        foreach (Component comp in playerComponents)
        {
            if (!(comp is Transform)) 
            {
                if (comp is MonoBehaviour) ((MonoBehaviour)comp).enabled = true;
                else if (comp is Collider) ((Collider)comp).enabled = true;
                else if (comp is Rigidbody) ((Rigidbody)comp).isKinematic = false;
            }
        }

        carController.enabled = false;
        carController.StopVehicle();

        impostorPlayer.gameObject.SetActive(false);

        playerCamera.enabled = true;
        vehicleCamera.enabled = false;

        player.SetActive(true);
        player.transform.SetParent(null);
        
        player.transform.localPosition = new Vector3(seatPosition.position.x + 2f, seatPosition.position.y, seatPosition.position.z);
        player.transform.localRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        /*if (carController.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            StartCoroutine(ActivateRagdoll());
        }*/
    }

    private IEnumerator ActivateRagdoll()
    {
        playerAnimator.enabled = false;

        foreach (Rigidbody rb in player.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = false;
        foreach (Collider col in player.GetComponentsInChildren<Collider>()) col.enabled = true;
        

        yield return new WaitForSeconds(ragdollDuration);

        foreach (Rigidbody rb in player.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
        foreach (Collider col in player.GetComponentsInChildren<Collider>()) if (col != player.GetComponent<Collider>()) col.enabled = false;
        

        playerAnimator.enabled = true;
    }
}
