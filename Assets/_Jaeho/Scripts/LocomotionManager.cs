using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    ActionBasedContinuousMoveProvider continuousMoveProvider;
    LineSwimmer SwimmingEvaluator;
    float waterWalkSpeed = 1.5f;
    float normalWalkSpeed = 3f;

    [SerializeField] AudioClip waterEnterAudio;
    private void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        SwimmingEvaluator = GetComponent<LineSwimmer>();

        continuousMoveProvider.moveSpeed = normalWalkSpeed;
        SwimmingEvaluator.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = waterWalkSpeed;
            SwimmingEvaluator.enabled = true;
            Physics.gravity = new Vector3(0f, -0.0005f, 0f);

            AudioSource.PlayClipAtPoint(waterEnterAudio, transform.position, 1f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            continuousMoveProvider.moveSpeed = normalWalkSpeed;
            SwimmingEvaluator.enabled = false;
            Physics.gravity = new Vector3(0f, -9.8f, 0f);
        }
    }
}
