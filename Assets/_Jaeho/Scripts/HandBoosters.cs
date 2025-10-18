using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandBoosters : MonoBehaviour
{
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;
    [SerializeField] Transform cameraTransform;

    float maxVelocity = 2f;
    float boostAddMin = 0.7f;
    float boostAddMax = 1f;
    float boostDrag = 0.9f;

    float leftBoostForce;
    float rightBoostForce;

    private CharacterController characterController;

    ActionBasedContinuousMoveProvider continuousMoveProvider;

    Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
    }
    void Update()
    {
        //Disable gravity for testing
        continuousMoveProvider.useGravity = false;

        bool leftBoosting = leftControllerSwimReference.action.IsPressed();
        bool rightBoosting = rightControllerSwimReference.action.IsPressed();

        if (leftBoosting)
        {
            AddLeftBoost();
        }

        if (rightBoosting)
        {
            AddRightBoost();
        }

        if (!leftBoosting && !rightBoosting)
        {
            velocity -= velocity * boostDrag * Time.deltaTime; //Simulate drag for gradual deceleration
        }

        velocity = Vector3.ClampMagnitude(velocity, maxVelocity); //Cap velocity

        characterController.Move(velocity * Time.deltaTime);
    }

    void AddLeftBoost()
    {
        Vector3 boostDirection = cameraTransform.position - leftController.position;
        boostDirection.Normalize();

        float dot = -Vector3.Dot(leftController.forward, boostDirection);

        leftBoostForce = Mathf.Lerp(boostAddMin, boostAddMax, (dot + 1) / 2);

        velocity += boostDirection * leftBoostForce * Time.deltaTime;

        HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, false, 0.4f, 0.1f);
    }

    void AddRightBoost()
    {
        Vector3 boostDirection = cameraTransform.position - rightController.position;
        boostDirection.Normalize();

        float dot = -Vector3.Dot(rightController.forward, boostDirection);

        rightBoostForce = Mathf.Lerp(boostAddMin, boostAddMax, (dot + 1) / 2);

        velocity += boostDirection * rightBoostForce * Time.deltaTime;

        HapticFeedbackManager.Instance?.InitiateHapticFeedback(false, true, 0.4f, 0.1f);
    }
}
