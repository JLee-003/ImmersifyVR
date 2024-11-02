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

    private void Awake()
    {
        // Load the Input Action Asset
        inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Flynn/Swimming tutorial/XRInputActions.inputactions");

        // Get references to the input actions
        leftGrip = inputActionAsset.FindAction("LeftGrip");
        rightGrip = inputActionAsset.FindAction("RightGrip");
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
    }
}