using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishTypeChance
{
    public int type;                // fish type ID
    [Range(0f, 1f)] public float chance = 1f; // probability weight
}

[System.Serializable]
public class FishInfo
{
    public float minX = 0f;
    public float minY = 0f;
    public float minZ = 0f;
    public float maxX = 0f;
    public float maxY = 0f;
    public float maxZ = 0f;

    public int numberOfFish;
    public float cooldown = 2.5f;
    [HideInInspector] public float respawnTimer = 0f;

    public List<GameObject> fishList = new List<GameObject>();

    // 🎲 possible fish types for this region
    public List<FishTypeChance> fishTypes = new List<FishTypeChance>();
}

public class FishSpawner : MonoBehaviour
{
    public List<FishInfo> fishInfoList;

    [SerializeField] float minimumPlayerDist = 5f;
    [SerializeField] GameObject fishPrefab; 

    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject tagged 'Player' found! Make sure your player has the correct tag.");
        }

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
            // clean up null fish
            for (int i = fishInfo.fishList.Count - 1; i >= 0; i--)
            {
                if (fishInfo.fishList[i] == null)
                {
                    fishInfo.fishList.RemoveAt(i);
                }
            }

            // respawn logic
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

    private FishTypeChance GetRandomFishType(FishInfo fishInfo)
    {
        float total = 0f;
        foreach (var ft in fishInfo.fishTypes) total += ft.chance;

        float r = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var ft in fishInfo.fishTypes)
        {
            cumulative += ft.chance;
            if (r <= cumulative)
                return ft;
        }

        return fishInfo.fishTypes[fishInfo.fishTypes.Count - 1]; // fallback
    }

    private void SpawnFish(FishInfo fishInfo)
    {
        bool spawned = false;
        int maxAttempts = 20; // safety guard
        int attempts = 0;

        while (!spawned && attempts < maxAttempts)
        {
            attempts++;

            float randomX = Random.Range(fishInfo.minX, fishInfo.maxX);
            float randomY = Random.Range(fishInfo.minY, fishInfo.maxY);
            float randomZ = Random.Range(fishInfo.minZ, fishInfo.maxZ);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            if (player == null || Vector3.Distance(randomPosition, player.transform.position) >= minimumPlayerDist)
            {
                // 🎲 pick type randomly
                FishTypeChance chosenType = GetRandomFishType(fishInfo);

                GameObject fish = Instantiate(fishPrefab, randomPosition, Quaternion.identity);
                Fish fishComponent = fish.GetComponent<Fish>();
                if (fishComponent != null)
                {
                    fishComponent.type = chosenType.type;
                }

                fishInfo.fishList.Add(fish);
                spawned = true;
            }
        }
    }
}
