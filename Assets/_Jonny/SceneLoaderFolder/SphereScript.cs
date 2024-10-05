using System.Collections;
using UnityEngine;

public class SphereScript : MonoBehaviour
{
    [SerializeField] private ScreenFader _screenFader;

    // Start is called before the first frame update
    public void LoadSceneOnGrab()
    {
        StartCoroutine(FadeAndLoadScene("Main"));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (_screenFader != null)
        {
            // Start fading out
            yield return _screenFader.StartFadeIn();

            // Load the new scene
            SceneLoader.Instance.LoadNewScene(sceneName);

            // Wait for the scene to load
            yield return null;

            // Start fading in
            _screenFader.StartFadeOut();
        }
        else
        {
            Debug.LogError("ScreenFader is not assigned.");
        }
    }

    public void ColorChangeOnHover()
    {
        GetComponent<Renderer>().material.color = new Color(0, 204, 102);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
