using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
public class SocketInteractorScript : MonoBehaviour
{
    public string targetSceneName;

    public void OnSelectEnter(XRBaseInteractor interactor)
    {
    
            SceneManager.LoadScene(targetSceneName);

        
    }
}