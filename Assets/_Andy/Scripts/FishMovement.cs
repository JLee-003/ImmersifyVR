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
        if(rb.velocity.magnitude <= 0.8f || timer <= 0)
        {
            Move();
        }

        if(transform.position.y >= maxYLevel)
        {
            var curPos = transform.position;
            curPos.y = maxYLevel;
            transform.position = curPos;

            var curVel = rb.velocity;
            if(curVel.y > 0) curVel.y = 0;
            rb.velocity = curVel;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime * smoothTime);
    }

    private void Move()
    {
        float randForce = Random.Range(minForce, maxForce);
        var playerDir = player.position - transform.position;
        if (playerDir.sqrMagnitude <= followDistance * followDistance)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
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

}
