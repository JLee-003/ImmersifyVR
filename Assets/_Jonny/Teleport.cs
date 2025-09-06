using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
public class Teleport : MonoBehaviour
{
    public static Teleport Instance { get; private set; }
    [SerializeField] XROrigin xrOrigin;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public async void tp(float x, float y, float z, float? lookYDeg = null) {
        await Fader.Instance.FadeIn();
        
        // Set position
        transform.position = new Vector3(x, y, z);
        
        // Set rotation - use the provided Y rotation or current Y rotation
        float lookY = lookYDeg ?? transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, lookY, 0f);
        
        // Apply rotation to the XROrigin's base transform
        if (xrOrigin != null)
        {
            xrOrigin.transform.rotation = Quaternion.Euler(0f, lookY, 0f);
        }
        
        Debug.Log($"Teleporting to {x}, {y}, {z} with rotation Y: {lookY}");
        Debug.Log($"Player position: {transform.position}, Player rotation: {transform.rotation.eulerAngles}");
        await Fader.Instance.FadeOut();
    }
}
 