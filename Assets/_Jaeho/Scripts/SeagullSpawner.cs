using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullSpawner : MonoBehaviour
{
    // Assign your seagull prefab in the inspector.
    public GameObject seagullPrefab;

    // Time between spawns.
    public float spawnInterval = 2f;

    // Radius of the circle where seagulls spawn.
    public float circleRadius = 300f;
    public Vector3 center = new Vector3(165, 47, 170);
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnSeagull();
            timer = spawnInterval;
        }
    }

    private void SpawnSeagull()
    {
        //Random spawning around center
        float angle = Random.Range(0f, 2 * Mathf.PI);

        float spawnX = center.x + circleRadius * Mathf.Cos(angle);
        float spawnZ = center.z + circleRadius * Mathf.Sin(angle);

        Vector3 spawnPosition = new Vector3(spawnX, center.y, spawnZ);

        //Face center
        Vector3 directionToCenter = center - spawnPosition;
        Quaternion rotation = Quaternion.LookRotation(directionToCenter);

        GameObject seagull = Instantiate(seagullPrefab, spawnPosition, rotation);

        seagull.transform.SetParent(transform);
    }
}
