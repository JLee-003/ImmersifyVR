using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class LineSwimmer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float swimForce = 15f;
    [SerializeField] float dragForce = 0.9f;
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

    [Header("Turning")]
    [SerializeField] float turnForce = 120f;
    [SerializeField] float turnDrag = 4f;
    [SerializeField] float minTurnStroke = 0.05f;
    [SerializeField] float maxTurnPerStroke = 90f;

    private bool tutorialMode = false;

    float yawVelocity;

    CharacterController characterController;
    float cooldownTimer;
    public Vector3 velocity;
    float totalDistanceSwum = 0f;

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

        ApplyTurning();

        // Apply drag force
        if (velocity.sqrMagnitude > 0.01f)
        {
            velocity -= velocity * dragForce * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }

        velocity += new Vector3(0f, -0.005f, 0f) * Time.deltaTime;
        Vector3 movement = velocity * Time.deltaTime;
        characterController.Move(movement);
        
        // Track total distance swum
        totalDistanceSwum += movement.magnitude;

        if (characterController.velocity.sqrMagnitude < 0.01f)
        {
            velocity = Vector3.zero;
        }
    }

    void OnDisable()
    {
        velocity = Vector3.zero;
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

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, false, 0.3f, 0.1f);
            leftSwimStarted = true;
        }

        if (leftSwimStarted && leftControllerSwimReference.action.WasReleasedThisFrame())
        {
            if (!tutorialMode)
            {
                endPosLeft = leftControllerTransform.localPosition;
                leftActionVector = startPosLeft - endPosLeft;

                Vector3 worldVelocity = trackingReference.TransformDirection(leftActionVector);
                velocity += worldVelocity * swimForce;
                AddTurnFromStroke(leftActionVector);

                cooldownTimer = 0f;

                HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, false, 0.3f, 0.2f);
                AudioSource.PlayClipAtPoint(swimAudio, transform.position, 1f);
            }
            
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

            HapticFeedbackManager.Instance?.InitiateHapticFeedback(false, true, 0.3f, 0.1f);
            rightSwimStarted = true;
        }

        if (rightSwimStarted && rightControllerSwimReference.action.WasReleasedThisFrame())
        {
            if (!tutorialMode)
            {
                endPosRight = rightControllerTransform.localPosition;
                rightActionVector = startPosRight - endPosRight;

                Vector3 worldVelocity = trackingReference.TransformDirection(rightActionVector);
                velocity += worldVelocity * swimForce;
                AddTurnFromStroke(rightActionVector);

                cooldownTimer = 0f;

                HapticFeedbackManager.Instance?.InitiateHapticFeedback(false, true, 0.4f, 0.2f);
                AudioSource.PlayClipAtPoint(swimAudio, transform.position, 1f);
            }

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

    void AddTurnFromStroke(Vector3 localActionVector)
    {
        float sideways = localActionVector.x;

        if (Mathf.Abs(sideways) < minTurnStroke)
            return;

        float turnAmount = sideways * turnForce;
        turnAmount = Mathf.Clamp(turnAmount, -maxTurnPerStroke, maxTurnPerStroke);

        yawVelocity += turnAmount;
    }

    void ApplyTurning()
    {
        if (Mathf.Abs(yawVelocity) > 0.01f)
        {
            transform.Rotate(0f, yawVelocity * Time.deltaTime, 0f);
            yawVelocity = Mathf.Lerp(yawVelocity, 0f, turnDrag * Time.deltaTime);
        }
        else
        {
            yawVelocity = 0f;
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

    // Public method to get total distance swum
    public float GetTotalDistanceSwum()
    {
        return totalDistanceSwum;
    }

    // Public method to reset distance counter
    public void ResetDistanceSwum()
    {
        totalDistanceSwum = 0f;
    }
    public void SetTutorialMode(bool enabled)
    {
        tutorialMode = enabled;

        if (enabled)
        {
            leftSwimStarted = false;
            rightSwimStarted = false;
        }
    }

    public void TriggerTutorialBoost(float boostForce, float upwardBoost)
    {
        Vector3 boostDirection = trackingReference.forward;
        boostDirection.y = 0f;
        boostDirection.Normalize();

        velocity += boostDirection * boostForce;
        velocity += Vector3.up * upwardBoost;

        cooldownTimer = 0f;

        leftSwimStarted = false;
        rightSwimStarted = false;

        HapticFeedbackManager.Instance?.InitiateHapticFeedback(true, true, 0.4f, 0.15f);

        if (swimAudio != null)
        {
            AudioSource.PlayClipAtPoint(swimAudio, transform.position, 1f);
        }
    }
}