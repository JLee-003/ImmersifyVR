using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadSceneOnGrab()
    {
        SceneLoader.Instance.LoadNewScene("ThirdScene");
        Debug.Log("Grabbing!");
    }

    public void ColorChangeOnHover()
    {
        GetComponent<Renderer>().material.color = new Color(0, 204, 102);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
