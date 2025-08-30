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
        transform.position = new Vector3(x, y, z);
        float lookY = lookYDeg ?? transform.eulerAngles.x;

        transform.rotation = Quaternion.Euler(0f, lookY, 0f);
        //xrOrigin.MoveCameraToWorldLocation(new Vector3(x, y, z));
        Debug.Log($"Teleporting to {x}, {y}, {z}");
        Debug.Log(transform.position);
        await Fader.Instance.FadeOut();
    }
}
 