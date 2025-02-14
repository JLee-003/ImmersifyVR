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
    public GameObject objectToShow;
    public UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button triggerButton = UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger;
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    private GameObject player;

    private bool hasReachedGreenPoint = false;

    void Start()
    {
        greenPoint.SetActive(false);
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
            tutorialText.text = "Now swim to the green point";
            greenPoint.SetActive(true);
        }

        if (player != null && !hasReachedGreenPoint && Vector3.Distance(player.transform.position, greenPoint.transform.position) < 5.0f)
        {
            hasReachedGreenPoint = true;
            objectToShow.SetActive(true);
            greenPoint.SetActive(false);
            StartCoroutine(WaitAndLoadScene()); // Ensure WaitAndLoadScene is called here
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasReachedGreenPoint = true;
            objectToShow.SetActive(true);
            greenPoint.SetActive(false);
            StartCoroutine(WaitAndLoadScene());
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
