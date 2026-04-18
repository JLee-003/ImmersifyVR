using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewSpaceballTutorial : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textBox1;
    [SerializeField] private TextMeshProUGUI textBox2;

    [Header("Arrow UI")]
    [SerializeField] private RectTransform arrowImage;
    [Tooltip("Extra Z rotation (deg) in case the arrow sprite's default orientation is not 'up'.")]
    [SerializeField] private float arrowBaseAngleOffset = 0f;

    [Header("Grip Button Hint")]
    [SerializeField] private GameObject gripButtonCanvas;

    [Header("Tutorial Objects")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject tennisBall;
    [SerializeField] private GameObject slideDownObject;
    [SerializeField] private float slideDistance = 5f;
    [SerializeField] private float slideDuration = 2f;

    [Header("Thruster Teaching")]
    [Tooltip("Required displacement (meters) from the substep's starting position along the taught axis.")]
    [SerializeField] private float directionMoveThreshold = 1f;
    [Tooltip("Minimum offset (meters) of a hand from the head along the axis to count as 'on that side'.")]
    [SerializeField] private float sideThreshold = 0.2f;

    [Header("Ball Hit Teaching")]
    [SerializeField] private string enemyBackWallTag = "EnemyPointZone";
    [Tooltip("Four walls that count as 'a different wall' in step 10.")]
    [SerializeField] private GameObject[] hitWalls;

    [Header("Combined Play")]
    [Tooltip("Ordered ball positions for the final combined-play phase. Index 0 = left of player, Index 1 = right of player.")]
    [SerializeField] private Vector3[] combinedPlayBallPositions;
    [Tooltip("Hittable range = CalibrationMeasurements.Instance.armLength * this multiplier.")]
    [SerializeField] private float hittableRangeMultiplier = 1.2f;
    [Tooltip("Ball rigidbody speed above which we consider the ball to have been hit.")]
    [SerializeField] private float ballHitVelocityThreshold = 0.5f;

    // ---- Scripted text ----
    private const string IntroText1 =
        "You are now in zero-gravity space with rocket arms to boost you! The arrow shows the direction of where you will be pushed to.";
    private const string TennisIntroText1 =
        "You are playing a tennis-like game in outer space. Send the ball to the other wall to score.";
    private const string CombineText1 =
        "Good job! Now, combine everything you've learned to play spaceball.";

    private const string TeachLeftText =
        "To move left, you need to push off the right. Point your arms right!";
    private const string TeachUpText =
        "To move up, you need to push off below. Point your arms down!";
    private const string TeachDownText =
        "To move down, you need to push off above. Point your arms up!";
    private const string TeachRightText =
        "To move right, you need to push off the left. Point your arms left!";

    private const string PressGripText = "Press the grip button to activate the thruster.";

    private const string SwingText = "Swing your fists/gloves at the ball to hit it.";
    private const string HitOtherWallText = "Now try to hit the ball on a different wall.";

    private const string BallLeftText =
        "The ball is to your left. Hold your hands to the right and press the grip button to move left to the ball!";
    private const string BallRightText =
        "The ball is to your right. Hold your hands to the left and press the grip button to move right to the ball!";
    private const string NowSwingText = "Now swing your hands to hit the ball!";

    // ---- Runtime state ----
    private HandBoosters handBoosters;
    private Rigidbody ballRb;
    private NewBallCollisionRelay ballRelay;
    private readonly HashSet<GameObject> hitWallSet = new HashSet<GameObject>();

    // Wall-hit detection flags (set from the relay; read in gating coroutines)
    private bool backWallHit = false;
    private bool otherWallHit = false;

    private enum TeachDirection { Left, Right, Up, Down }

    void Start()
    {
        TryCacheHandBoosters();

        SetArrowActive(true);
        SetGripCanvasActive(false);

        if (tennisBall != null)
        {
            ballRb = tennisBall.GetComponent<Rigidbody>();
            ballRelay = tennisBall.GetComponent<NewBallCollisionRelay>();
            if (ballRelay == null)
            {
                ballRelay = tennisBall.AddComponent<NewBallCollisionRelay>();
            }
            ballRelay.Initialize(this);
        }

        if (hitWalls != null)
        {
            foreach (GameObject wall in hitWalls)
            {
                if (wall != null)
                {
                    hitWallSet.Add(wall);
                }
            }
        }

        SetText(textBox1, IntroText1);
        SetText(textBox2, "");

        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        if (handBoosters == null)
        {
            TryCacheHandBoosters();
        }
        UpdateArrowRotation();
    }

    private void TryCacheHandBoosters()
    {
        if (handBoosters != null)
        {
            return;
        }

        if (PlayerReferences.instance != null && PlayerReferences.instance.playerObject != null)
        {
            handBoosters = PlayerReferences.instance.playerObject.GetComponent<HandBoosters>();
        }
    }

    // =========================================================================
    // Main flow
    // =========================================================================
    private IEnumerator RunTutorial()
    {
        // Steps 1-5: teach thrusters in four directions.
        yield return TeachDirectionRoutine(TeachDirection.Left);
        yield return TeachDirectionRoutine(TeachDirection.Up);
        yield return TeachDirectionRoutine(TeachDirection.Down);
        yield return TeachDirectionRoutine(TeachDirection.Right);

        // Step 6: slide the passed-in GameObject down 5 units over slideDuration.
        yield return SlideDownRoutine();

        // Step 7: teleport, disable thrusters, place ball in front.
        TeleportPlayerAndPlaceBall();

        // Step 8-9: tennis intro + swing teach.
        SetText(textBox1, TennisIntroText1);
        SetText(textBox2, SwingText);
        SetArrowActive(false);

        // Step 10a: wait for a back-wall hit.
        backWallHit = false;
        otherWallHit = false;
        yield return new WaitUntil(() => backWallHit);

        // Step 10b: teach hitting a different wall.
        SetText(textBox2, HitOtherWallText);
        yield return new WaitUntil(() => otherWallHit);

        // Step 11: re-enable thrusters, praise + combine.
        if (handBoosters != null)
        {
            handBoosters.SetThrustersEnabled(true);
        }
        SetText(textBox1, CombineText1);
        SetArrowActive(true);

        // Step 12-14: combined play at each provided position.
        if (combinedPlayBallPositions != null)
        {
            for (int i = 0; i < combinedPlayBallPositions.Length; i++)
            {
                yield return CombinedPlayRoutine(combinedPlayBallPositions[i], i == 0);
            }
        }

        SetText(textBox2, "");
    }

    // =========================================================================
    // Step 3-5: Teach a single direction
    // =========================================================================
    private IEnumerator TeachDirectionRoutine(TeachDirection dir)
    {
        Transform player = GetPlayerTransform();
        if (player == null)
        {
            yield break;
        }

        Vector3 startPos = player.position;
        SetText(textBox2, GetTeachText(dir));
        SetGripCanvasActive(false);

        bool gripHintShown = false;

        while (true)
        {
            player = GetPlayerTransform();
            if (player == null)
            {
                yield return null;
                continue;
            }

            if (!gripHintShown && IsHandOnCorrectSide(dir))
            {
                gripHintShown = true;
                SetText(textBox2, PressGripText);
                SetGripCanvasActive(true);
            }

            if (HasMovedEnough(dir, startPos, player.position))
            {
                break;
            }

            yield return null;
        }

        SetGripCanvasActive(false);
    }

    private string GetTeachText(TeachDirection dir)
    {
        switch (dir)
        {
            case TeachDirection.Left: return TeachLeftText;
            case TeachDirection.Right: return TeachRightText;
            case TeachDirection.Up: return TeachUpText;
            case TeachDirection.Down: return TeachDownText;
        }
        return "";
    }

    private bool IsHandOnCorrectSide(TeachDirection dir)
    {
        Transform head = GetHeadTransform();
        Transform lHand = GetLeftHandTransform();
        Transform rHand = GetRightHandTransform();
        if (head == null) return false;

        // To move <dir>, the player needs to hold their hands on the OPPOSITE side
        // (thrusters push away from the hands).
        switch (dir)
        {
            case TeachDirection.Left: // hands on the right
                return (lHand != null && lHand.position.x > head.position.x + sideThreshold)
                    || (rHand != null && rHand.position.x > head.position.x + sideThreshold);
            case TeachDirection.Right: // hands on the left
                return (lHand != null && lHand.position.x < head.position.x - sideThreshold)
                    || (rHand != null && rHand.position.x < head.position.x - sideThreshold);
            case TeachDirection.Up: // hands below head
                return (lHand != null && lHand.position.y < head.position.y - sideThreshold)
                    || (rHand != null && rHand.position.y < head.position.y - sideThreshold);
            case TeachDirection.Down: // hands above head
                return (lHand != null && lHand.position.y > head.position.y + sideThreshold)
                    || (rHand != null && rHand.position.y > head.position.y + sideThreshold);
        }
        return false;
    }

    private bool HasMovedEnough(TeachDirection dir, Vector3 startPos, Vector3 currentPos)
    {
        Vector3 delta = currentPos - startPos;
        switch (dir)
        {
            case TeachDirection.Left: return delta.x <= -directionMoveThreshold;
            case TeachDirection.Right: return delta.x >= directionMoveThreshold;
            case TeachDirection.Up: return delta.y >= directionMoveThreshold;
            case TeachDirection.Down: return delta.y <= -directionMoveThreshold;
        }
        return false;
    }

    // =========================================================================
    // Step 6: slide down
    // =========================================================================
    private IEnumerator SlideDownRoutine()
    {
        if (slideDownObject == null)
        {
            Debug.LogWarning("[NewSpaceballTutorial] SlideDownRoutine: slideDownObject is null; skipping.");
            yield break;
        }

        if (!slideDownObject.activeSelf)
        {
            slideDownObject.SetActive(true);
        }

        Vector3 start = slideDownObject.transform.position;
        Vector3 end = start + Vector3.down * slideDistance;

        Debug.Log($"[NewSpaceballTutorial] SlideDownRoutine begin. object='{slideDownObject.name}' start={start} end={end} slideDistance={slideDistance} slideDuration={slideDuration}");

        if (Mathf.Approximately(slideDistance, 0f))
        {
            Debug.LogWarning("[NewSpaceballTutorial] slideDistance is 0 in the inspector. Set it in the inspector (e.g. 5) or the object will not appear to move.");
        }

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideDuration > 0f ? Mathf.Clamp01(elapsed / slideDuration) : 1f;
            slideDownObject.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        slideDownObject.transform.position = end;
        Debug.Log($"[NewSpaceballTutorial] SlideDownRoutine end. finalPos={slideDownObject.transform.position}");
    }

    // =========================================================================
    // Step 7: teleport + place ball
    // =========================================================================
    private void TeleportPlayerAndPlaceBall()
    {
        Transform player = GetPlayerTransform();
        if (player != null && spawnPosition != null)
        {
            player.position = spawnPosition.position;
        }

        if (handBoosters != null)
        {
            handBoosters.SetThrustersEnabled(false);
        }

        if (tennisBall != null && spawnPosition != null)
        {
            float reach = CalibrationMeasurements.Instance != null
                ? CalibrationMeasurements.Instance.comfortReach
                : 0.5f;
            Vector3 ballPos = spawnPosition.position + spawnPosition.forward * reach;
            tennisBall.transform.position = ballPos;
            if (ballRb != null)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
            if (!tennisBall.activeSelf)
            {
                tennisBall.SetActive(true);
            }
        }
    }

    // =========================================================================
    // Step 12-14: combined play
    // =========================================================================
    private IEnumerator CombinedPlayRoutine(Vector3 ballPosition, bool ballIsLeftOfPlayer)
    {
        if (tennisBall == null)
        {
            yield break;
        }

        tennisBall.transform.position = ballPosition;
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
        if (!tennisBall.activeSelf)
        {
            tennisBall.SetActive(true);
        }

        SetText(textBox2, ballIsLeftOfPlayer ? BallLeftText : BallRightText);

        // Wait for the ball to be within hittable range of the player.
        float hittableRange = (CalibrationMeasurements.Instance != null
            ? CalibrationMeasurements.Instance.armLength
            : 0.8f) * hittableRangeMultiplier;

        while (true)
        {
            Transform player = GetPlayerTransform();
            if (player != null)
            {
                float d = Vector3.Distance(player.position, tennisBall.transform.position);
                if (d <= hittableRange)
                {
                    break;
                }
            }
            yield return null;
        }

        SetText(textBox2, NowSwingText);

        // Wait for the player to actually swing and move the ball.
        while (true)
        {
            if (ballRb != null && ballRb.velocity.magnitude >= ballHitVelocityThreshold)
            {
                break;
            }
            yield return null;
        }
    }

    // =========================================================================
    // Arrow rotation
    // =========================================================================
    private void UpdateArrowRotation()
    {
        if (arrowImage == null)
        {
            return;
        }

        Transform head = GetHeadTransform();
        Transform lHand = GetLeftHandTransform();
        Transform rHand = GetRightHandTransform();
        if (head == null)
        {
            return;
        }

        Vector3 lDir = Vector3.zero;
        Vector3 rDir = Vector3.zero;
        bool hasL = false, hasR = false;

        if (lHand != null)
        {
            lDir = head.position - lHand.position;
            lDir.z = 0f;
            if (lDir.sqrMagnitude > 0.0001f)
            {
                lDir.Normalize();
                hasL = true;
            }
        }
        if (rHand != null)
        {
            rDir = head.position - rHand.position;
            rDir.z = 0f;
            if (rDir.sqrMagnitude > 0.0001f)
            {
                rDir.Normalize();
                hasR = true;
            }
        }

        if (!hasL && !hasR)
        {
            return;
        }

        bool leftPressed = handBoosters != null && handBoosters.IsLeftBoosting;
        bool rightPressed = handBoosters != null && handBoosters.IsRightBoosting;

        Vector3 dir;
        if (leftPressed && !rightPressed)
        {
            dir = hasL ? lDir : rDir;
        }
        else if (rightPressed && !leftPressed)
        {
            dir = hasR ? rDir : lDir;
        }
        else
        {
            // Both pressed, or neither pressed: average both hands.
            if (hasL && hasR) dir = (lDir + rDir).normalized;
            else if (hasL) dir = lDir;
            else dir = rDir;
        }

        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f + arrowBaseAngleOffset;
        arrowImage.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    // =========================================================================
    // Ball collision relay hooks
    // =========================================================================
    internal void OnBallCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        GameObject go = collision.gameObject;
        if (go == null)
        {
            return;
        }

        if (!backWallHit && go.CompareTag(enemyBackWallTag))
        {
            backWallHit = true;
            return;
        }

        if (backWallHit && !otherWallHit && IsHitWall(go))
        {
            otherWallHit = true;
        }
    }

    private bool IsHitWall(GameObject go)
    {
        if (go == null || hitWalls == null)
        {
            return false;
        }

        if (hitWallSet.Contains(go))
        {
            return true;
        }

        // Also accept children of any listed wall.
        Transform t = go.transform;
        foreach (GameObject wall in hitWalls)
        {
            if (wall == null) continue;
            if (t.IsChildOf(wall.transform))
            {
                return true;
            }
        }
        return false;
    }

    // =========================================================================
    // Helpers
    // =========================================================================
    private void SetText(TextMeshProUGUI tmp, string value)
    {
        if (tmp != null)
        {
            tmp.text = value;
        }
    }

    private void SetArrowActive(bool active)
    {
        if (arrowImage != null)
        {
            arrowImage.gameObject.SetActive(active);
        }
    }

    private void SetGripCanvasActive(bool active)
    {
        if (gripButtonCanvas != null)
        {
            gripButtonCanvas.SetActive(active);
        }
    }

    private Transform GetPlayerTransform()
    {
        if (PlayerReferences.instance != null && PlayerReferences.instance.playerObject != null)
        {
            return PlayerReferences.instance.playerObject.transform;
        }
        return null;
    }

    private Transform GetHeadTransform()
    {
        if (PlayerReferences.instance != null)
        {
            return PlayerReferences.instance.cameraTransform;
        }
        return null;
    }

    private Transform GetLeftHandTransform()
    {
        if (PlayerReferences.instance != null && PlayerReferences.instance.leftController != null)
        {
            return PlayerReferences.instance.leftController.transform;
        }
        return null;
    }

    private Transform GetRightHandTransform()
    {
        if (PlayerReferences.instance != null && PlayerReferences.instance.rightController != null)
        {
            return PlayerReferences.instance.rightController.transform;
        }
        return null;
    }
}

public class NewBallCollisionRelay : MonoBehaviour
{
    private NewSpaceballTutorial tutorial;

    public void Initialize(NewSpaceballTutorial owner)
    {
        tutorial = owner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (tutorial != null)
        {
            tutorial.OnBallCollisionEnter(collision);
        }
    }
}
