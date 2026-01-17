using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReferences : MonoBehaviour
{
    public static PlayerReferences instance;

    public GameObject playerObject;
    public Transform cameraTransform;
    public LineSwimmer swimObject;
    public InputActionReference leftControllerSwimReference;
    public InputActionReference rightControllerSwimReference;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
