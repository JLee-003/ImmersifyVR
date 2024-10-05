using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public void tp(int x, int y, int z) {
        transform.position = new Vector3(x, y, z);
    }

    void Update() {
        tp(100,100,100);
    }
}
 