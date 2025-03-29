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
    public GameObject firstFish;
    public GameObject secondFish;
    public GameObject thirdFish;
    public GameObject finalFish;
    //public GameObject objectToShow;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button triggerButton = UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger;
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    private GameObject player;

    ActionBasedContinuousMoveProvider moveProvider;

    private bool gripButtonPressed = false;
    private bool hasReachedGreenPoint = false;
    private bool hasCaughtFirstFish = false;
    private bool hasCaughtSecondFish = false;
    private bool hasCaughtThirdFish = false;
    private bool loadedScene = false;

    void Start()
    {
        greenPoint.SetActive(false);
        firstFish.SetActive(false);
        secondFish.SetActive(false);
        thirdFish.SetActive(false);
        finalFish.SetActive(false);
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
        else
        {
            moveProvider = player.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
            moveProvider.moveSpeed = 0;
        }
    }

    void Update()
    {
        if (!hasReachedGreenPoint && leftControllerSwimReference.action.IsPressed() || rightControllerSwimReference.action.IsPressed())
        {
            //Debug.Log("Grip button pressed"); // Debug log
            gripButtonPressed = true;
            greenPoint.SetActive(true);
        }
        // Green Point
        if (player != null && gripButtonPressed == true && !hasReachedGreenPoint && Vector3.Distance(player.transform.position, greenPoint.transform.position) < 1.0f)
        {
            hasReachedGreenPoint = true;
            greenPoint.SetActive(false);
            firstFish.SetActive(true);
        }
        if (player != null && hasReachedGreenPoint && !hasCaughtFirstFish && firstFish == null)
        {
            hasCaughtFirstFish = true;
            secondFish.SetActive(true);
        }
        if (player != null && hasCaughtFirstFish && !hasCaughtSecondFish && secondFish == null)
        {
            hasCaughtSecondFish = true;
            thirdFish.SetActive(true);
        }
        if (player != null && hasCaughtSecondFish && !hasCaughtThirdFish && thirdFish == null)
        {
            hasCaughtThirdFish = true;
            finalFish.SetActive(true);
        }
        if (player != null && hasCaughtThirdFish && !loadedScene && finalFish == null)
        {
            StartCoroutine(WaitAndLoadScene()); // Ensure WaitAndLoadScene is called here
            loadedScene = true;
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(3);
        if (SceneLoader.Instance != null)
        {
            Debug.Log("SceneLoader instance found. Loading scene 'NewBeach'...");
            moveProvider.moveSpeed = 3f;
            SceneLoader.Instance.LoadNewScene("Main VISUALS");
        }
        else
        {
            Debug.LogError("SceneLoader instance is null. Cannot load scene.");
        }
    }
}
