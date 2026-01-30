using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceballTutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialTargets;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TennisEnemy tennisEnemy;
    [SerializeField] private GameObject tennisBall;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject moveBallCanvas;
    [SerializeField] private float closeDistanceSqr = 1f;
    [SerializeField] private Vector2 randomBallXBounds = new Vector2(-3.5f, 3.5f);
    [SerializeField] private Vector2 randomBallYBounds = new Vector2(0.5f, 3f);
    [SerializeField] private bool useDefaultBallZ = true;
    [SerializeField] private float randomBallZ = 0f;
    [SerializeField] private float ballRestVelocityThreshold = 0.1f;
    [SerializeField] private string enemyBackWallTag = "EnemyPointZone";
    [SerializeField] private int tutorialTargetCount = 3;


    private int currentTargetIndex = 0;
    private Transform playerTransform;
    private bool tutorialStarted = false;
    private bool enemyActivated = false;
    private bool pointScored = false;
    private bool pointScoredHandled = false;
    private int scoreAtEnemyStart = 0;
    private bool ballInMotion = false;
    private Rigidbody ballRb;
    private Vector3 defaultBallPosition;
    private Vector3 currentBallPosition;
    private Vector3 defaultPlayerPosition;

    private enum TutorialPhase
    {
        WaitingForClose,
        ReadyToHit,
        ScorePoint,
        PointScored
    }

    private TutorialPhase phase = TutorialPhase.WaitingForClose;

    private const string MoveCloserText = "Good job! Move to the ball: hold your hands opposite the direction of the ball and press the grip button.";
    private const string FirstSwingText = "Now swing your arm to hit the ball. Try to hit the ball in the green zone!";
    private const string HitTargetText = "Now hit the ball in the green zone!";
    private const string ScorePointText = "Now try to score a point on the enemy's side!";
    private const string PointScoredText = "Great job! You scored a point!";

    void Start()
    {
        if (tutorialText != null)
        {
            tutorialText.text = MoveCloserText;
        }

        if (PlayerReferences.instance != null && PlayerReferences.instance.playerObject != null)
        {
            playerTransform = PlayerReferences.instance.playerObject.transform;
            defaultPlayerPosition = spawnPoint != null ? spawnPoint.position : playerTransform.position;
        }

        if (tutorialTargets != null)
        {
            foreach (GameObject target in tutorialTargets)
            {
                if (target != null)
                {
                    target.SetActive(false);
                    TutorialTargetTrigger relay = target.GetComponent<TutorialTargetTrigger>();
                    if (relay == null)
                    {
                        relay = target.AddComponent<TutorialTargetTrigger>();
                    }
                    relay.Initialize(this);
                }
            }
        }

        if (tennisEnemy != null)
        {
            tennisEnemy.gameObject.SetActive(false);
        }

        if (moveBallCanvas != null)
        {
            moveBallCanvas.SetActive(false);
        }

        if (tennisBall != null)
        {
            defaultBallPosition = tennisBall.transform.position;
            currentBallPosition = defaultBallPosition;
            ballRb = tennisBall.GetComponent<Rigidbody>();

            BallTutorialCollisionRelay relay = tennisBall.GetComponent<BallTutorialCollisionRelay>();
            if (relay == null)
            {
                relay = tennisBall.AddComponent<BallTutorialCollisionRelay>();
            }
            relay.Initialize(this);
        }
    }

    void Update()
    {
        Debug.Log("Phase: " + phase);
        if (tutorialText == null || tennisBall == null)
        {
            return;
        }

        if (playerTransform == null && PlayerReferences.instance != null && PlayerReferences.instance.playerObject != null)
        {
            playerTransform = PlayerReferences.instance.playerObject.transform;
        }

        if (playerTransform == null)
        {
            return;
        }

        if (phase == TutorialPhase.ScorePoint || phase == TutorialPhase.PointScored)
        {
            UpdateScoreProgress();

            if (pointScored && phase == TutorialPhase.ScorePoint)
            {
                phase = TutorialPhase.PointScored;
            }

            if (phase == TutorialPhase.PointScored)
            {
                tutorialText.text = PointScoredText;
                if (!pointScoredHandled)
                {
                    pointScoredHandled = true;
                    SceneLoader.Instance.LoadNewScene("SpaceballLv1");
                }
                return;
            }

            tutorialText.text = ScorePointText;
            return;
        }

        bool isClose = (playerTransform.position - tennisBall.transform.position).sqrMagnitude < closeDistanceSqr;

        switch (phase)
        {
            case TutorialPhase.WaitingForClose:
                SetMoveBallCanvasActive(true);
                if (!isClose)
                {
                    tutorialText.text = MoveCloserText;
                    return;
                }

                if (!tutorialStarted)
                {
                    ActivateTarget(currentTargetIndex);
                    tutorialStarted = true;
                }

                ballInMotion = false;
                phase = TutorialPhase.ReadyToHit;
                tutorialText.text = currentTargetIndex == 0 ? FirstSwingText : HitTargetText;
                break;

            case TutorialPhase.ReadyToHit:
                SetMoveBallCanvasActive(false);
                if (!isClose)
                {
                    phase = TutorialPhase.WaitingForClose;
                    SetMoveBallCanvasActive(true);
                    tutorialText.text = MoveCloserText;
                    return;
                }

                if (IsTargetComplete(currentTargetIndex))
                {
                    HandleTargetHit();
                    return;
                }

                UpdateMissDetection();
                if (phase != TutorialPhase.ReadyToHit)
                {
                    return;
                }
                tutorialText.text = currentTargetIndex == 0 ? FirstSwingText : HitTargetText;
                break;
        }
    }

    private void UpdateScoreProgress()
    {
        if (!enemyActivated || pointScored)
        {
            return;
        }

        if (ScoreManager.Instance != null && ScoreManager.Instance.playerScore > scoreAtEnemyStart)
        {
            pointScored = true;
        }
    }

    private bool IsTargetComplete(int index)
    {
        int activeTargetCount = GetActiveTargetCount();
        if (index < 0 || index >= activeTargetCount)
        {
            return false;
        }

        GameObject target = tutorialTargets[index];
        return target == null || !target.activeInHierarchy;
    }

    private void ActivateTarget(int index)
    {
        int activeTargetCount = GetActiveTargetCount();
        if (index < 0 || index >= activeTargetCount)
        {
            return;
        }

        GameObject target = tutorialTargets[index];
        if (target != null)
        {
            target.SetActive(true);
        }
    }

    private void HandleTargetHit()
    {
        DisableTarget(currentTargetIndex);
        currentTargetIndex++;
        ballInMotion = false;

        int activeTargetCount = GetActiveTargetCount();
        if (currentTargetIndex >= activeTargetCount)
        {
            StartScorePhase();
            return;
        }

        ActivateTarget(currentTargetIndex);
        RandomizeBallPosition();
        phase = TutorialPhase.WaitingForClose;
        tutorialText.text = MoveCloserText;
    }

    private void DisableTarget(int index)
    {
        int activeTargetCount = GetActiveTargetCount();
        if (index < 0 || index >= activeTargetCount)
        {
            return;
        }

        GameObject target = tutorialTargets[index];
        if (target != null)
        {
            target.SetActive(false);
        }
    }

    private void UpdateMissDetection()
    {
        if (ballRb == null)
        {
            return;
        }

        float speed = ballRb.velocity.magnitude;
        if (!ballInMotion && speed > ballRestVelocityThreshold)
        {
            ballInMotion = true;
        }
        else if (ballInMotion && speed <= ballRestVelocityThreshold)
        {
            HandleMiss();
        }
    }

    private void HandleMiss()
    {
        ballInMotion = false;
        ResetBallToCurrentPosition();
        phase = TutorialPhase.WaitingForClose;
        tutorialText.text = MoveCloserText;
        SetMoveBallCanvasActive(true);
    }

    private void RandomizeBallPosition()
    {
        if (tennisBall == null)
        {
            return;
        }

        float minX = Mathf.Min(randomBallXBounds.x, randomBallXBounds.y);
        float maxX = Mathf.Max(randomBallXBounds.x, randomBallXBounds.y);
        float minY = Mathf.Min(randomBallYBounds.x, randomBallYBounds.y);
        float maxY = Mathf.Max(randomBallYBounds.x, randomBallYBounds.y);

        float z = useDefaultBallZ ? defaultBallPosition.z : randomBallZ;
        Vector3 newPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), z);

        currentBallPosition = newPosition;
        tennisBall.transform.position = newPosition;
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
    }

    private void ResetBallToCurrentPosition()
    {
        if (tennisBall == null)
        {
            return;
        }

        tennisBall.transform.position = currentBallPosition;
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
    }

    private void ActivateEnemy()
    {
        if (enemyActivated)
        {
            return;
        }

        enemyActivated = true;
        scoreAtEnemyStart = ScoreManager.Instance != null ? ScoreManager.Instance.playerScore : 0;

        if (tennisEnemy != null)
        {
            tennisEnemy.gameObject.SetActive(true);
        }
    }

    private void StartScorePhase()
    {
        SetMoveBallCanvasActive(false);
        foreach (GameObject target in tutorialTargets)
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }

        if (tennisBall != null)
        {
            tennisBall.transform.position = defaultBallPosition;
            currentBallPosition = defaultBallPosition;
            if (ballRb != null)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }

        if (playerTransform != null)
        {
            if (spawnPoint != null)
            {
                playerTransform.position = spawnPoint.position;
            }
            else
            {
                playerTransform.position = defaultPlayerPosition;
            }
        }

        ActivateEnemy();
        phase = TutorialPhase.ScorePoint;
    }

    private void SetMoveBallCanvasActive(bool isActive)
    {
        if (moveBallCanvas != null)
        {
            moveBallCanvas.SetActive(isActive);
        }
    }

    internal void OnBallTriggerEnter(Collider other)
    {
        if (phase != TutorialPhase.ReadyToHit && phase != TutorialPhase.WaitingForClose)
        {
            return;
        }

        if (IsCurrentTargetHit(other))
        {
            HandleTargetHit();
        }
    }

    internal void OnTargetTriggered(GameObject targetObject)
    {
        if ((phase != TutorialPhase.ReadyToHit && phase != TutorialPhase.WaitingForClose) || targetObject == null)
        {
            return;
        }

        int activeTargetCount = GetActiveTargetCount();
        if (currentTargetIndex < 0 || currentTargetIndex >= activeTargetCount)
        {
            return;
        }

        GameObject currentTarget = tutorialTargets[currentTargetIndex];
        if (currentTarget == null)
        {
            return;
        }

        if (targetObject == currentTarget || targetObject.transform.IsChildOf(currentTarget.transform))
        {
            HandleTargetHit();
        }
    }

    internal void OnBallCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag(enemyBackWallTag))
        {
            if (phase == TutorialPhase.ScorePoint)
            {
                // if (ScoreManager.Instance == null || !ScoreManager.Instance.isActiveAndEnabled)
                pointScored = true;
                phase = TutorialPhase.PointScored;
                return;
            }

            if (phase != TutorialPhase.PointScored)
            {
                HandleMiss();
            }
        }
    }

    private bool IsCurrentTargetHit(Collider other)
    {
        if (tutorialTargets == null || other == null)
        {
            return false;
        }

        int activeTargetCount = GetActiveTargetCount();
        if (currentTargetIndex < 0 || currentTargetIndex >= activeTargetCount)
        {
            return false;
        }

        GameObject currentTarget = tutorialTargets[currentTargetIndex];
        if (currentTarget == null)
        {
            return false;
        }

        return other.gameObject == currentTarget || other.transform.IsChildOf(currentTarget.transform);
    }

    private int GetActiveTargetCount()
    {
        if (tutorialTargets == null || tutorialTargets.Length == 0)
        {
            return 0;
        }

        if (tutorialTargetCount <= 0)
        {
            return tutorialTargets.Length;
        }

        return Mathf.Min(tutorialTargetCount, tutorialTargets.Length);
    }
}

public class BallTutorialCollisionRelay : MonoBehaviour
{
    private SpaceballTutorialManager manager;

    public void Initialize(SpaceballTutorialManager tutorialManager)
    {
        manager = tutorialManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (manager != null)
        {
            manager.OnBallTriggerEnter(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (manager != null)
        {
            manager.OnBallCollisionEnter(collision);
        }
    }
}

public class TutorialTargetTrigger : MonoBehaviour
{
    private SpaceballTutorialManager manager;

    public void Initialize(SpaceballTutorialManager tutorialManager)
    {
        manager = tutorialManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (manager == null || other == null)
        {
            return;
        }

        if (other.CompareTag("Ball"))
        {
            manager.OnTargetTriggered(gameObject);
        }
    }
}
