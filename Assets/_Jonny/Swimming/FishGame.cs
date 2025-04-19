using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FishGame : MonoBehaviour
{
    int fishCaught = 0;
    [SerializeField] TextMeshProUGUI fishText;
    public static FishGame Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void CaughtFishUIDisplay() {
        fishCaught++;
        fishText.text = $"Fish Caught: {fishCaught}";
    } 

}
 