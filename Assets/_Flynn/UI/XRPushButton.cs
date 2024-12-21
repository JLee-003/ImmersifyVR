using UnityEngine.SceneManagement;
using UnityEngine;

public class XRPushButton : MonoBehaviour
{
    public string targetSceneName;

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}