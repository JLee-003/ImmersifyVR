using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneHandModelPair
{
    public string sceneName;
    public GameObject handModelPrefab;
}

public class HandModelLoading : MonoBehaviour
{
    [Header("Hand Model Configuration")]
    [Tooltip("Array of scene-to-hand-model mappings")]
    [SerializeField] private SceneHandModelPair[] sceneHandModels;
    
    [Tooltip("Default hand model if no scene mapping is found")]
    [SerializeField] private GameObject defaultHandModel;
    
    [Header("Controller Settings")]
    [Tooltip("Is this the left hand controller?")]
    [SerializeField] private bool isLeftHand = true;
    
    private GameObject currentHandModelInstance;
    private Dictionary<string, GameObject> sceneModelDictionary;

    void Start()
    {
        // Build dictionary for faster lookups
        BuildSceneDictionary();
        
        // Load hand model for current scene
        LoadHandModelForCurrentScene();
        
        // Subscribe to scene loading events if SceneLoader exists
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnLoadEnd.AddListener(LoadHandModelForCurrentScene);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnLoadEnd.RemoveListener(LoadHandModelForCurrentScene);
        }
    }

    /// <summary>
    /// Builds a dictionary from the array for faster scene lookups
    /// </summary>
    private void BuildSceneDictionary()
    {
        sceneModelDictionary = new Dictionary<string, GameObject>();
        
        if (sceneHandModels != null)
        {
            foreach (var pair in sceneHandModels)
            {
                if (!string.IsNullOrEmpty(pair.sceneName) && pair.handModelPrefab != null)
                {
                    sceneModelDictionary[pair.sceneName] = pair.handModelPrefab;
                }
            }
        }
    }

    /// <summary>
    /// Loads the appropriate hand model based on the current active scene
    /// </summary>
    private void LoadHandModelForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Try to find hand model for this scene
        if (sceneModelDictionary.ContainsKey(currentSceneName))
        {
            SwitchHandModel(sceneModelDictionary[currentSceneName]);
        }
        else if (defaultHandModel != null)
        {
            // Fall back to default hand model
            SwitchHandModel(defaultHandModel);
        }
        else
        {
            Debug.LogWarning($"No hand model found for scene '{currentSceneName}' and no default model assigned.");
        }
    }

    /// <summary>
    /// Switches to a new hand model. Can be called manually from other scripts.
    /// </summary>
    /// <param name="newHandModelPrefab">The hand model prefab to instantiate</param>
    public void SwitchHandModel(GameObject newHandModelPrefab)
    {
        if (newHandModelPrefab == null)
        {
            Debug.LogError("Cannot switch to null hand model prefab.");
            return;
        }

        // Destroy current hand model if it exists
        DestroyCurrentHandModel();

        // Instantiate new hand model as child of this controller
        currentHandModelInstance = Instantiate(newHandModelPrefab, transform);
        
        // Reset local transform to match controller
        currentHandModelInstance.transform.localPosition = Vector3.zero;
        currentHandModelInstance.transform.localRotation = Quaternion.identity;

        Debug.Log($"Switched to hand model: {newHandModelPrefab.name} on {(isLeftHand ? "Left" : "Right")} controller");
    }

    /// <summary>
    /// Switches hand model by scene name
    /// </summary>
    /// <param name="sceneName">Name of the scene</param>
    public void SwitchHandModelByScene(string sceneName)
    {
        if (sceneModelDictionary.ContainsKey(sceneName))
        {
            SwitchHandModel(sceneModelDictionary[sceneName]);
        }
        else
        {
            Debug.LogWarning($"No hand model mapping found for scene: {sceneName}");
        }
    }

    /// <summary>
    /// Destroys the current hand model instance
    /// </summary>
    private void DestroyCurrentHandModel()
    {
        if (currentHandModelInstance != null)
        {
            Destroy(currentHandModelInstance);
            currentHandModelInstance = null;
        }
    }

    /// <summary>
    /// Manually reload the hand model for the current scene
    /// </summary>
    public void ReloadHandModel()
    {
        LoadHandModelForCurrentScene();
    }
}
