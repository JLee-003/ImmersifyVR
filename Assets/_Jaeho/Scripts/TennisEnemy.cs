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
        Vector3 dir = player.position - transform.position;
        dir.Normalize();

        ball.GetComponent<ZeroGravProjectile>().ChangeVelocity(dir * hitForce);
    }

    void MoveToBall(GameObject ball)
    {
        // ------ jonny implementation -----
        // x and y can move, z is fixed. predict the coordinates that it will reach the z axis. then move the enemy there.
        // enemy has a speed (moveSpeed). if enemy can't reach it in time that's one way for the player to beat the enemy

        // calculate z location <--- can be optimized, no need to calculate every frame. instead can calculate once when the ball is hit
        Vector3 ballVelocity = ball.GetComponent<Rigidbody>().velocity;
        // is there someting weird that can happen when the ball is rotated so the z isn't actually z?

        // key physics formula: df = v*t + di since acceleration is 0. time is constant across because its the same object moving
        float timeToZ = (ball.transform.position.z - transform.position.z) / ballVelocity.z;
        float predictedX = timeToZ * ballVelocity.x + ball.transform.position.x;
        float predictedY = timeToZ * ballVelocity.y + ball.transform.position.y; ;

        Vector3 moveDir = new Vector3(predictedX, predictedY, transform.position.z) - transform.position; // do we want them to be able to fly up to hit the ball?
        moveDir.Normalize();
        
        Vector3 newPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

        if (newPos.x > minBoundaries.x && newPos.x < maxBoundaries.x && newPos.y > minBoundaries.y && newPos.y < maxBoundaries.y)
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
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
