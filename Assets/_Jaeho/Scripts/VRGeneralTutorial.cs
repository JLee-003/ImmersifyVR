using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VRGeneralTutorial : MonoBehaviour
{
    [SerializeField] Transform[] checkpointColliders;
    [SerializeField] Transform door;
    GameObject player;
    Camera playerView;

    float checkpointRadius = 0.8f;

    int totalCheckpoints;
    int checkpointsReached = 0;

    float doorSpeed = 0.75f;

    float angleThreshold = 40f;

    [SerializeField] TextMeshProUGUI checkpointsText;
    [SerializeField] Button pokeButton;

    private void Awake()
    {
        Teleport.Instance.tp(-0.5f, 0.2f, 0f);
    }
    private void Start()
    {
        totalCheckpoints = checkpointColliders.Length;

        //Get player transform
        player = GameObject.FindGameObjectWithTag("Player");
        playerView = player.GetComponentInChildren<Camera>();

        pokeButton.onClick.AddListener(TransitionOut);
    }

    private void Update()
    {
        //Checkpoints logic
        foreach (Transform checkpoint in checkpointColliders)
        {
            if (checkpoint != null && checkpoint.gameObject.activeInHierarchy == true)
            {
                //Colliding Check
                Vector3 checkpointPosXZ = checkpoint.position;
                checkpointPosXZ.y = 0;
                Vector3 playerPosXZ = player.transform.position;
                playerPosXZ.y = 0;

                float distance = Vector3.Distance(checkpointPosXZ, playerPosXZ);
                bool colliding = distance < checkpointRadius;

                //Angle Check
                Transform corner = checkpoint.GetChild(0);
                Vector3 playerForward = playerView.transform.forward;
                playerForward.y = 0;
                playerForward.Normalize();
                
                Vector3 playerToCheckpt = corner.position - playerView.transform.position;
                playerToCheckpt.y = 0;
                playerToCheckpt.Normalize();

                float angle = Vector3.Angle(playerForward, playerToCheckpt);
                //Debug.Log($"Angle to {checkpoint.name} = {angle}, cornerPos:{corner.position}, forward: {playerForward}, playerToCheckpt:{playerToCheckpt}");
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

    void TransitionOut()
    {
        Debug.Log("Transitioning out...");
        SceneLoader.Instance.LoadNewScene("Lobby");
    }
}
