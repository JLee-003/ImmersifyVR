using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public UnityEvent OnLoadBegin = new UnityEvent();
    public UnityEvent OnLoadEnd = new UnityEvent();

    private bool isLoading = false;

    [SerializeField] string startingScene;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            LoadNewScene(startingScene, false);
        }

        SceneManager.sceneLoaded += SetActiveScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetActiveScene;
    }

    public async void LoadNewScene(string sceneName, bool unload = true)
    {
        if (!isLoading)
        {
            await Fader.Instance.FadeIn(reset: false);
            StartCoroutine(LoadScene(sceneName, unload));
        }
    }

    private IEnumerator LoadScene(string sceneName, bool unload)
    {
        isLoading = true;

        OnLoadBegin?.Invoke();

        if (unload)
        {
            yield return StartCoroutine(UnloadCurrent());
        }
        // Optional delay for testing
        // yield return new WaitForSeconds(3.0f);

        yield return StartCoroutine(LoadNew(sceneName));

        Task t = Fader.Instance.FadeOut();
        yield return new WaitUntil(() => t.IsCompleted);

        OnLoadEnd?.Invoke();

        isLoading = false;
    }


    private IEnumerator UnloadCurrent()
    {
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
