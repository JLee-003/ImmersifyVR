using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeControllerTracking : MonoBehaviour
{
    public Transform tracked; // your XR controller transform

    void FixedUpdate()
    {
        if (!tracked) return;
        transform.SetPositionAndRotation(tracked.position, tracked.rotation);
    }
}
