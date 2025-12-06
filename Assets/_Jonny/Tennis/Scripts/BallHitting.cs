using System.Collections;
using UnityEngine;

public class BallHitting : MonoBehaviour
{
    [Tooltip("Multiplier to scale the power of the hit.")]
    public float powerMultiplier = 1.5f;

    public float swingThreshold = 2.0f;
    public float pushbackForce = 2.0f;

    public Transform xrOrigin;

    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    [Header("Hit Constraints")]
    [Tooltip("Min/max outgoing speed")]
    public float minHitSpeed = 2f;
    public float maxHitSpeed = 15f;

    [Tooltip("Max angle from the hand’s forward for horizontal deflection (unused)")]
    public float maxHorizontalAngle = 30f;

    [Tooltip("Max angle above the horizontal plane (unused)")]
    public float maxVerticalAngle = 0f;

    [SerializeField] AudioClip hitAudio;

    // NEW: ensure a minimum Z speed (forward/back) only
    [Header("Z-Axis Safeguard")]
    [Tooltip("Absolute minimum |Z| speed after a hit. Sign (+/-) is preserved from the swing.")]
    public float minZSpeed = 3f;

    bool leftHaptic = true;
    bool rightHaptic = true;

    void Start()
    {
        previousPosition = transform.position;

        Transform parentHand = transform.parent;
        if (parentHand != null)
        {
            if (parentHand.CompareTag("LeftHand"))
            {
                leftHaptic = true;
                rightHaptic = false;
            }
            else if (parentHand.CompareTag("RightHand"))
            {
                leftHaptic = false;
                rightHaptic = true;
            }
            else
            {
                Debug.LogWarning($"BoosterParticles: Parent '{parentHand.name}' does not have LeftHand or RightHand tag!");
            }
        }
        else
        {
            Debug.LogWarning("BoosterParticles: No parent found!");
        }
    }

    void Update()
    {
        // Hand/controller velocity from position delta
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        if (CompareTag("LeftHand")) Debug.Log("This racket is from the left hand.");
        else if (CompareTag("RightHand")) Debug.Log("This racket is from the right hand.");

        ReturnBall(other.gameObject);
    }

    void ReturnBall(GameObject ball)
    {
        // Build outgoing velocity from the swing
        Vector3 rawDir = currentVelocity.sqrMagnitude > 1e-6f ? currentVelocity.normalized : transform.forward;
        float rawSpeed = currentVelocity.magnitude * powerMultiplier;
        float speed = Mathf.Clamp(rawSpeed, minHitSpeed, maxHitSpeed);

        Vector3 vel = rawDir * speed;

        // Apply to projectile
        ZeroGravProjectile projectile = ball.GetComponent<ZeroGravProjectile>();
        if (projectile != null)
        {
            projectile.ChangeVelocity(vel, true);
            if (hitAudio) AudioSource.PlayClipAtPoint(hitAudio, transform.position, 1f);
            HapticFeedbackManager.Instance.InitiateHapticFeedback(leftHaptic, rightHaptic, 1f, 0.5f);
        }
    }
}
