using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    ActionBasedContinuousMoveProvider continuousMoveProvider;
    BasicSwimmingEvaluator[] BasicSwimmingEvaultors;
    float waterWalkSpeed = 1.5f;
    float normalWalkSpeed = 3f;
    private void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        BasicSwimmingEvaultors = GetComponentsInChildren<BasicSwimmingEvaluator>();

        continuousMoveProvider.moveSpeed = normalWalkSpeed;
        BasicSwimmingEvaultors[0].enabled = false;
        BasicSwimmingEvaultors[1].enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = waterWalkSpeed;
            BasicSwimmingEvaultors[0].enabled = true;
            BasicSwimmingEvaultors[1].enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = normalWalkSpeed;
            BasicSwimmingEvaultors[0].enabled = false;
            BasicSwimmingEvaultors[1].enabled = false;
        }
    }
}
