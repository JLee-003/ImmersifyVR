using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleObject : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f); // Destroy this object after 5 seconds
    }
}
