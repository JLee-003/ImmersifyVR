using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadSceneOnGrab()
    {
        SceneLoader.Instance.LoadNewScene("Scene Loader");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
