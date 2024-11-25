using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShaderBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        var bounds = meshRenderer.bounds;
        bounds.Expand(new Vector3(500, 500, 500));
        meshRenderer.bounds = bounds;

    }
}
