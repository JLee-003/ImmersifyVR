using UnityEngine;

public class TutorialSphereFollow : MonoBehaviour
{
    [SerializeField] bool followLeftController = true;
    [SerializeField] Vector3 offset = Vector3.zero;
    
    private GameObject targetController;
    
    void Start()
    {
        // Find the target controller based on the boolean choice
        if (followLeftController)
        {
            GameObject[] leftHands = GameObject.FindGameObjectsWithTag("LeftHand");
            if (leftHands.Length > 0)
            {
                targetController = leftHands[0];
            }
            else
            {
                Debug.LogError("No GameObject with 'LeftHand' tag found for TutorialSphereFollow.");
            }
        }
        else
        {
            GameObject[] rightHands = GameObject.FindGameObjectsWithTag("RightHand");
            if (rightHands.Length > 0)
            {
                targetController = rightHands[0];
            }
            else
            {
                Debug.LogError("No GameObject with 'RightHand' tag found for TutorialSphereFollow.");
            }
        }
        
        gameObject.SetActive(true);
    }
    
    void Update()
    {
        // Follow the target controller if it exists and this sphere is active
        if (targetController != null && gameObject.activeInHierarchy)
        {
            transform.position = targetController.transform.position + offset;
        }
    }
}
