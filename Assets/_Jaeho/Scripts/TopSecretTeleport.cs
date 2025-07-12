using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class TopSecretTeleport : MonoBehaviour
{
    private InputDevice rightController;

    float teleportTimer = 0f;

    void Start()
    {
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
        }
    }

    void Update()
    {
        if (rightController.isValid)
        {
            bool aButtonPressed = false;
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out aButtonPressed) && aButtonPressed)
            {
                Debug.Log("A button pressed");
            }

            bool bButtonPressed = false;
            if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bButtonPressed) && bButtonPressed)
            {
                Debug.Log("B button pressed");
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
