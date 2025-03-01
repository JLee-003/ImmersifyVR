using UnityEngine.SceneManagement;
using UnityEngine;

public class XRPushButton : MonoBehaviour
{
    public string targetSceneName;

    public void LoadTargetScene()
    {
        SceneLoader.Instance.LoadNewScene(targetSceneName);
    }
}