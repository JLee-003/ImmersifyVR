using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public UnityEvent OnLoadBegin = new UnityEvent();
    public UnityEvent OnLoadEnd = new UnityEvent();
    [SerializeField] public Animator animator;
    public ScreenFader screenFader = null;

    private bool isLoading = false;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        SceneManager.sceneLoaded += SetActiveScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetActiveScene;
    }

    public void LoadNewScene(string sceneName)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadScene(sceneName));
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        isLoading = true;

        OnLoadBegin?.Invoke();

        yield return StartCoroutine(UnloadCurrent());

        // Optional delay for testing
        // yield return new WaitForSeconds(3.0f);

        // Start a different animation for loading the scene
        if (animator != null)
        {
            animator.GetComponent<Animator>().Play("Fade_In");

        }

        yield return StartCoroutine(LoadNew(sceneName));
        if (screenFader != null)
        {
            yield return screenFader.StartFadeOut();
        }
        OnLoadEnd?.Invoke();

        isLoading = false;
    }


    private IEnumerator UnloadCurrent()
    {
        // Trigger the animation for unloading the scene
        if (animator != null)
        {
            animator.GetComponent<Animator>().SetTrigger("FadeOut");

        }
        yield return new WaitForSeconds(1);
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }
    }


    private IEnumerator LoadNew(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            
            yield return null;
        }
    }

    private void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }
}
