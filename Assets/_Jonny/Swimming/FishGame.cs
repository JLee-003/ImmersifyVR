using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FishGame : MonoBehaviour
{
    int fishCaught = 0;
    public static FishGame Instance { get; private set; }

    [Header("Stats Display")]
    private TextMeshProUGUI statsDisplayText;
    private GameObject statsDisplayPanel;
    private GameObject statsDisplayTextObject;
    private LineSwimmer lineSwimmer;
    [SerializeField] private float inactivityThreshold = 5f;
    
    // UI positioning
    private Vector3 fixedUIPosition;
    private bool uiPositionFixed = false;
    private FixedBelowView fixedBelowViewComponent;

    public float difficultyLevel { get; private set; } = 0f;

    private List<float> catchTimes = new List<float>();
    [SerializeField] private float difficultyCheckWindow = 30f;
    [SerializeField] private float difficultyStep = 0.5f;
    [SerializeField] private float minDifficulty = -3f;
    [SerializeField] private float maxDifficulty = 5f;

    // Player inactivity tracking
    private Vector3 lastPlayerPosition;
    private float timeSinceLastMovement = 0f;
    [SerializeField] private float movementThreshold = 0.01f;

    // Game timer
    private float gameTime = 0f;

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

    private void Start()
    {
        InitializePlayerTracking();
        InitializeStatsDisplay();
        
        // Disable FixedBelowView if present - we want fixed global position instead
        fixedBelowViewComponent = GetComponent<FixedBelowView>();
        if (fixedBelowViewComponent != null)
        {
            fixedBelowViewComponent.enabled = false;
        }
        
        if (PlayerReferences.instance != null)
        {
            lineSwimmer = PlayerReferences.instance.swimObject;
            if (lineSwimmer == null)
            {
                Debug.LogWarning("FishGame: swimObject is null in PlayerReferences. Make sure to assign it in the Inspector.");
            }
            else
            {
                Debug.Log($"FishGame: LineSwimmer reference successfully set to {lineSwimmer.gameObject.name}");
            }
        }
        else
        {
            Debug.LogError("FishGame: PlayerReferences.instance is null!");
        }
    }

    private void InitializeStatsDisplay()
    {
        // Find Panel as direct child
        Transform panelTransform = transform.Find("Panel");
        Transform numberDisplayTransform = transform.Find("NumberDisplay");
        
        if (panelTransform != null)
        {
            statsDisplayPanel = panelTransform.gameObject;
        }
        else
        {
            Debug.LogWarning("Child named 'Panel' not found.");
        }

        if (numberDisplayTransform != null)
        {
            statsDisplayTextObject = numberDisplayTransform.gameObject;
            statsDisplayText = numberDisplayTransform.GetComponent<TextMeshProUGUI>();
            if (statsDisplayText == null)
            {
                Debug.LogWarning("TextMeshProUGUI component not found on 'NumberDisplay' child.");
            }
        }
        else
        {
            Debug.LogWarning("Child named 'NumberDisplay' not found.");
        }

        // Disable all children of the object this script is attached to
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        TrackPlayerMovement();
        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
        if (statsDisplayPanel == null || statsDisplayTextObject == null) return;

        bool shouldShowStats = timeSinceLastMovement >= inactivityThreshold;

        if (shouldShowStats)
        {
            // Fix UI position in global space when first showing
            if (!uiPositionFixed && PlayerReferences.instance != null && PlayerReferences.instance.cameraTransform != null)
            {
                Transform playerTransform = PlayerReferences.instance.cameraTransform;
                // Position UI in front of player at fixed distance, then lock it
                fixedUIPosition = playerTransform.position + playerTransform.forward * 2.0f;
                transform.position = fixedUIPosition;
                transform.rotation = Quaternion.LookRotation(transform.position - playerTransform.position);
                uiPositionFixed = true;
            }
            
            // Keep UI at fixed global position
            if (uiPositionFixed)
            {
                transform.position = fixedUIPosition;
            }
            
            statsDisplayPanel.SetActive(true);
            statsDisplayTextObject.SetActive(true);
            UpdateStatsText();
        }
        else
        {
            statsDisplayPanel.SetActive(false);
            statsDisplayTextObject.SetActive(false);
            // Reset position flag when hidden so it repositions next time
            uiPositionFixed = false;
        }
    }

    private void UpdateStatsText()
    {
        if (statsDisplayText == null) return;

        float swimmingDistance = lineSwimmer != null ? lineSwimmer.GetTotalDistanceSwum() : 0f;
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        // Debug logging
        if (lineSwimmer != null)
        {
            Debug.Log($"FishGame: Swimming distance = {swimmingDistance}");
        }

        statsDisplayText.text = $"Fish caught: {fishCaught}\n" +
                               $"Swimming distance: {swimmingDistance:F1}m\n" +
                               $"Time in session: {minutes:D2}:{seconds:D2}";
    }

    private void InitializePlayerTracking()
    {
        if (PlayerReferences.instance != null && PlayerReferences.instance.cameraTransform != null)
        {
            lastPlayerPosition = PlayerReferences.instance.cameraTransform.position;
            timeSinceLastMovement = 0f;
        }
    }

    private void TrackPlayerMovement()
    {
        if (PlayerReferences.instance == null || PlayerReferences.instance.cameraTransform == null)
            return;

        Transform playerTransform = PlayerReferences.instance.cameraTransform;
        
        bool playerMoved = false;
        bool gripPressed = false;
        
        // Check if player has moved (position changed)
        float positionDelta = Vector3.Distance(playerTransform.position, lastPlayerPosition);
        if (positionDelta > movementThreshold)
        {
            playerMoved = true;
            lastPlayerPosition = playerTransform.position;
        }
        
        // Check if grip button is pressed (using same method as LineSwimmer, from PlayerReferences)
        if (PlayerReferences.instance != null)
        {
            if (PlayerReferences.instance.leftControllerSwimReference != null && 
                PlayerReferences.instance.leftControllerSwimReference.action != null)
            {
                if (PlayerReferences.instance.leftControllerSwimReference.action.IsPressed())
                {
                    gripPressed = true;
                }
            }
            
            if (PlayerReferences.instance.rightControllerSwimReference != null && 
                PlayerReferences.instance.rightControllerSwimReference.action != null)
            {
                if (PlayerReferences.instance.rightControllerSwimReference.action.IsPressed())
                {
                    gripPressed = true;
                }
            }
        }

        if (playerMoved || gripPressed)
        {
            // Player moved or grip button pressed - reset timer (UI disappears)
            timeSinceLastMovement = 0f;
        }
        else
        {
            // Player hasn't moved and no grip pressed - increment timer
            timeSinceLastMovement += Time.deltaTime;
        }
    }

    public void CaughtFishUIDisplay()
    {
        fishCaught++;

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

    // Get how long the player hasn't moved or rotated
    public float GetPlayerInactivityTime()
    {
        return timeSinceLastMovement;
    }

    // Check if player is inactive for a certain duration
    public bool IsPlayerInactive(float inactivityDuration)
    {
        return timeSinceLastMovement >= inactivityDuration;
    }

    // Get time since game initialization
    public float GetGameTime()
    {
        return gameTime;
    }

    // Reset game timer
    public void ResetGameTime()
    {
        gameTime = 0f;
    }

}
 