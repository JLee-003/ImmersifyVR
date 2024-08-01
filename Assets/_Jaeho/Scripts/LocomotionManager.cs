using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    ActionBasedContinuousMoveProvider continuousMoveProvider;
    BreastStrokeEvaluator[] breastStrokeEvaultors;
    private void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        breastStrokeEvaultors = GetComponents<BreastStrokeEvaluator>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.enabled = false;
            breastStrokeEvaultors[0].enabled = true;
            breastStrokeEvaultors[1].enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        { 
            continuousMoveProvider.enabled = true;
            breastStrokeEvaultors[0].enabled = false;
            breastStrokeEvaultors[1].enabled = false;
        }
    }
}
