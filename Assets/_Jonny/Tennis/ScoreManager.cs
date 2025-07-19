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

    GameObject enemyObj;
    GameObject ballObj;

    Vector3 ballServePos = new Vector3(0, 1, -9);
    Vector3 playerNeutralPos = new Vector3(0, 0.5f, -11);
    Vector3 enemyNeutralPos = new Vector3(0, 0.75f, 11);


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
    }
    public void enemyPoint()
    {
        enemyScore++;
        updateScoreboard();
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
        playerScore = 0;
        enemyScore = 0;

        string playerTxt = $"{playerScore}";
        string enemyTxt = $"{enemyScore}";
        if (playerScore < 10) playerTxt = $"0{playerScore}";
        if (enemyScore < 10) enemyTxt = $"0{enemyScore}";


        scoreText.text = $"{playerTxt} : {enemyTxt}";


        setCourtToNeutral();
    }

    public void setCourtToNeutral()
    {
        Debug.Log("Resetting court to neutral serving state.");
        Teleport.Instance.tp(playerNeutralPos.x, playerNeutralPos.y, playerNeutralPos.z);
        enemyObj.transform.position = enemyNeutralPos;
        ballObj.transform.position = ballServePos;
        ballObj.GetComponent<ZeroGravProjectile>().ChangeVelocity(Vector3.zero);
    }
}
