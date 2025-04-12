using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        // Check if XR Origin has entered the portal
        if (other.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadNewScene("FishActivityTutorial");
        }
    }


}
