using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterableBehaviour : MonoBehaviour
{
   public GameObject shatteredVersion;

   void OnMouseDown()
    {
        Instantiate(shatteredVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
