using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class BackgroundMusic : MonoBehaviour
{
    [Serializable]
    public class SceneMusic
    {
        [Tooltip("Exact scene name (as in Build Settings)")]
        public string sceneName;
        public AudioClip clip;
    }

    [SerializeField] private List<SceneMusic> sceneMusic = new List<SceneMusic>();

    private readonly Dictionary<string, AudioClip> _map = new Dictionary<string, AudioClip>();
    private AudioSource _src;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _src = GetComponent<AudioSource>();
        if (_src == null) _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = true;
        _src.spatialBlend = 0f; //2D

        BuildMap();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void Start()
    {
        ApplyForScene(SceneManager.GetActiveScene().name);
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        ApplyForScene(newScene.name);
    }

    private void BuildMap()
    {
        _map.Clear();
        foreach (var sm in sceneMusic)
        {
            if (sm == null || string.IsNullOrEmpty(sm.sceneName) || sm.clip == null) continue;
            _map[sm.sceneName] = sm.clip; // last wins if duplicate names
        }
    }

    private void ApplyForScene(string sceneName)
    {
        if (_map.TryGetValue(sceneName, out var nextClip) && nextClip != null)
        {
            // If it's the same clip, don't restart—just make sure it's playing.
            if (_src.clip == nextClip)
            {
                if (!_src.isPlaying) _src.Play();
                return;
            }

            _src.clip = nextClip;
            _src.Play();
        }
        else
        {
            // No mapping for this scene—stop music.
            _src.Stop();
            _src.clip = null;
        }
    }
}
