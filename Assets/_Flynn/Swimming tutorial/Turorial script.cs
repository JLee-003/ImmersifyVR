using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour
{
    public InputActionAsset inputActionAsset;

    private InputAction leftGrip;
    private InputAction rightGrip;

    public GameObject textObject1;
    public GameObject textObject2;
    public GameObject textObject3; // New GameObject to show when position changes
    public GameObject monitoredObject; // The object whose position we are monitoring

    private Vector3 lastPosition;

    private void Awake()
    {
        // Load the Input Action Asset
        inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Flynn/Swimming tutorial/XRInputActions.inputactions");

        // Get references to the input actions
        leftGrip = inputActionAsset.FindAction("LeftGrip");
        rightGrip = inputActionAsset.FindAction("RightGrip");

        // Initialize lastPosition with the initial position of the monitored object
        if (monitoredObject != null)
        {
            lastPosition = monitoredObject.transform.position;
        }
    }

    private void OnEnable()
    {
        leftGrip.Enable();
        rightGrip.Enable();
    }

    private void OnDisable()
    {
        leftGrip.Disable();
        rightGrip.Disable();
    }

    void Update()
    {
        if (leftGrip.IsPressed() && rightGrip.IsPressed())
        {
            textObject1.SetActive(false);
            textObject2.SetActive(true);
        }
        else
        {
            textObject1.SetActive(true);
            textObject2.SetActive(false);
        }

        // Check if the position of the monitored object has changed
        if (monitoredObject != null && monitoredObject.transform.position != lastPosition)
        {
            textObject2.SetActive(false);
            textObject3.SetActive(true);

            // Update lastPosition to the new position
            lastPosition = monitoredObject.transform.position;
        }

    }
}
