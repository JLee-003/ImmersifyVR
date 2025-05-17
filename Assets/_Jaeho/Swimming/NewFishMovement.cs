using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFishMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float changeDirectionTimer = 3.0f;
    private Vector3 randomDirection;
    private float timer;
    public Rigidbody fish;
    public float bound = 9.0f;

    void Start()
    {
        fish = GetComponent<Rigidbody>();
        randomDirection = GetRandomDirection();
    }
    void Update()
    {

        timer += Time.deltaTime;

        if (fish.position.y >= bound && randomDirection.y > 0)
        {
            randomDirection.y = -randomDirection.y;
        }

        if (timer >= changeDirectionTimer)
        {
            randomDirection = GetRandomDirection();

            if (fish.position.y >= bound && randomDirection.y > 0)
            {
                randomDirection.y = -randomDirection.y;
            }

            fish.AddForce(randomDirection * speed, ForceMode.Impulse);
            timer = 0;
        }

    }

    void FixedUpdate()
    {
        Quaternion rot = Quaternion.LookRotation(randomDirection, Vector3.up);
        fish.MoveRotation(rot);
        fish.angularVelocity = Vector3.zero;
    }

    Vector3 GetRandomDirection()
    {
        float randX = Random.Range(-1.0f, 1.0f);
        float randY = Random.Range(-1.0f, 1.0f);
        float randZ = Random.Range(-1.0f, 1.0f);

        return new Vector3(randX, randY, randZ).normalized;
    }
}
