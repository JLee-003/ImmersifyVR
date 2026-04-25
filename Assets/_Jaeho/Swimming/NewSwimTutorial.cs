using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class NewSwimTutorial : MonoBehaviour
{
    public enum TutorialState
    {
        WaitForStartPose,
        WaitForEndPose,
        WaitForGripRelease,
        WaitForRockBreak,
        NormalSwimmingWithFish
    }

    [Header("Start Hand Zones")]
    [SerializeField] private TutorialHandZone leftStartZone;
    [SerializeField] private TutorialHandZone rightStartZone;

    [Header("End Hand Zones")]
    [SerializeField] private TutorialHandZone leftEndZone;
    [SerializeField] private TutorialHandZone rightEndZone;

    [Header("Input")]
    [SerializeField] private InputActionReference leftGripReference;
    [SerializeField] private InputActionReference rightGripReference;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header("Fish")]
    [SerializeField] private GameObject[] tutorialFish;

    [Header("Tutorial Boost")]
    [SerializeField] private float firstBoostForce = 4.5f;
    [SerializeField] private float firstUpwardBoost = 0.1f;

    private TutorialState currentState;
    private LineSwimmer swimmer;
    private bool rockBroken = false;

    private void Start()
    {
        SetStartZonesActive(true);
        SetEndZonesActive(false);
        SetFishActive(false);

        currentState = TutorialState.WaitForStartPose;
        UpdateTutorialText();

        FindSwimmer();

        if (swimmer != null)
        {
            swimmer.SetTutorialMode(true);
        }
        else
        {
            Debug.LogError("NewSwimTutorial could not find LineSwimmer.");
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case TutorialState.WaitForStartPose:
                HandleWaitForStartPose();
                break;

            case TutorialState.WaitForEndPose:
                HandleWaitForEndPose();
                break;

            case TutorialState.WaitForGripRelease:
                HandleWaitForGripRelease();
                break;

            case TutorialState.WaitForRockBreak:
                HandleWaitForRockBreak();
                break;

            case TutorialState.NormalSwimmingWithFish:
                break;
        }
    }

    private void FindSwimmer()
    {
        if (PlayerReferences.instance == null)
        {
            Debug.LogError("PlayerReferences.instance is null.");
            return;
        }

        // This matches the style you were already using.
        swimmer = PlayerReferences.instance.GetComponent<LineSwimmer>();

        // Backup options in case LineSwimmer is on the actual player object or a child.
        if (swimmer == null && PlayerReferences.instance.playerObject != null)
        {
            swimmer = PlayerReferences.instance.playerObject.GetComponent<LineSwimmer>();

            if (swimmer == null)
            {
                swimmer = PlayerReferences.instance.playerObject.GetComponentInChildren<LineSwimmer>();
            }
        }
    }

    private void HandleWaitForStartPose()
    {
        if (AreBothHandsInStartZones() && AreBothGripsPressed())
        {
            SetStartZonesActive(false);
            SetEndZonesActive(true);

            currentState = TutorialState.WaitForEndPose;
            UpdateTutorialText();
        }
    }

    private void HandleWaitForEndPose()
    {
        // If they release early, reset the tutorial stroke.
        if (!AreBothGripsPressed())
        {
            ResetToStartPose();
            return;
        }

        if (AreBothHandsInEndZones())
        {
            currentState = TutorialState.WaitForGripRelease;
            UpdateTutorialText();
        }
    }

    private void HandleWaitForGripRelease()
    {
        if (AreBothGripsReleased())
        {
            SetStartZonesActive(false);
            SetEndZonesActive(false);

            ActivateManualPropulsion();

            currentState = TutorialState.WaitForRockBreak;
            UpdateTutorialText();
        }
    }

    private void HandleWaitForRockBreak()
    {
        if (!rockBroken)
            return;

        // After the rock breaks, let the player swim normally.
        if (swimmer != null)
        {
            swimmer.SetTutorialMode(false);
        }

        SetStartZonesActive(false);
        SetEndZonesActive(false);
        SetFishActive(true);

        currentState = TutorialState.NormalSwimmingWithFish;
        UpdateTutorialText();
    }

    private bool AreBothHandsInStartZones()
    {
        if (leftStartZone == null || rightStartZone == null)
            return false;

        return leftStartZone.IsCorrectHandInside && rightStartZone.IsCorrectHandInside;
    }

    private bool AreBothHandsInEndZones()
    {
        if (leftEndZone == null || rightEndZone == null)
            return false;

        return leftEndZone.IsCorrectHandInside && rightEndZone.IsCorrectHandInside;
    }

    private bool AreBothGripsPressed()
    {
        if (leftGripReference == null || rightGripReference == null)
            return false;

        if (leftGripReference.action == null || rightGripReference.action == null)
            return false;

        return leftGripReference.action.IsPressed() && rightGripReference.action.IsPressed();
    }

    private bool AreBothGripsReleased()
    {
        if (leftGripReference == null || rightGripReference == null)
            return false;

        if (leftGripReference.action == null || rightGripReference.action == null)
            return false;

        return !leftGripReference.action.IsPressed() && !rightGripReference.action.IsPressed();
    }

    private void ResetToStartPose()
    {
        SetStartZonesActive(true);
        SetEndZonesActive(false);

        currentState = TutorialState.WaitForStartPose;
        UpdateTutorialText();
    }

    private void SetStartZonesActive(bool active)
    {
        if (leftStartZone != null)
            leftStartZone.gameObject.SetActive(active);

        if (rightStartZone != null)
            rightStartZone.gameObject.SetActive(active);
    }

    private void SetEndZonesActive(bool active)
    {
        if (leftEndZone != null)
            leftEndZone.gameObject.SetActive(active);

        if (rightEndZone != null)
            rightEndZone.gameObject.SetActive(active);
    }

    private void SetFishActive(bool active)
    {
        if (tutorialFish == null)
            return;

        foreach (GameObject fish in tutorialFish)
        {
            if (fish != null)
                fish.SetActive(active);
        }
    }

    private void UpdateTutorialText()
    {
        if (tutorialText == null) return;

        switch (currentState)
        {
            case TutorialState.WaitForStartPose:
                tutorialText.text = "Put both hands in the starting circles and hold both grip buttons.";
                break;

            case TutorialState.WaitForEndPose:
                tutorialText.text = "Move both hands to the ending circles while still holding grip.";
                break;

            case TutorialState.WaitForGripRelease:
                tutorialText.text = "Now let go of both grip buttons.";
                break;

            case TutorialState.WaitForRockBreak:
                tutorialText.text = "Break through the rock.";
                break;

            case TutorialState.NormalSwimmingWithFish:
                tutorialText.text = "Nice! Now swim normally and catch the fish.";
                break;
        }
    }

    private void ActivateManualPropulsion()
    {
        Debug.Log("Tutorial stroke complete. Triggering controlled tutorial boost.");

        if (swimmer != null)
        {
            swimmer.TriggerTutorialBoost(firstBoostForce, firstUpwardBoost);
        }
    }

    public void OnTutorialRockBroken()
    {
        rockBroken = true;
    }

    public void ForceResetTutorial()
    {
        rockBroken = false;

        if (swimmer != null)
        {
            swimmer.SetTutorialMode(true);
        }

        SetFishActive(false);
        ResetToStartPose();
    }

    public TutorialState GetCurrentState()
    {
        return currentState;
    }
}