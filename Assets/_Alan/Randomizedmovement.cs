using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float changeDirectionTimer = 6.0f;
    private Vector3 randomDirection;
    private float timer;
    public Rigidbody fish;
    private float bound = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        fish = GetComponent<Rigidbody>();
        randomDirection = GetRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

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



    Vector3 GetRandomDirection()
    {
        float randX = Random.Range(-1.0f, 1.0f);
        float randY = Random.Range(-1.0f, 1.0f);
        float randZ = Random.Range(-1.0f, 1.0f);

        return new Vector3(randX, randY, randZ).normalized;
    }
}