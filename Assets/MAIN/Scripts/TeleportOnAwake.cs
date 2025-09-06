using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class TeleportOnAwake : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] XROrigin xrOrigin;
    
    private void Start()
    {
        // Get the XROrigin if not assigned
        if (xrOrigin == null)
        {
            xrOrigin = FindObjectOfType<XROrigin>();
        }
        
        if (xrOrigin != null)
        {
            // Teleport the XROrigin to the spawn point position
            xrOrigin.transform.position = spawnPoint.position;
            
            // Reset rotation to face the spawn point's forward direction:
            xrOrigin.transform.rotation = Quaternion.Euler(0f, spawnPoint.eulerAngles.y, 0f);
        }
        else
        {
            // Fallback to the old method if XROrigin is not found
            Teleport.Instance.tp(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z, spawnPoint.eulerAngles.y);
        }
    }
}
