using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Transform enemyPrefab;
    [SerializeField] Vector3 spawnCenter;
    [SerializeField] float spawnRadius;
    [SerializeField] float spawnCooldown = 3f;

    float spawnTimer = 0f;


    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCooldown)
        {
            spawnTimer -= spawnCooldown;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnPos = spawnCenter + new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * spawnRadius;
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
