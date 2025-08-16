using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceballTutorial : MonoBehaviour
{
    [Header("References")]
    public GameObject tennisBall;
    public TextMeshProUGUI tutorialText;

    [Header("Settings")]
    public float triggerDistance = 1.0f;
    public float velocityThreshold = 0.1f;

    private Transform player;
    private Rigidbody ballRb;
    private bool ballInMotion = false;
    private bool tutorialEnded = false; 

    void Start()
    {
        player = PlayerReferences.instance.playerObject.transform;

        if (tennisBall != null)
            ballRb = tennisBall.GetComponent<Rigidbody>();

        if (tutorialText != null)
            tutorialText.text = "Move closer to the ball";
    }

    void Update()
    {
        if (tutorialEnded || player == null || tennisBall == null || tutorialText == null) return; 

        if (ballRb != null)
        {
            if (!ballInMotion && ballRb.velocity.magnitude > velocityThreshold)
            {
                tutorialText.text = "Nice! Try to make the enemy miss!";
                ballInMotion = true;
            }
            else if (ballInMotion && ballRb.velocity.magnitude <= velocityThreshold)
            {
                ballInMotion = false;
            }
        }

        if (!ballInMotion)
        {
            float distance = Vector3.Distance(player.position, tennisBall.transform.position);

            if (distance <= triggerDistance)
                tutorialText.text = "Now swing your arm forward to hit the ball";
            else
                tutorialText.text = "Move closer to the ball";
        }
    }

    public void EndTutorial()
    {
        tutorialEnded = true;
    }
}
