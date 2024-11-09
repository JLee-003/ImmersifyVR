using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialCheckpoints : MonoBehaviour
{
    [SerializeField] Transform[] checkpointColliders;
    [SerializeField] Transform door;
    GameObject player;
    Camera playerView;

    float checkpointRadius = 2.2f;

    int totalCheckpoints;
    int checkpointsReached = 0;

    float doorSpeed = 0.75f;

    float angleThreshold = 40f;

    [SerializeField] TextMeshProUGUI checkpointsText;

    private void Start()
    {
        totalCheckpoints = checkpointColliders.Length;

        //Get player transform
        player = GameObject.FindGameObjectWithTag("Player");
        playerView = player.GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        foreach (Transform checkpoint in checkpointColliders)
        {
            if (checkpoint != null && checkpoint.gameObject.activeInHierarchy == true)
            {
                bool colliding = Vector3.Distance(checkpoint.position, player.transform.position) < checkpointRadius;
                Vector3 playerForward = playerView.transform.forward;
                playerForward.y = 0;
                Vector3 playerToCheckpt = checkpoint.position - player.transform.position;
                playerToCheckpt.y = 0;
                float angle = Vector3.Angle(playerForward, playerToCheckpt);
                Debug.Log($"Angle to {checkpoint.name} = {angle}");
                if (colliding && angle < angleThreshold)
                {
                    checkpointsReached += 1;
                    Debug.Log($"Checkpoint reached! {checkpointsReached}/{totalCheckpoints}");
                    checkpoint.gameObject.SetActive(false);
                    //Note for future: Add sound effect to indicate that a checkpoint has been completed
                    break;
                }
            }
        }

        if (checkpointsReached == totalCheckpoints)
        {
            checkpointsText.SetText($"Checkpoints: {totalCheckpoints}/{totalCheckpoints}\n\nOpening Door...");
            OpenDoor();
        }
        else
        {
            checkpointsText.SetText($"Checkpoints: {checkpointsReached}/{totalCheckpoints}\n\nGo to all the green cylinder checkpoints to open the door!");
        }
    }

    void OpenDoor()
    {
        door.position = Vector3.MoveTowards(door.position, new Vector3(18f, 2f, -3.7f), doorSpeed * Time.deltaTime);
    }
}
