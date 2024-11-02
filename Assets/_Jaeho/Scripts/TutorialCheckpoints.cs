using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialCheckpoints : MonoBehaviour
{
    [SerializeField] Transform[] checkpointColliders;
    [SerializeField] Transform door;
    GameObject player;

    float checkpointRadius = 2.2f;

    int totalCheckpoints;
    int checkpointsReached = 0;

    float doorSpeed = 0.75f;

    [SerializeField] TextMeshProUGUI checkpointsText;

    private void Start()
    {
        totalCheckpoints = checkpointColliders.Length;

        //Get player transform
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        foreach (Transform checkpoint in checkpointColliders)
        {
            if (checkpoint != null && checkpoint.gameObject.activeInHierarchy == true && Vector3.Distance(checkpoint.position, player.transform.position) < checkpointRadius)
            {
                checkpointsReached += 1;
                Debug.Log($"Checkpoint reached! {checkpointsReached}/{totalCheckpoints}");
                checkpoint.gameObject.SetActive(false);
                //Note for future: Add sound effect to indicate that a checkpoint has been completed
                break;
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
