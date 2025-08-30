using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private float maxYLevel;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;

    [SerializeField] private float maxTime;
    [SerializeField] private float smoothTime;

    [SerializeField] private float followDistance;

    private float randRange = 0.3f;

    private Transform player;
    private Rigidbody rb;
    private float timer;

    private void Awake()
    {
        player = PlayerReferences.instance.playerObject.transform;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (rb.velocity.magnitude <= 0.8f || timer <= 0)
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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(rb.velocity),
                Time.deltaTime * smoothTime
            );
        }
    }

    private void Move()
    {
        float randForce = Random.Range(minForce, maxForce);
        var playerDir = player.position - transform.position;
        if (playerDir.sqrMagnitude <= followDistance * followDistance)
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
        timer = maxTime;
    }

    private void ApplyDifficulty()
    {
        float level = FishGame.Instance.difficultyLevel;

        randRange = Mathf.Lerp(0.05f, 0.6f, level);        // small → easy, large → hard
        followDistance = Mathf.Lerp(20f, 5f, level);       // large → easy, small → hard
    }

}
