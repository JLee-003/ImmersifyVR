using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCollect : MonoBehaviour
{
    public int fishCollected = 0;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Fish"))
        {
            fishCollected += 1;
            Destroy(other.gameObject);
        }
    }
}
