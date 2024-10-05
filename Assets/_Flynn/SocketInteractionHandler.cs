using UnityEngine;
using UnityEngine.SceneManagement;

public class SocketInteractionHandler : MonoBehaviour
{
    public string targetSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractableObject"))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}