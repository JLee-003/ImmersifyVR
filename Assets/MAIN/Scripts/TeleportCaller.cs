using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCaller : MonoBehaviour
{
    public void tpToSpecificPos() { // MOVE TO SEPARATE SCRIPT
        Teleport.Instance.tp(100,20,100);
    }

    void Update() {
    }
}
 