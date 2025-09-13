using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTracker : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            Physics.gravity = new Vector3(0f, -9.8f, 0f);
        }
    }

}
