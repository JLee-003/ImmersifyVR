using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class TopSecretTeleport : MonoBehaviour
{
    [SerializeField] InputActionReference rightAButtonAction;
    [SerializeField] InputActionReference rightBButtonAction;

    float teleportTimer = 0f;

    /*void Start()
    {
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
        }
    }*/

    void Update()
    {
        bool aButtonPressed = false;
        if (rightAButtonAction.action.IsPressed())
        {
            Debug.Log("A button pressed");
            aButtonPressed = true;
        }

        bool bButtonPressed = false;
        if (rightBButtonAction.action.IsPressed())
        {
            Debug.Log("B button pressed");
            bButtonPressed = true;
        }

        if (aButtonPressed && bButtonPressed)
        {
            teleportTimer += Time.deltaTime;
        }

        else
        {
            teleportTimer = 0f;
        }

        if (teleportTimer >= 5f)
        {
            teleportTimer = 0f;

            TeleportToOtherScene();
        }
    }

    void TeleportToOtherScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string sceneName;
        if (currentScene == "TennisGameTest")
        {
            sceneName = "FishActivityTutorial";
        }
        else if(currentScene == "MAIN Visuals")
        {
            sceneName = "TennisGameTest";
        }
        else
        {
            sceneName = "Lobby";
        }

        SceneLoader.Instance.LoadNewScene(sceneName);
    }
}
