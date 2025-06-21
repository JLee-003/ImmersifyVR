using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravProjectile : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ChangeVelocity(Vector3 newVelocity)
    {
        rb.velocity = newVelocity;
    }
}
