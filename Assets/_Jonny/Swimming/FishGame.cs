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

    public float difficultyLevel { get; private set; } = 0f;

    private List<float> catchTimes = new List<float>();
    [SerializeField] private float difficultyCheckWindow = 30f;
    [SerializeField] private float difficultyStep = 0.5f;
    [SerializeField] private float minDifficulty = -3f;
    [SerializeField] private float maxDifficulty = 5f;

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

    public void CaughtFishUIDisplay()
    {
        fishCaught++;
        fishText.text = $"Fish Caught: {fishCaught}";

        catchTimes.Add(Time.time);
        catchTimes.RemoveAll(t => Time.time - t > difficultyCheckWindow);

        AdjustDifficulty();
    }

    private void AdjustDifficulty()
    {
        int recentCatches = catchTimes.Count;

        if (recentCatches >= 5)
            difficultyLevel += difficultyStep;
        else if (recentCatches <= 1)
            difficultyLevel -= difficultyStep;

        difficultyLevel = Mathf.Clamp(difficultyLevel, minDifficulty, maxDifficulty);
    }

}
 