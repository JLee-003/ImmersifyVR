using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class VRSceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    public Image fadeImage; // Reference to the Image component for the fade effect

    public void LoadScene()
    {
        if (SceneManager.GetSceneByName(sceneToLoad).IsValid())
        {
            StartCoroutine(FadeToBlack(sceneToLoad));
        }
        else
        {
            Debug.LogError("Scene " + sceneToLoad + " not found.");
        }
    }

    IEnumerator FadeToBlack(string sceneName)
    {
        float fadeSpeed = 1f; // Adjust the fade speed as needed
        fadeImage.gameObject.SetActive(true);

        while (fadeImage.color.a < 1f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a + (fadeSpeed * Time.deltaTime));
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

        // Optionally, fade back in after loading the new scene
        while (fadeImage.color.a > 0f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImage.color.a - (fadeSpeed * Time.deltaTime));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}