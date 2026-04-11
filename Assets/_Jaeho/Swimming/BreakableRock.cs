using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    LineSwimmer swimmer;
    private void Start()
    {
        swimmer = PlayerReferences.instance.GetComponent<LineSwimmer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Rock Broken");
            swimmer.velocity *= -0.5f;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Collided with " + other.gameObject.tag);
        }
    }
}
