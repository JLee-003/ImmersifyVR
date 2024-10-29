using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SwimmingEvaluator : MonoBehaviour
{
    ActionBasedController controller;
    CharacterController characterController;
    [SerializeField] Transform head;

    bool measuring = false;

    // Start is called before the first frame update
    void Start()
    {
        characterController = head.GetComponent<CharacterController>();
        controller = GetComponent<ActionBasedController>();

    }

    // Update is called once per frame
    void Update()
    {
        // Conditional: if controller is moving above X speed, start measuring

        // Else if controller is measuring (and not moving above X speed), stop measuring and apply speed boost
    }

    void CalculateSpeedBoost() {
        // Calculate current controller velocity

        // Flip current controller velocity to opposite direction


    }

    void ApplyBoost() {
        // Apply boost to character controller
    }
}
