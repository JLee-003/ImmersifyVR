using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class LineSwimmer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float swimForce = 15f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce = 0.1f;
    [SerializeField] float minTimeBetweenStrokes = 1f;

    [Header("References")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] Transform trackingReference;
    [SerializeField] Transform leftControllerTransform;
    [SerializeField] Transform rightControllerTransform;

    [Header("Line Renderers")]
    [SerializeField] LineRenderer leftLineRenderer;
    [SerializeField] LineRenderer rightLineRenderer;

    [Header("Audio Clips")]
    [SerializeField] AudioClip swimAudio;

    CharacterController characterController;
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

        SetupLineRenderer(leftLineRenderer);
        SetupLineRenderer(rightLineRenderer);
    }

    void Update()
    {
        LeftControllerSwim();
        RightControllerSwim();

        UpdateStrokeLines();

        // Apply drag force
        if (velocity.sqrMagnitude > 0.01f)
        {
            velocity -= velocity * dragForce * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    void LeftControllerSwim()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer > minTimeBetweenStrokes && leftControllerSwimReference.action.IsPressed())
        {
            if (!leftSwimStarted)
            {
                startPosLeft = leftControllerTransform.localPosition;
            }

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, false, 0.5f, 0.1f);
            leftSwimStarted = true;
        }

        if (leftSwimStarted && leftControllerSwimReference.action.WasReleasedThisFrame())
        {
            endPosLeft = leftControllerTransform.localPosition;
            leftActionVector = startPosLeft - endPosLeft;

            Vector3 worldVelocity = trackingReference.TransformDirection(leftActionVector);
            velocity += worldVelocity * swimForce;

            cooldownTimer = 0f;

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, false, 1.0f, 0.4f);
            AudioSource.PlayClipAtPoint(swimAudio, transform.position, 1f);
            
            leftSwimStarted = false;
        }
    }

    void RightControllerSwim()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer > minTimeBetweenStrokes && rightControllerSwimReference.action.IsPressed())
        {
            if (!rightSwimStarted)
            {
                startPosRight = rightControllerTransform.localPosition;
            }

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(false, true, 0.5f, 0.1f);
            rightSwimStarted = true;
        }

        if (rightSwimStarted && rightControllerSwimReference.action.WasReleasedThisFrame())
        {
            endPosRight = rightControllerTransform.localPosition;
            rightActionVector = startPosRight - endPosRight;

            Vector3 worldVelocity = trackingReference.TransformDirection(rightActionVector);
            velocity += worldVelocity * swimForce;

            cooldownTimer = 0f;

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(false, true, 1.0f, 0.4f);
            AudioSource.PlayClipAtPoint(swimAudio, transform.position, 1f);

            rightSwimStarted = false;
        }
    }

    void UpdateStrokeLines()
    {
        if (leftSwimStarted)
        {
            leftLineRenderer.enabled = true;
            leftLineRenderer.positionCount = 2;
            leftLineRenderer.SetPosition(0, leftControllerTransform.parent.TransformPoint(startPosLeft));
            leftLineRenderer.SetPosition(1, leftControllerTransform.position);
        }
        else
        {
            leftLineRenderer.enabled = false;
        }

        if (rightSwimStarted)
        {
            rightLineRenderer.enabled = true;
            rightLineRenderer.positionCount = 2;
            rightLineRenderer.SetPosition(0, rightControllerTransform.parent.TransformPoint(startPosRight));
            rightLineRenderer.SetPosition(1, rightControllerTransform.position);
        }
        else
        {
            rightLineRenderer.enabled = false;
        }
    }

    void SetupLineRenderer(LineRenderer lr)
    {
        if (lr == null) return;

        lr.useWorldSpace = true;
        lr.enabled = false;
        lr.positionCount = 0;
        lr.widthMultiplier = 0.01f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0f, 0.8f, 1f); // light cyan
        lr.endColor = new Color(0f, 0f, 1f);   // dark blue
    }

}