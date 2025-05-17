using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [SerializeField] Transform balloon;
    [SerializeField] float spawnCooldown = 1f;

    float spawnTimer = 0f;
    

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCooldown)
        {
            spawnTimer -= spawnCooldown;
            SpawnBalloon();
        }
    }

    void SpawnBalloon()
    {
        Transform newBalloon = Instantiate(balloon);
        newBalloon.transform.position = new Vector3(Random.Range(3f, -3f), -6f, 6f);
    }
}
