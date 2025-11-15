using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HandBoosters : MonoBehaviour
{
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] Transform leftController;
    [SerializeField] Transform rightController;
    [SerializeField] Transform cameraTransform;
    [SerializeField] AudioClip thrusterAudio;

    // Scene gating (explicit allowlist)
    [Tooltip("Thrusters are enabled ONLY in scenes listed here (exact names).")]
    [SerializeField] string[] enabledInScenes = new string[0];

    float maxVelocity = 4f;
    float boostAddMin = 3f;
    float boostAddMax = 6f;
    float boostDrag = 0.1f;

    float leftBoostForce;
    float rightBoostForce;

    private CharacterController characterController;
    private ActionBasedContinuousMoveProvider continuousMoveProvider;

    private AudioSource leftThrusterSource;
    private AudioSource rightThrusterSource;

    Vector3 velocity;

    bool _sceneAllowed = false;

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

        // Evaluate scene on startup and hook future loads
        EvaluateSceneGate();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Re-check when scenes change
    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        EvaluateSceneGate();
    }

    // Allow only listed scenes
    void EvaluateSceneGate()
    {
        if (enabledInScenes == null || enabledInScenes.Length == 0)
        {
            _sceneAllowed = false;
        }
        else
        {
            string current = SceneManager.GetActiveScene().name;
            _sceneAllowed = System.Array.IndexOf(enabledInScenes, current) >= 0;
        }

        if (!_sceneAllowed)
        {
            if (leftThrusterSource && leftThrusterSource.isPlaying) leftThrusterSource.Stop();
            if (rightThrusterSource && rightThrusterSource.isPlaying) rightThrusterSource.Stop();
            velocity = Vector3.zero;
        }
    }

    void Update()
    {
        // Not an allowed scene? Do nothing
        if (!_sceneAllowed) return;

        continuousMoveProvider.useGravity = false;
        continuousMoveProvider.moveSpeed = 0f;

        Debug.Log("HAND BOOSTERS IS ON");

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
        {
            // frame-rate independent damping, exponential
            float dragFactor = Mathf.Pow(boostDrag, Time.deltaTime);
            velocity *= dragFactor;

            // snap to zero when very small
            /*if (velocity.sqrMagnitude < 0.0001f)
                velocity = Vector3.zero;*/
        }

        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleThrusterAudio(AudioSource source, bool isBoosting, Transform hand)
    {
        if (isBoosting)
        {
            if (!source.isPlaying)
            {
                source.transform.position = hand.position;
                source.Play();
            }
            else
            {
                source.transform.position = hand.position;
            }
        }
        else if (source.isPlaying)
        {
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
