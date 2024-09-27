using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishInfo
{
    public float minX = 0f;
    public float minY = 0f;
    public float minZ = 0f;
    public float maxX = 0f;
    public float maxY = 0f;
    public float maxZ = 0f;

    public int type;

    public int numberOfFish;

    public float cooldown = 5f;
    [HideInInspector] public float respawnTimer = 0f;

    public List<GameObject> fishList = new List<GameObject>();
}

public class FishSpawner : MonoBehaviour
{
    public List<FishInfo> fishInfoList;

    [SerializeField] float minimumPlayerDist;

    [SerializeField] GameObject fishPrefab;

    GameObject playerView;

    private void Start()
    {

        playerView = GameObject.FindGameObjectWithTag("MainCamera");


        foreach (FishInfo fishInfo in fishInfoList)
        {
            for (int i = 0; i < fishInfo.numberOfFish; i++)
            {
                SpawnFish(fishInfo);
            }
        }
    }
    private void Update()
    {
        foreach (FishInfo fishInfo in fishInfoList)
        {
            foreach(GameObject fish in fishInfo.fishList)
            {
                if (fish == null)
                {
                    fishInfo.fishList.Remove(fish);
                }
            }
            if (fishInfo.fishList.Count < fishInfo.numberOfFish)
            {
                if (fishInfo.respawnTimer >= fishInfo.cooldown)
                {
                    SpawnFish(fishInfo);
                    fishInfo.respawnTimer = 0f;
                }
                else
                {
                    fishInfo.respawnTimer += Time.deltaTime;
                }
            }
        }
    }

    private void SpawnFish(FishInfo fishInfo)
    {
        bool spawned = false;
        while (!spawned)
        {
            float randomX = Random.Range(fishInfo.minX, fishInfo.maxX);
            float randomY = Random.Range(fishInfo.minY, fishInfo.maxY);
            float randomZ = Random.Range(fishInfo.minZ, fishInfo.maxZ);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            Vector3 difference = randomPosition - playerView.transform.position;

            if (difference.magnitude >= minimumPlayerDist)
            {
                GameObject fish = Instantiate(fishPrefab, randomPosition, Quaternion.identity);
                fish.GetComponent<Fish>().type = fishInfo.type;
                fishInfo.fishList.Add(fish);

                spawned = true;
            }
        }
    }
}
