using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTracker : MonoBehaviour
{
    LineSwimmer SwimmingEvaluator;
    private void Start()
    {
        SwimmingEvaluator = GetComponent<LineSwimmer>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            SwimmingEvaluator.enabled = false;
            Physics.gravity = new Vector3(0f, -9.8f, 0f);

            Debug.Log("Delay exited water volume.");
        }
    }

}
