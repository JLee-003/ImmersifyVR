using UnityEngine;

public class FixedBelowView : MonoBehaviour
{
    private Transform xrCamera; 
    public float heightOffset = -1.0f;  // Distance below the player

    void Awake()
    {
        xrCamera = PlayerReferences.instance.cameraTransform;
    }
    void Update()
    {
        if (xrCamera != null)
        {
            // Keep the UI below the player, but ignore head tilt
            Vector3 targetPosition = xrCamera.position + Vector3.down * Mathf.Abs(heightOffset);
            transform.position = targetPosition;
        }
        else
        {
            Debug.Log("no camera detected");
        }
    }
} 