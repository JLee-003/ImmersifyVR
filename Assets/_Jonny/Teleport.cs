using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public static Teleport Instance { get; private set; }
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
    
    public void tp(int x, int y, int z) {
        transform.position = new Vector3(x, y, z);
    }

    public void tpToSpecificPos() {
        tp(100,20,100);
    }

    void Update() {
    }
}
 