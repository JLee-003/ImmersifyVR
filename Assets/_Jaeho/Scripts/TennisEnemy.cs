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

        Vector3 dir = player.position - transform.position;
        
        // Apply randomness to the hit direction
        if (hitRandomness > 0f)
        {
            // Add random offset to x and y components based on randomness
            // Higher randomness = larger random offset
            float maxOffset = hitRandomness * 2f; // Scale factor for randomness
            dir.x += Random.Range(-maxOffset, maxOffset);
            dir.y += Random.Range(-maxOffset, maxOffset);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ball)
        {
            HitBall();
        }
    }
}
