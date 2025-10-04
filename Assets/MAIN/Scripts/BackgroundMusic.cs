using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class BackgroundMusic : MonoBehaviour
{
    [Serializable]
    public class SceneMusic
    {
        [Tooltip("Exact scene name (as in Build Settings)")]
        public string sceneName;
        public AudioClip clip;

        [Tooltip("Volume for this scene (0..1).")]
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [Header("Master Volume")]
    [Tooltip("Overall music volume (0..1). Multiplied by per-scene volume.")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    [SerializeField] private List<SceneMusic> sceneMusic = new List<SceneMusic>();

    // --- Underwater (Low-Pass) ---
    [Header("Underwater Low-Pass")]
    [Tooltip("Start with the underwater effect enabled.")]
    [SerializeField] private bool startUnderwater = false;

    [Tooltip("Cutoff above water (Hz)")]
    [SerializeField] private float cutoffAbove = 22000f;

    [Tooltip("Cutoff underwater (Hz)")]
    [SerializeField] private float cutoffUnder = 800f; // ~500–1000 feels 'underwater'

    [Tooltip("Resonance Q above water (0.1–10)")]
    [SerializeField] private float resonanceAbove = 1.0f;

    [Tooltip("Resonance Q underwater (0.1–10)")]
    [SerializeField] private float resonanceUnder = 1.2f;

    [Tooltip("Seconds to transition between states")]
    [SerializeField] private float transitionTime = 0.25f;

    // Store both clip and per-scene volume
    private struct SceneEntry
    {
        public AudioClip clip;
        public float sceneVolume;
    }

    private readonly Dictionary<string, SceneEntry> _map = new Dictionary<string, SceneEntry>();
    private AudioSource _src;
    private AudioLowPassFilter _lpf;
    private bool _underwater;
    private Coroutine _lpfRoutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _src = GetComponent<AudioSource>();
        if (_src == null) _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake = false;
        _src.loop = true;
        _src.spatialBlend = 0f; // 2D

        _lpf = GetComponent<AudioLowPassFilter>();
        if (_lpf == null) _lpf = gameObject.AddComponent<AudioLowPassFilter>();

        BuildMap();

        // Initialize LPF to the starting state
        _underwater = startUnderwater;
        ApplyLPFInstant(_underwater);
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
            _map[sm.sceneName] = new SceneEntry { clip = sm.clip, sceneVolume = Mathf.Clamp01(sm.volume) };
        }
    }

    private void ApplyForScene(string sceneName)
    {
        if (_map.TryGetValue(sceneName, out var entry) && entry.clip != null)
        {
            bool sameClip = _src.clip == entry.clip;

            _src.clip = entry.clip;
            _src.volume = Mathf.Clamp01(volume) * Mathf.Clamp01(entry.sceneVolume);

            if (!sameClip || !_src.isPlaying) _src.Play();
        }
        else
        {
            _src.Stop();
            _src.clip = null;
        }
    }

    private void OnValidate()
    {
        // Keep dictionary and current AudioSource volume in sync when editing
        if (sceneMusic != null) BuildMap();

        if (_src == null) _src = GetComponent<AudioSource>();
        if (_src != null && _src.clip != null)
        {
            var active = SceneManager.GetActiveScene().name;
            if (_map.TryGetValue(active, out var entry))
                _src.volume = Mathf.Clamp01(volume) * Mathf.Clamp01(entry.sceneVolume);
            else
                _src.volume = Mathf.Clamp01(volume);
        }

        if (_lpf == null) _lpf = GetComponent<AudioLowPassFilter>();
        if (_lpf != null)
        {
            // Reflect the current underwater flag in edit mode
            ApplyLPFInstant(_underwater);
        }
    }

    // --- Public API ---

    /// <summary>Master music volume (0..1).</summary>
    public void SetMasterVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        var active = SceneManager.GetActiveScene().name;
        if (_src != null)
        {
            if (_map.TryGetValue(active, out var entry))
                _src.volume = volume * Mathf.Clamp01(entry.sceneVolume);
            else
                _src.volume = volume;
        }
    }

    /// <summary>Set per-scene volume (0..1) at runtime.</summary>
    public void SetSceneVolume(string sceneName, float v)
    {
        v = Mathf.Clamp01(v);
        if (_map.TryGetValue(sceneName, out var entry))
        {
            entry.sceneVolume = v;
            _map[sceneName] = entry;

            if (SceneManager.GetActiveScene().name == sceneName && _src != null)
                _src.volume = Mathf.Clamp01(volume) * v;
        }

        // Update serialized list so Inspector reflects changes (optional)
        for (int i = 0; i < sceneMusic.Count; i++)
        {
            if (sceneMusic[i] != null && sceneMusic[i].sceneName == sceneName)
            {
                sceneMusic[i].volume = v;
                break;
            }
        }
    }

    /// <summary>Toggle the underwater muffle effect.</summary>
    public void SetUnderwater(bool underwater)
    {
        if (_underwater == underwater) return;
        _underwater = underwater;

        if (_lpfRoutine != null) StopCoroutine(_lpfRoutine);
        _lpfRoutine = StartCoroutine(AnimateLPF(
            targetCutoff: _underwater ? cutoffUnder : cutoffAbove,
            targetQ: _underwater ? resonanceUnder : resonanceAbove,
            time: Mathf.Max(0f, transitionTime)
        ));
    }

    /// <summary>Expose as a property if you want to flip it from Inspector via script.</summary>
    public bool Underwater
    {
        get => _underwater;
        set => SetUnderwater(value);
    }

    // --- LPF helpers ---

    private void ApplyLPFInstant(bool underwater)
    {
        if (_lpf == null) return;
        _lpf.cutoffFrequency = underwater ? cutoffUnder : cutoffAbove;
        _lpf.lowpassResonanceQ = Mathf.Clamp(underwater ? resonanceUnder : resonanceAbove, 0.1f, 10f);
    }

    private System.Collections.IEnumerator AnimateLPF(float targetCutoff, float targetQ, float time)
    {
        if (_lpf == null || time <= 0f)
        {
            if (_lpf != null)
            {
                _lpf.cutoffFrequency = targetCutoff;
                _lpf.lowpassResonanceQ = Mathf.Clamp(targetQ, 0.1f, 10f);
            }
            yield break;
        }

        float startCutoff = _lpf.cutoffFrequency;
        float startQ = _lpf.lowpassResonanceQ;
        float t = 0f;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / time);
            _lpf.cutoffFrequency = Mathf.Lerp(startCutoff, targetCutoff, k);
            _lpf.lowpassResonanceQ = Mathf.Lerp(startQ, Mathf.Clamp(targetQ, 0.1f, 10f), k);
            yield return null;
        }

        _lpf.cutoffFrequency = targetCutoff;
        _lpf.lowpassResonanceQ = Mathf.Clamp(targetQ, 0.1f, 10f);
    }
}
