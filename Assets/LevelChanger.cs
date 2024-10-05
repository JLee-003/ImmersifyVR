using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] public Animator animator;
    private string levelToLoad;

    // Update is called once per frame
    void Update()
    {

    }
    public void FadeToLevel(string LevelName)
    {
        levelToLoad = LevelName;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
