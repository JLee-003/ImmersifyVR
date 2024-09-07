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
}

public class FishSpawner : MonoBehaviour
{
    public List<FishInfo> fishInfoList;

    [SerializeField] GameObject fishPrefab;

    private void Start()
    {
        // Loop through each fish in the list
        foreach (FishInfo fishInfo in fishInfoList)
        {
            for (int i = 0; i < fishInfo.numberOfFish; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(fishInfo.minX, fishInfo.maxX),
                    Random.Range(fishInfo.minY, fishInfo.maxY),
                    Random.Range(fishInfo.minZ, fishInfo.maxZ));

                GameObject fish = Instantiate(fishPrefab, randomPosition, Quaternion.identity);
                fish.GetComponent<Fish>().type = fishInfo.type;
            }
        }
    }

}
