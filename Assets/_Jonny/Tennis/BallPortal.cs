using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPortal : MonoBehaviour
{
    [SerializeField] private Vector3 respawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                Vector3 velocity = ballRb.velocity; // Store current velocity

                // Teleport the ball
                ballRb.position = respawnPosition;
                ballRb.velocity = velocity; // Restore velocity
            }
        }
    }

}
