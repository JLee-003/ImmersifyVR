using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for TextMeshPro

public class TennisTutorialBackWall : MonoBehaviour
{
    [Header("References")]
    public Transform resetPoint;
    public TextMeshProUGUI tutorialText; 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Reset position & velocity
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            collision.transform.position = resetPoint.position;
            collision.transform.rotation = resetPoint.rotation;

            if (ballRb != null)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }

            StartCoroutine(EndTutorialSequence());
        }
    }

private IEnumerator EndTutorialSequence()
{
    if (tutorialText != null)
    {
        tutorialText.text = "Nice! Now you're ready for the real thing!";
    }

    FindObjectOfType<SpaceballTutorial>()?.EndTutorial();

    yield return new WaitForSeconds(3f);

    SceneLoader.Instance.LoadNewScene("TennisGameTest");
}
}
