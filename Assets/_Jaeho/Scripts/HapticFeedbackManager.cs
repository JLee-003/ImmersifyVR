using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager Instance { get; private set; }
    XRBaseController[] controllers;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
    }
    private void Start()
    {
        controllers = GetComponentsInChildren<XRBaseController>();
    }

    /// <summary>
    /// Initiates haptic feedback on the given controllers.
    /// </summary>
    /// <param name="initiateLeft">If the haptic feedback should be initiated on the left controller.</param>
    /// /// <param name="initiateRight">If the haptic feedback should be initiated on the right controller.</param>
    /// <param name="intensity">The intensity of the haptic feedback (range of 0.0 to 1.0).</param>
    /// <param name="duration">The duration of the haptic feedback in seconds.</param>
    public void InitiateHapticFeedback(bool initiateLeft, bool initiateRight, float intensity, float duration)
    {
        if (initiateLeft)
        {
            controllers[0].SendHapticImpulse(intensity, duration);
        }
        if (initiateRight)
        {
            controllers[1].SendHapticImpulse(intensity, duration);
        }
    }
}