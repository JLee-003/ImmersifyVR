using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class SwimmingTutorial : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Change Text to TextMeshProUGUI
    public GameObject greenPoint;
    public GameObject firstFish;
    public GameObject secondFish;
    //public GameObject objectToShow;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button triggerButton = UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger;
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    private GameObject player;

    private bool hasReachedGreenPoint = false;
    private bool hasCaughtFirstFish = false;

    void Start()
    {
        greenPoint.SetActive(false);
        firstFish.SetActive(false);
        secondFish.SetActive(false);
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    void Update()
    {
        if (leftControllerSwimReference.action.IsPressed() || rightControllerSwimReference.action.IsPressed())
        {
            Debug.Log("Grip button pressed"); // Debug log
            tutorialText.text = "Now, swim to the green sphere.";
            greenPoint.SetActive(true);
        }
        // Green Point
        if (player != null && !hasReachedGreenPoint && Vector3.Distance(player.transform.position, greenPoint.transform.position) < 5.0f)
        {
            hasReachedGreenPoint = true;
            greenPoint.SetActive(false);
            firstFish.SetActive(true);

            tutorialText.text = "Well done! Now, try to catch the fish in front of you by touching it with your hand.";
        }
        // First Stationary Fish
        if (player != null && hasReachedGreenPoint && !hasCaughtFirstFish && firstFish == null)
        {
            hasCaughtFirstFish = true;
            secondFish.SetActive(true);

            tutorialText.text = "Finally, catch this moving fish!";
        }
        // Second Moving Fish
        if (player != null && hasReachedGreenPoint && hasCaughtFirstFish && secondFish == null)
        {
            tutorialText.text = "Great Job! You're now ready for the open ocean.";

            StartCoroutine(WaitAndLoadScene()); // Ensure WaitAndLoadScene is called here
        }
    }

    private IEnumerator WaitAndLoadScene()
    {
        
        if (SceneLoader.Instance != null)
        {
            Teleport.Instance.tp(72f,32f,56f);
            Debug.Log("SceneLoader instance found. Loading scene 'NewBeach'...");
            SceneLoader.Instance.LoadNewScene("Main VISUALS");
        }
        else
        {
            Debug.LogError("SceneLoader instance is null. Cannot load scene.");
        }
        yield return new WaitForSeconds(3);
    }
}
