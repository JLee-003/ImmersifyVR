using UnityEngine;

public class BallControllerMirror : MonoBehaviour
{

    public string controllerTag = "LeftHand";


    [Tooltip("How strongly the ball follows the controller movement")]
    public float movementMultiplier = 10.0f;

    [Tooltip("Smooth movement")]
    public float smoothFactor = 5f;

    private Transform controller;
    private Vector3 previousControllerPosition;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Try to find the controller by tag
        GameObject controllerObj = GameObject.FindGameObjectWithTag(controllerTag);
        controller = controllerObj.transform;
        previousControllerPosition = controller.localPosition;
    }

    
    void FixedUpdate()
    {
        if (controller == null || rb == null) return;

        Vector3 controllerDelta = controller.localPosition - previousControllerPosition;
        if (controllerDelta.sqrMagnitude <= 0.001) return;

        // Apply the controller's delta movement to the ball
        Vector3 targetVelocity = controllerDelta / Time.fixedDeltaTime * movementMultiplier;

        // Smooth velocity application
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * smoothFactor);

        previousControllerPosition = controller.localPosition;
    }
}
