using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    LineSwimmer swimmer;
    float velocityRequired = 8f;
    float velocityTaken = 0f;

    Renderer rend;
    Color originalColor;
    Color crackedRock = new Color(0.9f, 0.87f, 0.82f);
    private void Start()
    {
        swimmer = PlayerReferences.instance.GetComponent<LineSwimmer>();
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Rock Hit with velocity " + swimmer.velocity.magnitude);
            velocityTaken += swimmer.velocity.magnitude;
            swimmer.velocity *= -0.5f;

            float t = Mathf.Clamp01(velocityTaken / velocityRequired);
            rend.material.color = Color.Lerp(originalColor, crackedRock, t);

            if (velocityTaken >= velocityRequired)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Collided with " + other.gameObject.tag);
        }
    }
}
