using UnityEngine;

public class BallHitting : MonoBehaviour
{
    [Tooltip("Multiplier to scale the power of the hit.")]
    public float powerMultiplier = 1.5f;

    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    [Header("Hit Constraints")]
    [Tooltip("Min/max outgoing speed")]
    public float minHitSpeed = 2f;
    public float maxHitSpeed = 15f;

    [Tooltip("Max angle from the hand’s forward for horizontal deflection")]
    public float maxHorizontalAngle = 30f;

    [Tooltip("Max angle above the horizontal plane")]
    public float maxVerticalAngle = 0f;

    [SerializeField]
    Transform enemy; //Change this


    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        // Calculate velocity of the hand based on position delta
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        // Raw from your hand’s velocity
        Vector3 rawDir = currentVelocity.normalized;
        float rawSpeed = currentVelocity.magnitude * powerMultiplier;

        float speed = Mathf.Clamp(rawSpeed, minHitSpeed, maxHitSpeed);

        // Clamp horizontal angle relative to enemyDir
        Vector3 enemyDir = enemy.position - transform.position;
        enemyDir.Normalize();

        float horizAngle = Vector3.Angle(enemyDir, rawDir);
        if (horizAngle > maxHorizontalAngle)
        {
            rawDir = Vector3.RotateTowards(enemyDir, rawDir, maxHorizontalAngle * Mathf.Deg2Rad, 0f);
        }

        // Clamp vertical angle relative to horizontal plane
        Vector3 flatDir = new Vector3(rawDir.x, 0f, rawDir.z).normalized;
        float vertAngle = Vector3.Angle(rawDir, flatDir);
        if (vertAngle > maxVerticalAngle)
        {
            rawDir = Vector3.RotateTowards(flatDir, rawDir, maxVerticalAngle * Mathf.Deg2Rad, 0f);
        }

        ZeroGravProjectile projectile = other.GetComponent<ZeroGravProjectile>();
        if (projectile != null)
        {
            projectile.ChangeVelocity(rawDir * speed);
        }
    }
}
