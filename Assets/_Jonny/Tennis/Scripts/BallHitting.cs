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

    void Start()
    {
        previousPosition = transform.position;
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

        // Enforce a minimum Z forward speed
        float absZ = Mathf.Abs(vel.z);
        if (absZ < minZSpeed)
        {
            // Preserve player's chosen Z sign if any; else fall back to transform.forward.z or +Z
            float zSign = Mathf.Sign(vel.z);
            if (Mathf.Abs(zSign) < 1e-6f)
            {
                float fz = transform.forward.z;
                zSign = Mathf.Abs(fz) > 1e-6f ? Mathf.Sign(fz) : 1f;
            }

            float desiredZ = zSign * minZSpeed;

            // Keep Z at desired magnitude, trim X/Y only if needed to respect maxHitSpeed
            float maxXY2 = Mathf.Max(0f, (maxHitSpeed * maxHitSpeed) - (desiredZ * desiredZ));
            float curXY2 = (vel.x * vel.x) + (vel.y * vel.y);

            if (curXY2 > maxXY2 && curXY2 > 1e-12f)
            {
                float scaleXY = Mathf.Sqrt(maxXY2 / curXY2);
                vel.x *= scaleXY;
                vel.y *= scaleXY;
            }

            vel.z = desiredZ;
        }

        // Apply to projectile
        ZeroGravProjectile projectile = ball.GetComponent<ZeroGravProjectile>();
        if (projectile != null)
        {
            projectile.ChangeVelocity(vel, true);
            if (hitAudio) AudioSource.PlayClipAtPoint(hitAudio, transform.position, 1f);
        }
    }
}
