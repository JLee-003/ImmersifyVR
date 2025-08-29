using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisEnemy : MonoBehaviour
{
    [SerializeField] float hitForce = 3f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float missChance = 0.025f;
    private int totalHits = 0;

    public float moveSpeedMultiplier = 1f;


    [SerializeField] Vector2 minBoundaries;
    [SerializeField] Vector2 maxBoundaries;

    Transform player;

    GameObject ball;

    [SerializeField] AudioClip hitAudio;

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
        if (totalHits > 10)
        {
            totalHits = 0;
            missChance = 0.025f;
        }
        else if (Random.value < missChance)
        {
            Debug.Log("missed due to random chance!");
            totalHits = 0;
            missChance = 0.025f;
        }
        else
        {
            Vector3 dir = player.position - transform.position;
            dir.Normalize();

            ball.GetComponent<ZeroGravProjectile>().SetVelocity(dir * hitForce);
            totalHits++;
            missChance += 0.025f;

            AudioSource.PlayClipAtPoint(hitAudio, transform.position, 0.5f);
        }
    }

    void MoveToBall(GameObject ball)
    {

        // ------ jonny implementation -----
        // x and y can move, z is fixed. predict the coordinates that it will reach the z axis. then move the enemy there.
        // enemy has a speed (moveSpeed). if enemy can't reach it in time that's one way for the player to beat the enemy

        // calculate z location <--- can be optimized, no need to calculate every frame. instead can calculate once when the ball is hit
        Vector3 ballVelocity = ball.GetComponent<Rigidbody>().velocity;


        // if ball is moving away from enemy then dont move
        if (ballVelocity.z < 0) return;


        // key physics formula: df = v*t + di since acceleration is 0. time is constant across because its the same object moving
        float timeToZ = Mathf.Abs(ball.transform.position.z - transform.position.z) / ballVelocity.z;
        float predictedX = timeToZ * ballVelocity.x + ball.transform.position.x;
        float predictedY = timeToZ * ballVelocity.y + ball.transform.position.y;

        Vector3 moveDir = new Vector3(predictedX, predictedY, transform.position.z) - transform.position; // do we want them to be able to fly up to hit the ball?
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
