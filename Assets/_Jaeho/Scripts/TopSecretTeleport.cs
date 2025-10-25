using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TopSecretTeleport : MonoBehaviour
{
    [SerializeField] InputActionReference xButtonAction;
    [SerializeField] InputActionReference yButtonAction;
    [SerializeField] InputActionReference aButtonAction;
    [SerializeField] InputActionReference bButtonAction;

    float teleportTimer = 0f;
    float restartTimer = 0f;

    LineSwimmer swimmer;
    ActionBasedContinuousMoveProvider continuousMoveProvider;

    private BackgroundMusic _bgm;

    /*void Start()
    {
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
        }
    }*/

    private void Start()
    {
        continuousMoveProvider = PlayerReferences.instance.playerObject.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        swimmer = PlayerReferences.instance.playerObject.GetComponent<LineSwimmer>();

        _bgm = FindObjectOfType<BackgroundMusic>();
    }

    void Update()
    {
        bool xButtonPressed = false;
        if (xButtonAction.action.IsPressed())
        {
            Debug.Log("X button pressed");
            xButtonPressed = true;
        }

        bool yButtonPressed = false;
        if (yButtonAction.action.IsPressed())
        {
            Debug.Log("Y button pressed");
            yButtonPressed = true;
        }

        if (xButtonPressed && yButtonPressed)
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

        // restart game if A/B Button pressed

        bool aButtonPressed = false;
        if (aButtonAction.action.IsPressed())
        {
            Debug.Log("A button pressed");
            aButtonPressed = true;
        }

        bool bButtonPressed = false;
        if (bButtonAction.action.IsPressed())
        {
            Debug.Log("B button pressed");
            bButtonPressed = true;
        }

        if (aButtonPressed && bButtonPressed)
        {
            restartTimer += Time.deltaTime;
        }

        else
        {
            restartTimer = 0f;
        }

        if (restartTimer >= 5f)
        {
            restartTimer = 0f;

            RestartGame();
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
        else if (currentScene == "Swimming Game")
        {
            sceneName = "TennisGameTest";
        }
        else
        {
            sceneName = "Lobby";
        }
        continuousMoveProvider.moveSpeed = 3f;
        swimmer.enabled = false;
        Physics.gravity = new Vector3(0f, -9.8f, 0f);
        continuousMoveProvider.useGravity = true;
        _bgm?.SetUnderwater(false);

        SceneLoader.Instance.LoadNewScene(sceneName);
    }
    
    void RestartGame()
    {
        string sceneName = "VRTutorial";

        continuousMoveProvider.moveSpeed = 3f;
        swimmer.enabled = false;
        Physics.gravity = new Vector3(0f, -9.8f, 0f);
        continuousMoveProvider.useGravity = true;
        _bgm?.SetUnderwater(false);

        SceneLoader.Instance.LoadNewScene(sceneName);
    }
}
