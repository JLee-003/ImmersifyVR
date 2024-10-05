using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int type;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Destroy(gameObject);
        }
    }
}
