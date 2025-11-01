using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class SwimmingTutorial : MonoBehaviour
{
    public GameObject greenPoint;
    GameObject[] tutorialFish;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button triggerButton = UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger;
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] TextMeshProUGUI LandText;
    [SerializeField] TextMeshProUGUI swimMotionText;
    private GameObject player;

    ActionBasedContinuousMoveProvider continuousMoveProvider;


    ActionBasedContinuousMoveProvider moveProvider;

    private bool gripButtonPressed = false;
    private bool hasReachedGreenPoint = false;
    private bool loadedScene = false;
    private bool handsMoved = false;
    private bool gripReleased = false;
    private Vector3 leftControllerStartPosition;
    private Vector3 rightControllerStartPosition;
    private CharacterController playerCharacterController;
    private GameObject leftController;
    private GameObject rightController;

    public int tutorialFishCaught = 0;
    int fishCaughtRequirement = 3;

    void Start()
    {
        greenPoint.SetActive(false);
        tutorialFish = GameObject.FindGameObjectsWithTag("Fish");
        player = GameObject.FindWithTag("Player");
        foreach (GameObject fish in tutorialFish)
        {
            fish.SetActive(false);
        }

        // Initialize LandText with initial message
        if (LandText != null)
        {
            LandText.text = "To move, gently push the left joystick in the direction you want to walk.";
        }

        // Initialize swimMotionText with initial message
        if (swimMotionText != null)
        {
            swimMotionText.text = "Push the grip buttons on both controllers to start the action";
        }


        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
        else
        {
            moveProvider = player.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        }

        // Get player CharacterController from PlayerReferences for velocity tracking
        if (PlayerReferences.instance != null && PlayerReferences.instance.playerObject != null)
        {
            playerCharacterController = PlayerReferences.instance.playerObject.GetComponent<CharacterController>();
        }
        else
        {
            Debug.LogError("PlayerReferences instance or playerObject not found.");
        }

        // Find left and right controller references by tag
        GameObject[] leftHands = GameObject.FindGameObjectsWithTag("LeftHand");
        GameObject[] rightHands = GameObject.FindGameObjectsWithTag("RightHand");

        if (leftHands.Length > 0)
        {
            leftController = leftHands[0]; // Take the first one found
        }
        else
        {
            Debug.LogError("No GameObject with 'LeftHand' tag found.");
        }

        if (rightHands.Length > 0)
        {
            rightController = rightHands[0]; // Take the first one found
        }
        else
        {
            Debug.LogError("No GameObject with 'RightHand' tag found.");
        }
    }


    void Update()
    {
        // Check for player velocity to change LandText dynamically
        if (LandText != null && playerCharacterController != null)
        {
            Vector3 velocity = playerCharacterController.velocity;
            float velocityMagnitude = velocity.magnitude;

            if (velocityMagnitude > 0.1f) // Check if player is moving
            {
                LandText.text = "Try to walk into the water!";
            }
            else // Player is stationary
            {
                LandText.text = "To move, gently push the left joystick in the direction you want to walk.";
            }
        }

        if (!gripButtonPressed)
        {
            bool leftGripPressed = leftControllerSwimReference.action.IsPressed();
            bool rightGripPressed = rightControllerSwimReference.action.IsPressed();

            if (leftGripPressed || rightGripPressed)
            {
                gripButtonPressed = true;
                greenPoint.SetActive(true);

                leftControllerStartPosition = leftController.transform.localPosition;
                rightControllerStartPosition = rightController.transform.localPosition;

                // Change text when grip is pressed
                if (swimMotionText != null && !handsMoved)
                {
                    swimMotionText.text = "Move both your hands in any direction";
                }
            }

        }

        // Check for hand movement when grip is pressed
        if (gripButtonPressed && !handsMoved)
        {
            if (leftController != null && rightController != null)
            {
                // Calculate distancce using local position changes
                Vector3 leftCurrentPosition = leftController.transform.localPosition;
                Vector3 rightCurrentPosition = rightController.transform.localPosition;

                float leftDist = (leftCurrentPosition - leftControllerStartPosition).magnitude;
                float rightDist = (rightCurrentPosition - rightControllerStartPosition).magnitude;

                if (leftDist > 0.3f || rightDist > 0.3f)
                {
                    handsMoved = true;
                    if (swimMotionText != null)
                    {
                        swimMotionText.text = "Let go of the grip button after your stroke to finish the action";
                    }
                }
            }
        }
        // Check for grip release after hands have moved

        bool leftGripReleased = !leftControllerSwimReference.action.IsPressed();
        bool rightGripReleased = !rightControllerSwimReference.action.IsPressed();

        if (leftGripReleased && rightGripReleased)
        {
            gripButtonPressed = false;

            handsMoved = false;

            if (swimMotionText != null)
            {
                swimMotionText.text = "Push the grip buttons on both controllers to start the action";
            }
        }



        // Green Point
        if (player != null && !hasReachedGreenPoint && Vector3.Distance(player.transform.position, greenPoint.transform.position) < 3f)
        {
            hasReachedGreenPoint = true;
            greenPoint.SetActive(false);
            foreach (GameObject fish in tutorialFish)
            {
                fish.SetActive(true);
            }
            moveProvider.moveSpeed = 0;
        }


        if (player != null && tutorialFishCaught >= fishCaughtRequirement)
        {
            StartCoroutine(WaitAndLoadScene()); // Ensure WaitAndLoadScene is called here
            loadedScene = true;
            //Manually run water-exit code
            /*moveProvider.moveSpeed = 3f;
            swimmer.enabled = false;
            moveProvider.useGravity = true;*/
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(3);
        if (SceneLoader.Instance != null)
        {
            Debug.Log("SceneLoader instance found. Loading scene 'Swimming Game'...");
            //Manually run water-exit code; again, just in case
            LocomotionManager.Instance.ResetLandDefaults();

            SceneLoader.Instance.LoadNewScene("Swimming Game");
        }
        else
        {
            Debug.LogError("SceneLoader instance is null. Cannot load scene.");
        }
    }
}
