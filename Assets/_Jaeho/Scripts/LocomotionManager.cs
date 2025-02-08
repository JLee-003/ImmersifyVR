using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    ActionBasedContinuousMoveProvider continuousMoveProvider;
    Swimmer SwimmingEvaluator;
    float waterWalkSpeed = 1.5f;
    float normalWalkSpeed = 3f;
    private void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        SwimmingEvaluator = GetComponent<Swimmer>();

        continuousMoveProvider.moveSpeed = normalWalkSpeed;
        SwimmingEvaluator.enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = waterWalkSpeed;
            SwimmingEvaluator.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = normalWalkSpeed;
            SwimmingEvaluator.enabled = false;
        }
    }
}
