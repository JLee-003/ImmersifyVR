using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFish : MonoBehaviour
{
    [SerializeField] AudioClip removeAudio;
    
    [SerializeField] private float maxYLevel;

    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;

    [SerializeField] private float maxTime;
    [SerializeField] private float smoothTime;

    [SerializeField] private List<Vector3> checkpoints;
    [SerializeField] private float checkpointThreshold = 1.5f;

    [SerializeField] private bool stationary = false;

    [SerializeField] private SwimmingTutorial tutorialManager;

    private Rigidbody rb;
    private float timer;
    private int currentCheckpointIndex = 0;

    public GameObject catchEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y >= maxYLevel)
        {
            var curPos = transform.position;
            curPos.y = maxYLevel;
            transform.position = curPos;

            var curVel = rb.velocity;
            if (curVel.y > 0) curVel.y = 0;
            rb.velocity = curVel;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rb.velocity), Time.deltaTime * smoothTime);
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (rb.velocity.magnitude <= 0.8f || timer <= 0)
        {
            Move();
        }
    }

    private void Move()
    {
        if (!stationary && checkpoints != null && checkpoints.Count > 0)
        {
            // Determine the target checkpoint.
            Vector3 target = checkpoints[currentCheckpointIndex];
            Vector3 direction = target - transform.position;

            // If the fish is close enough to the checkpoint, switch to the next one.
            if (direction.sqrMagnitude < checkpointThreshold * checkpointThreshold)
            {
                currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
                target = checkpoints[currentCheckpointIndex];
                direction = target - transform.position;
            }

            float randForce = Random.Range(minForce, maxForce);

            rb.AddForce(direction.normalized * randForce, ForceMode.Impulse);
            timer = maxTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            AudioSource.PlayClipAtPoint(removeAudio, transform.position, 1f);
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, true, 1f, 1f);

            tutorialManager.tutorialFishCaught++;

            // Instantiate the particle effect
            if (catchEffect != null)
            {
                GameObject effect = Instantiate(catchEffect, transform.position, Quaternion.identity);
                Debug.Log("CAUGHT!");
            }

            Destroy(gameObject);
        }
    }
}
