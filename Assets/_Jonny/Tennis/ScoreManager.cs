using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI scoreText;

    int enemyScore = 0;
    int playerScore = 0;

    // Start is called before the first frame update
    void Awake()
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
    void Start() {
        GameObject scoreTextObject = GameObject.Find("GameScoreText");
        scoreText = scoreTextObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void playerPoint()
    {
        playerScore++;
        updateScoreboard();
    }
    public void enemyPoint()
    {
        enemyScore++;
        updateScoreboard();
    }
    public void updateScoreboard()
    {
        scoreText.text = $"{playerScore} : {enemyScore}";
    }
}
