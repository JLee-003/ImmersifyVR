using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedMovement : MonoBehaviour
{
    public float speed = 5.0f;

    private float acceleration = Random.Range(-2.0f, 2.0f);

    public float changeDirectionTimer = 4.0f;
    private Vector3 randomDirection;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        randomDirection = GetRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(randomDirection * speed * Time.deltaTime);

        timer += Time.deltaTime;

        if (timer >= changeDirectionTimer)
        {
            speed += acceleration;
            randomDirection = GetRandomDirection();
            timer = 0;
        }
    }

    Vector3 GetRandomDirection()
    {
        float randX = Random.Range(-1.0f, 1.0f);
        float randY = Random.Range(-1.0f, 1.0f);
        float randZ = Random.Range(-1.0f, 1.0f);

        return new Vector3(randX, randY, randZ).normalized;
    }
}