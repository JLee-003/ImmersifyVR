using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisEnemy : MonoBehaviour
{
    [SerializeField] float hitForce = 1f;
    [SerializeField] float moveSpeed = 7.5f;

    [SerializeField] Vector2 minBoundaries;
    [SerializeField] Vector2 maxBoundaries;

    Transform player;

    GameObject ball;

    private void Start()
    {
        player = PlayerReferences.instance.playerObject.transform;

        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    private void Update()
    {
        if (ball != null)
        {
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
        }
    }

    void HitBall()
    {
        Vector3 dir = player.position - transform.position;
        dir.Normalize();

        ball.GetComponent<ZeroGravProjectile>().ChangeVelocity(dir * hitForce);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ball)
        {
            HitBall();
        }
    }
}
