using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float maxYLevel;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;

    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private float smoothTime;

    [SerializeField] private float followDistance;

    [SerializeField] float nearPlayerHoldTime = 0.8f;
    [SerializeField] float nearPlayerRadius = 2.5f;
    [SerializeField] float farAwayFromPlayerRadius = 20f;
    [SerializeField] float farAwayFromPlayerMinForce;
    [SerializeField] float farAwayFromPlayerMaxForce;

    private float holdTimer;

    private float randRange = 0.3f;

    private Transform player;
    private Rigidbody rb;
    private float timer;
    private Fish fishScript;

    private void Awake()
    {
        player = PlayerReferences.instance.playerObject.transform;
        rb = GetComponent<Rigidbody>();
        fishScript = GetComponent<Fish>();
    }

    private void Update()
    {
        float distSqr = (player.position - transform.position).sqrMagnitude;

        if (distSqr <= nearPlayerRadius * nearPlayerRadius)
        {
            holdTimer = nearPlayerHoldTime; // keep refreshing while you're close
        }
        else
        {
            holdTimer = Mathf.Max(0f, holdTimer - Time.deltaTime);
        }

        timer -= Time.deltaTime;
        if (timer <= 0 && holdTimer <= 0f)
        {
            Move();
        }


        if (transform.position.y >= maxYLevel)
        {
            var curPos = transform.position;
            curPos.y = maxYLevel;
            transform.position = curPos;

            var curVel = rb.velocity;
            if (curVel.y > 0) curVel.y = 0;
            rb.velocity = curVel;
        }

        if (rb.velocity.sqrMagnitude > 0.0001f)
        {
            // Get the movement direction offset from the Fish script
            Vector3 movementDirectionOffset = Vector3.zero;
            if (fishScript != null && fishScript.meshes != null && fishScript.type > 0 && fishScript.type <= fishScript.meshes.Length)
            {
                movementDirectionOffset = fishScript.meshes[fishScript.type - 1].movementDirection;
            }
            
            // Apply the movement direction offset when calculating rotation
            Quaternion baseRotation = Quaternion.LookRotation(rb.velocity);
            Quaternion offsetRotation = Quaternion.Euler(movementDirectionOffset);
            Quaternion targetRotation = baseRotation * offsetRotation;
            
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * smoothTime
            );
        }
    }

    private void Move()
    {
        var playerDir = player.position - transform.position;
        float distSqr = playerDir.sqrMagnitude;
        
        // Use farAwayFromPlayerMinForce/MaxForce when fish is far from player, otherwise use random force
        float randForce = distSqr > farAwayFromPlayerRadius * farAwayFromPlayerRadius 
            ? Random.Range(farAwayFromPlayerMinForce, farAwayFromPlayerMaxForce)
            : Random.Range(minForce, maxForce);
        
        if (distSqr <= followDistance * followDistance)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1*randRange, randRange), Random.Range(-1*randRange, randRange), Random.Range(-1*randRange, randRange));
            var offsetDir = playerDir.normalized + randomOffset;
            rb.AddForce(offsetDir.normalized * randForce, ForceMode.Impulse);
        }
        else
        {
            var dir = Random.insideUnitSphere;
            rb.AddForce(dir * randForce, ForceMode.Impulse);
        }
        timer = Random.Range(minTime, maxTime);
    }

    private void ApplyDifficulty()
    {
        float level = FishGame.Instance.difficultyLevel;

        randRange = Mathf.Lerp(0.05f, 0.6f, level);        // small → easy, large → hard
        followDistance = Mathf.Lerp(20f, 5f, level);       // large → easy, small → hard
    }

}
