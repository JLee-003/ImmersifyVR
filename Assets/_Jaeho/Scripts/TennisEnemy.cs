using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisEnemy : MonoBehaviour
{
    [SerializeField] float hitForce = 3f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float missChance = 0.025f;
    [SerializeField] [Range(0f, 1f)] float hitRandomness = 0f; // 0 = no randomness, 1 = very random
    [SerializeField] float missOffsetRange = 1.5f; // How far off the enemy aims when missing
    [SerializeField] float stoppingDistance = 0.1f; // Stop moving when this close to target position
    private int totalHits = 0;

    [Header("Wall Bounce Settings")]
    [SerializeField] [Range(0f, 1f)] float bounceChance = 0f; // Chance to bounce off walls instead of hitting straight
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;

    public float moveSpeedMultiplier = 1f;


    [SerializeField] Vector2 minBoundaries;
    [SerializeField] Vector2 maxBoundaries;

    Transform player;

    GameObject ball;

    [SerializeField] AudioClip hitAudio;

    private bool willMissThisShot = false;
    private Vector2 missOffset = Vector2.zero;
    private bool ballApproaching = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();

        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    private void Update()
    {
        if (ball != null)
        {
            /*
            Vector3 moveDir = ball.transform.position - transform.position;
            moveDir.y = 0f;
            moveDir.Normalize();

            Vector3 newPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

            if (newPos.x > minBoundaries.x && newPos.x < maxBoundaries.x)
            {
                transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
            }

            if (newPos.z > minBoundaries.y && newPos.z < maxBoundaries.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, newPos.z);
            }
            */
            MoveToBall(ball);
        }
    }

    void HitBall()
    {
        moveSpeedMultiplier = 1f;
        
        // If we're missing this shot, don't hit the ball
        if (willMissThisShot)
        {
            Debug.Log("Enemy missed - moved to wrong position!");
            // State will reset naturally when ball moves away or stops
            return;
        }

        if (totalHits > 10)
        {
            totalHits = 0;
            missChance = 0.025f;
        }

        Vector3 dir;
        
        // Decide between straight shot and wall bounce
        if (Random.value < bounceChance)
        {
            // Wall bounce shot
            Vector3 targetPos = GetRandomPlayerTarget();
            
            // Decide between 1 bounce (2/3 chance) or 2 bounces (1/3 chance)
            int numBounces = Random.value < 0.6667f ? 1 : 2;
            
            dir = CalculateBounceDirection(targetPos, numBounces);
        }
        else
        {
            // Straight shot to player with randomness
            dir = player.position - transform.position;
            
            if (hitRandomness > 0f)
            {
                float maxOffset = hitRandomness * 2f;
                dir.x += Random.Range(-maxOffset, maxOffset);
                dir.y += Random.Range(-maxOffset, maxOffset);
            }
        }
        
        dir.Normalize();

        ball.GetComponent<ZeroGravProjectile>().SetVelocity(dir * hitForce);
        totalHits++;
        missChance += 0.025f;

        AudioSource.PlayClipAtPoint(hitAudio, transform.position, 0.5f);
    }

    void MoveToBall(GameObject ball)
    {

        // ------ jonny implementation -----
        // x and y can move, z is fixed. predict the coordinates that it will reach the z axis. then move the enemy there.
        // enemy has a speed (moveSpeed). if enemy can't reach it in time that's one way for the player to beat the enemy

        // calculate z location <--- can be optimized, no need to calculate every frame. instead can calculate once when the ball is hit
        Vector3 ballVelocity = ball.GetComponent<Rigidbody>().velocity;


        // if ball is moving away from enemy then dont move (enemy hit it or player is serving)
        if (ballVelocity.z < 0)
        {
            // Reset approach state but keep rally stats
            ballApproaching = false;
            willMissThisShot = false;
            missOffset = Vector2.zero;
            return;
        }

        // Reset rally when ball is essentially stopped (miss/score - rally ended)
        if (ballVelocity.magnitude < 0.1f)
        {
            ballApproaching = false;
            willMissThisShot = false;
            missOffset = Vector2.zero;
            totalHits = 0;
            missChance = 0.025f;
            return;
        }

        // Ball just started approaching - decide if we'll miss this shot
        if (!ballApproaching)
        {
            ballApproaching = true;
            
            // Check if we should miss this shot
            if (totalHits > 10)
            {
                totalHits = 0;
                missChance = 0.025f;
            }
            
            if (Random.value < missChance)
            {
                willMissThisShot = true;
                // Generate random offset for the miss
                missOffset = new Vector2(
                    Random.Range(-missOffsetRange, missOffsetRange),
                    Random.Range(-missOffsetRange, missOffsetRange)
                );
                Debug.Log($"Enemy will miss this shot! Offset: {missOffset}");
            }
            else
            {
                willMissThisShot = false;
                missOffset = Vector2.zero;
            }
        }


        // key physics formula: df = v*t + di since acceleration is 0. time is constant across because its the same object moving
        float timeToZ = Mathf.Abs(ball.transform.position.z - transform.position.z) / ballVelocity.z;
        float predictedX = timeToZ * ballVelocity.x + ball.transform.position.x;
        float predictedY = timeToZ * ballVelocity.y + ball.transform.position.y;

        // Apply miss offset if we're going to miss
        if (willMissThisShot)
        {
            predictedX += missOffset.x;
            predictedY += missOffset.y;
        }

        Vector3 targetPosition = new Vector3(predictedX, predictedY, transform.position.z);
        Vector3 currentPosition = transform.position;
        
        // Check if we're already close enough to the target - if so, stop moving to prevent jitter
        float distanceToTarget = Vector3.Distance(new Vector3(currentPosition.x, currentPosition.y, 0), 
                                                   new Vector3(targetPosition.x, targetPosition.y, 0));
        
        if (distanceToTarget <= stoppingDistance)
        {
            return; // Already at target position
        }

        Vector3 moveDir = targetPosition - currentPosition;
        moveDir.Normalize();
        
        Vector3 newPos = transform.position + moveDir * moveSpeed * moveSpeedMultiplier * Time.deltaTime;

        float clampedX = Mathf.Clamp(newPos.x, minBoundaries.x, maxBoundaries.x);
        float clampedY = Mathf.Clamp(newPos.y, minBoundaries.y, maxBoundaries.y);

        // Update the enemy position using the clamped values
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);




    }

    Vector3 GetRandomPlayerTarget()
    {
        Vector3 targetPos = player.position;
        
        if (hitRandomness > 0f)
        {
            float maxOffset = hitRandomness * 2f;
            targetPos.x += Random.Range(-maxOffset, maxOffset);
            targetPos.y += Random.Range(-maxOffset, maxOffset);
        }
        
        return targetPos;
    }

    Vector3 CalculateBounceDirection(Vector3 targetPos, int numBounces)
    {
        if (numBounces == 1)
        {
            return CalculateSingleBounce(targetPos);
        }
        else if (numBounces == 2)
        {
            return CalculateDoubleBounce(targetPos);
        }
        
        return (targetPos - transform.position).normalized;
    }

    Vector3 CalculateSingleBounce(Vector3 targetPos)
    {
        GameObject[] walls = { leftWall, rightWall, topWall, bottomWall };
        GameObject selectedWall = walls[Random.Range(0, walls.Length)];
        
        if (selectedWall == null)
        {
            return (targetPos - transform.position).normalized;
        }
        
        Vector3 wallPos = selectedWall.transform.position;
        Vector3 wallNormal = GetWallNormal(selectedWall);
        
        Vector3 desiredOutgoing = (targetPos - wallPos).normalized;
        Vector3 requiredIncoming = Vector3.Reflect(desiredOutgoing, -wallNormal).normalized;
        
        Vector3 bouncePoint = CalculateBouncePoint(transform.position, requiredIncoming, selectedWall, wallNormal);
        
        Vector3 finalDir = (bouncePoint - transform.position).normalized;
        
        return finalDir;
    }

    Vector3 CalculateDoubleBounce(Vector3 targetPos)
    {
        GameObject[] walls = { leftWall, rightWall, topWall, bottomWall };
        GameObject wall1 = walls[Random.Range(0, walls.Length)];
        GameObject wall2 = walls[Random.Range(0, walls.Length)];
        
        if (wall1 == null || wall2 == null)
        {
            return (targetPos - transform.position).normalized;
        }
        
        Vector3 wall2Pos = wall2.transform.position;
        Vector3 wall2Normal = GetWallNormal(wall2);
        
        Vector3 desiredFinalOutgoing = (targetPos - wall2Pos).normalized;
        Vector3 toWall2 = Vector3.Reflect(desiredFinalOutgoing, -wall2Normal).normalized;
        
        Vector3 wall1Pos = wall1.transform.position;
        Vector3 wall1Normal = GetWallNormal(wall1);
        
        Vector3 bounce2Point = CalculateBouncePoint(wall1Pos, toWall2, wall2, wall2Normal);
        Vector3 fromWall1ToWall2 = (bounce2Point - wall1Pos).normalized;
        Vector3 toWall1 = Vector3.Reflect(fromWall1ToWall2, -wall1Normal).normalized;
        
        Vector3 bounce1Point = CalculateBouncePoint(transform.position, toWall1, wall1, wall1Normal);
        
        Vector3 finalDir = (bounce1Point - transform.position).normalized;
        
        return finalDir;
    }

    Vector3 GetWallNormal(GameObject wall)
    {
        if (wall == leftWall) return Vector3.right;
        if (wall == rightWall) return Vector3.left;
        if (wall == topWall) return Vector3.down;
        if (wall == bottomWall) return Vector3.up;
        return Vector3.forward;
    }

    Vector3 CalculateBouncePoint(Vector3 startPos, Vector3 direction, GameObject wall, Vector3 wallNormal)
    {
        Vector3 wallPos = wall.transform.position;
        
        if (wall == leftWall || wall == rightWall)
        {
            float wallX = wallPos.x;
            if (Mathf.Abs(direction.x) < 0.001f) return wallPos;
            
            float t = (wallX - startPos.x) / direction.x;
            float hitY = startPos.y + direction.y * t;
            float hitZ = startPos.z + direction.z * t;
            return new Vector3(wallX, hitY, hitZ);
        }
        else
        {
            float wallY = wallPos.y;
            if (Mathf.Abs(direction.y) < 0.001f) return wallPos;
            
            float t = (wallY - startPos.y) / direction.y;
            float hitX = startPos.x + direction.x * t;
            float hitZ = startPos.z + direction.z * t;
            return new Vector3(hitX, wallY, hitZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ball)
        {
            HitBall();
        }
    }
}
