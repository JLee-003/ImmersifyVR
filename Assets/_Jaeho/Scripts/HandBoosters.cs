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
    [SerializeField] AudioClip thrusterAudio;

    float maxVelocity = 3f;
    float boostAddMin = 0.75f;
    float boostAddMax = 1.25f;
    float boostDrag = 0.99f;

    float leftBoostForce;
    float rightBoostForce;

    private CharacterController characterController;
    private ActionBasedContinuousMoveProvider continuousMoveProvider;

    private AudioSource leftThrusterSource;
    private AudioSource rightThrusterSource;

    Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();

        // Create looping AudioSources for each hand
        leftThrusterSource = gameObject.AddComponent<AudioSource>();
        rightThrusterSource = gameObject.AddComponent<AudioSource>();

        leftThrusterSource.clip = thrusterAudio;
        rightThrusterSource.clip = thrusterAudio;

        leftThrusterSource.loop = true;
        rightThrusterSource.loop = true;

        leftThrusterSource.playOnAwake = false;
        rightThrusterSource.playOnAwake = false;
    }

    void Update()
    {
        continuousMoveProvider.useGravity = false;

        bool leftBoosting = leftControllerSwimReference.action.IsPressed();
        bool rightBoosting = rightControllerSwimReference.action.IsPressed();

        // Play or stop looping audio based on grip input
        HandleThrusterAudio(leftThrusterSource, leftBoosting, leftController);
        HandleThrusterAudio(rightThrusterSource, rightBoosting, rightController);

        if (leftBoosting)
            AddLeftBoost();

        if (rightBoosting)
            AddRightBoost();

        if (!leftBoosting && !rightBoosting)
            velocity -= velocity * boostDrag * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleThrusterAudio(AudioSource source, bool isBoosting, Transform hand)
    {
        if (isBoosting)
        {
            // Start playing if not already
            if (!source.isPlaying)
            {
                source.transform.position = hand.position;
                source.Play();
            }
            else
            {
                // Update position while playing (so it follows hand)
                source.transform.position = hand.position;
            }
        }
        else if (source.isPlaying)
        {
            // Stop sound smoothly
            source.Stop();
        }
    }

    void AddLeftBoost()
    {
        Vector3 boostDirection = cameraTransform.position - leftController.position;
        boostDirection.Normalize();

        Debug.Log(leftController.localPosition);

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
