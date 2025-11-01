using UnityEngine;

public class FixedBelowView : MonoBehaviour // actually fixed IN FRONT OF THE PLAYER, not BELOW the player.
{
    private Transform xrCamera; 
    public float distanceFromCamera = 2.0f;  // Distance in front of the camera
    public bool faceCamera = true;  // Whether to rotate to face the camera

    void Awake()
    {
        xrCamera = PlayerReferences.instance.cameraTransform;
    }
    void Update()
    {
        if (xrCamera != null)
        {
            // Position the UI in front of the camera
            Vector3 targetPosition = xrCamera.position + xrCamera.forward * distanceFromCamera;
            transform.position = targetPosition;

            // Optionally rotate to face the camera
            if (faceCamera)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - xrCamera.position);
            }
        }
        else
        {
            Debug.Log("no camera detected");
        }
    }
} 