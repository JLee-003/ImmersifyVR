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
    private GameObject player;

    ActionBasedContinuousMoveProvider moveProvider;

    private bool gripButtonPressed = false;
    private bool hasReachedGreenPoint = false;
    private bool loadedScene = false;
    LineSwimmer swimmer;

    public int tutorialFishCaught = 0;
    int fishCaughtRequirement = 4;

    void Start()
    {
        greenPoint.SetActive(false);
        tutorialFish = GameObject.FindGameObjectsWithTag("Fish");
        player = GameObject.FindWithTag("Player");
        foreach (GameObject fish in tutorialFish)
        {
            fish.SetActive(false);
        }
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
        else
        {
            swimmer = player.GetComponent<LineSwimmer>();
            moveProvider = player.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        }
    }

    void Update()
    {
        if (!hasReachedGreenPoint && leftControllerSwimReference.action.IsPressed() || rightControllerSwimReference.action.IsPressed())
        {
            gripButtonPressed = true;
            greenPoint.SetActive(true);
        }
        // Green Point
        if (player != null && gripButtonPressed == true && !hasReachedGreenPoint && Vector3.Distance(player.transform.position, greenPoint.transform.position) < 1.0f)
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
            moveProvider.moveSpeed = 3f;
            swimmer.enabled = false;
            Physics.gravity = new Vector3(0f, -9.8f, 0f);
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(3);
        if (SceneLoader.Instance != null)
        {
            Debug.Log("SceneLoader instance found. Loading scene 'Swimming Game'...");
            //Manually run water-exit code; again, just in case
            moveProvider.moveSpeed = 3f;
            swimmer.enabled = false;
            Physics.gravity = new Vector3(0f, -9.8f, 0f);
            SceneLoader.Instance.LoadNewScene("Swimming Game");
        }
        else
        {
            Debug.LogError("SceneLoader instance is null. Cannot load scene.");
        }
    }
}
