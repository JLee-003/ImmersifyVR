using UnityEngine;

public class TutorialHandZone : MonoBehaviour
{
    public enum HandSide
    {
        Left,
        Right
    }

    [SerializeField] private HandSide handSide;

    public bool IsCorrectHandInside { get; private set; }

    private Transform targetHand;

    private void Start()
    {
        if (PlayerReferences.instance == null)
        {
            Debug.LogError("PlayerReferences.instance not found.");
            return;
        }

        if (handSide == HandSide.Left)
        {
            targetHand = PlayerReferences.instance.leftController.transform;
        }
        else
        {
            targetHand = PlayerReferences.instance.rightController.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetHand == null) return;

        if (other.transform == targetHand || other.transform.IsChildOf(targetHand))
        {
            IsCorrectHandInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetHand == null) return;

        if (other.transform == targetHand || other.transform.IsChildOf(targetHand))
        {
            IsCorrectHandInside = false;
        }
    }
}