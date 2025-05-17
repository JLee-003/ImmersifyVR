using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisEnemy : MonoBehaviour
{
    [Header("Ball Spawning/Shooting")]
    [SerializeField] Transform ballPrefab;
    Transform player;

    [Header("Firing")]
    [SerializeField] float fireCooldown = 1f;
    [SerializeField] float shootForce = 15f;   // tweak to taste
    float fireTimer = 0f;

    private void Start()
    {
        player = PlayerReferences.instance.playerObject.transform;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldown)
        {
            fireTimer -= fireCooldown;
            ShootBall();
        }
    }

    void ShootBall()
    {
        Transform ball = Instantiate(ballPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Vector3 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * shootForce;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(gameObject);
        }
    }
}
