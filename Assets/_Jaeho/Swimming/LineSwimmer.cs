using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class LineSwimmer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float swimForce = 100f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce = 0.1f;
    [SerializeField] float minTimeBetweenStrokes = 1f;

    [Header("References")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] Transform trackingReference;
    [SerializeField] Transform leftControllerTransform; // Reference to the left controller's transform
    [SerializeField] Transform rightControllerTransform; // Reference to the right controller's transform

    CharacterController characterController;
    LineRenderer lineRenderer;

    float cooldownTimer;
    Vector3 velocity;

    Vector3 startPosLeft;
    Vector3 endPosLeft;
    Vector3 leftActionVector;
    bool leftSwimStarted = false;

    Vector3 startPosRight;
    Vector3 endPosRight;
    Vector3 rightActionVector;
    bool rightSwimStarted = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }
    private void Update()
    {
        LeftControllerSwim();
        RightControllerSwim();

        UpdateStrokeLine();

        // Apply drag force
        if (velocity.sqrMagnitude > 0.01f)
        {
            velocity -= velocity * dragForce * Time.deltaTime;
        }

        else
        {
            velocity = Vector3.zero;
        }

        // Move the character controller
        characterController.Move(velocity * Time.deltaTime);
    }
    void LeftControllerSwim()
    {
        cooldownTimer += Time.deltaTime;

        //Left Grip Button Pressed
        if (cooldownTimer > minTimeBetweenStrokes
            && leftControllerSwimReference.action.IsPressed())
        {
            if (!leftSwimStarted)
            {
                startPosLeft = leftControllerTransform.localPosition;
            }

            //Swimming haptics
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, false, 0.1f, 0.1f);

            leftSwimStarted = true;
        }

        if (leftSwimStarted && leftControllerSwimReference.action.WasReleasedThisFrame())
        {
            endPosLeft = leftControllerTransform.localPosition;

            leftActionVector = startPosLeft - endPosLeft;

            // Perform force
            Vector3 worldVelocity = trackingReference.TransformDirection(leftActionVector);
            velocity += worldVelocity * swimForce;

            cooldownTimer = 0f;
            //Swimming haptics
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, false, 0.25f, 0.4f);
            leftSwimStarted = false;
        }
    }
    void RightControllerSwim()
    {
        cooldownTimer += Time.deltaTime;

        //Right Grip Button Pressed
        if (cooldownTimer > minTimeBetweenStrokes
            && rightControllerSwimReference.action.IsPressed())
        {
            if (!rightSwimStarted)
            {
                startPosRight = rightControllerTransform.localPosition;
            }

            //Swimming haptics
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, false, 0.1f, 0.1f);

            rightSwimStarted = true;
        }

        if (rightSwimStarted && rightControllerSwimReference.action.WasReleasedThisFrame())
        {
            endPosRight = rightControllerTransform.localPosition;

            rightActionVector = startPosRight - endPosRight;

            // Perform force
            Vector3 worldVelocity = trackingReference.TransformDirection(rightActionVector);
            velocity += worldVelocity * swimForce;

            cooldownTimer = 0f;
            //Swimming haptics
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, false, 0.25f, 0.4f);
            rightSwimStarted = false;
        }
    }
    void UpdateStrokeLine()
    {
        // Left stroke active = left controller line
        if (leftSwimStarted)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, leftControllerTransform.parent.TransformPoint(startPosLeft));
            lineRenderer.SetPosition(1, leftControllerTransform.position);
        }

        // Right stroke active = Right controller line
        else if (rightSwimStarted)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, rightControllerTransform.parent.TransformPoint(startPosRight));
            lineRenderer.SetPosition(1, rightControllerTransform.position);
        }

        // No stroke active = No line
        else
        {
            lineRenderer.enabled = false;
        }
    }
}

