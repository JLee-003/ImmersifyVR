using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterParticles : MonoBehaviour
{
    private bool isLeftHand;

    private HandBoosters handBoosters;
    private Transform handTransform;
    private Transform xrOriginCamera;
    private ParticleSystem particles;

    void Start()
    {



        // Check parent tag to determine if this is left or right hand
        Transform parentHand = transform.parent;
        if (parentHand != null)
        {
            if (parentHand.CompareTag("LeftHand"))
            {
                isLeftHand = true;
                handTransform = parentHand;
            }
            else if (parentHand.CompareTag("RightHand"))
            {
                isLeftHand = false;
                handTransform = parentHand;
            }
            else
            {
                Debug.LogWarning($"BoosterParticles: Parent '{parentHand.name}' does not have LeftHand or RightHand tag!");
            }
        }
        else
        {
            Debug.LogWarning("BoosterParticles: No parent found!");
        }

        // Find HandBoosters component
        handBoosters = FindObjectOfType<HandBoosters>();
        if (handBoosters == null)
        {
            Debug.LogWarning("BoosterParticles: HandBoosters component not found!");
        }

        // Get XR origin camera from PlayerReferences
        if (PlayerReferences.instance != null && PlayerReferences.instance.cameraTransform != null)
        {
            xrOriginCamera = PlayerReferences.instance.cameraTransform;
        }
        else
        {
            Debug.LogWarning("BoosterParticles: PlayerReferences.instance or cameraTransform not found!");
        }

        // Find particle system as child of this GameObject
        particles = GetComponentInChildren<ParticleSystem>();
        if (particles == null)
        {
            Debug.LogWarning($"BoosterParticles: No ParticleSystem found as child of {gameObject.name}!");
        }
        else
        {
            particles.Play(); // initialize
            var emission = particles.emission;
            emission.enabled = false; // start disabled
        }


    }

    void Update()
    {
        if (handBoosters == null || xrOriginCamera == null || handTransform == null || particles == null) return;

        // Check if this hand is boosting
        bool isBoosting = isLeftHand ? handBoosters.IsLeftBoosting : handBoosters.IsRightBoosting;

        if (isBoosting)
        {
            // Calculate direction from XR origin (camera) toward this hand
            Vector3 direction = (handTransform.position - xrOriginCamera.position).normalized;

            // Position particles at the hand
            particles.transform.position = handTransform.position;

            // Rotate particles to emit away from XR origin (toward hand)
            particles.transform.rotation = Quaternion.LookRotation(direction);

            // Enable emission
            var emission = particles.emission;
            emission.enabled = true;

            Debug.Log("PARTICLES TURNED ON");
        }
        else
        {
            // Disable emission when not boosting
            Debug.Log("PARTICLES TURNED OFF");
            var emission = particles.emission;
            emission.enabled = false;
        }
    }
}
