using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI setText;
    [SerializeField] TextMeshProUGUI matchPointText;

    int enemyScore = 0;
    int enemySetScore = 0;
    int playerScore = 0;
    int playerSetScore = 0;
    int winCondition = 5;

    [SerializeField] string[] levelScenes; // Array of scene names for level progression
    int currentLevelIndex = 0; // Tracks which level the player is on

    GameObject enemyObj;
    GameObject ballObj;

    [SerializeField] Vector3 ballServePos;
    [SerializeField] Vector3 playerNeutralPos;
    [SerializeField] Vector3 enemyNeutralPos;

    [SerializeField] GameObject playerWinScreen;



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
    void Start()
    {
        GameObject scoreTextObject = GameObject.Find("GameScoreText");
        scoreText = scoreTextObject.GetComponent<TMPro.TextMeshProUGUI>();

        enemyObj = GameObject.FindGameObjectWithTag("Enemy");
        ballObj = GameObject.FindGameObjectWithTag("Ball");
    }

    // Update is called once per frame
    public void playerPoint()
    {
        playerScore++;
        updateScoreboard();
        setCourtToNeutral();
    }
    public void enemyPoint()
    {
        enemyScore++;
        updateScoreboard();
        setCourtToNeutral();
    }
    public void updateScoreboard()
    {
        string playerTxt = $"{playerScore}";
        string enemyTxt = $"{enemyScore}";
        if (playerScore < 10) playerTxt = $"0{playerScore}";
        if (enemyScore < 10) enemyTxt = $"0{enemyScore}";


        scoreText.text = $"{playerTxt} : {enemyTxt}";


        // check if any player wins
        if (playerScore >= winCondition && playerScore - enemyScore >= 2)
        {
            // player wins
            playerSetScore++;
            resetMatch();

        }
        else if (enemyScore >= winCondition && enemyScore - playerScore >= 2)
        {
            // enemy wins
            enemySetScore++;
            resetMatch();

        }
    }
    public void resetMatch()
    {
        if (enemySetScore >= 3) //Player loses
        {
            Debug.Log("Match reset.");
            return;
        }

        if (playerSetScore >= 3) //Player wins
        {
            Debug.Log("Player wins the match! Moving to next level.");
            LoadNextLevel();
            return;
        }


        Debug.Log("Resetting match.");
        playerScore = 0;
        enemyScore = 0;
        scoreText.text = "00 : 00";

        setText.text = $"{playerSetScore} : {enemySetScore}";


        setCourtToNeutral();
    }

    public void setCourtToNeutral()
    {
        Debug.Log("Resetting court to neutral serving state.");
        Teleport.Instance.tp(playerNeutralPos.x, playerNeutralPos.y, playerNeutralPos.z);
        enemyObj.transform.position = enemyNeutralPos;
        ballObj.transform.position = ballServePos;
        ballObj.GetComponent<ZeroGravProjectile>().SetVelocity(Vector3.zero);
    }

    void LoadNextLevel()
    {
        // Check if levelScenes array is set up and has scenes
        if (levelScenes == null || levelScenes.Length == 0)
        {
            Debug.LogWarning("No level scenes configured in ScoreManager!");
            return;
        }

        // Check if SceneLoader instance exists
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found! Make sure SceneLoader is in the scene.");
            return;
        }

        // Increment to next level
        currentLevelIndex++;

        // Check if there's a next level available
        if (currentLevelIndex < levelScenes.Length)
        {
            string nextScene = levelScenes[currentLevelIndex];
            Debug.Log($"Loading next level: {nextScene}");
            SceneLoader.Instance.LoadNewScene(nextScene);
        }
        else
        {
            Debug.Log("No more levels! Player has completed all levels.");
            // Optionally, you could loop back to first level or show a completion screen
            // SceneLoader.Instance.LoadNewScene(levelScenes[0]); // Uncomment to loop back
        }
    }
}
