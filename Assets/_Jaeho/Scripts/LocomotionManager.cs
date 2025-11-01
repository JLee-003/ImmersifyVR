using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
    public static LocomotionManager Instance { get; private set; }

    ActionBasedContinuousMoveProvider continuousMoveProvider;
    LineSwimmer swimmingEvaluator;
    CharacterController characterController;

    //speeds
    float waterWalkSpeed = 1.5f;
    float normalWalkSpeed = 3f;

    [Header("Underwater")]
    [SerializeField] float sinkSpeed = 0.1f;   // m/s downward while in water

    [Header("Audio/FX")]
    [SerializeField] AudioClip waterEnterAudio;
    private BackgroundMusic _bgm;

    bool inWater = false;

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
        }
    }

    void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        swimmingEvaluator = GetComponent<LineSwimmer>();
        characterController = GetComponent<CharacterController>();
        _bgm = FindObjectOfType<BackgroundMusic>();

        ResetLandDefaults();
    }

    void Update()
    {
        // When in water, we do the vertical ourselves
        if (inWater && characterController)
        {
            Vector3 step = Vector3.down * sinkSpeed * Time.deltaTime;
            characterController.Move(step);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            inWater = true;

            continuousMoveProvider.moveSpeed = waterWalkSpeed;
            continuousMoveProvider.useGravity = false;   // turn OFF built-in gravity
            swimmingEvaluator.enabled = true;

            if (waterEnterAudio) AudioSource.PlayClipAtPoint(waterEnterAudio, transform.position, 1f);
            _bgm?.SetUnderwater(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water Volume"))
        {
            ResetLandDefaults();
        }
    }

    public void ResetLandDefaults()
    {
        inWater = false;

        continuousMoveProvider.moveSpeed = normalWalkSpeed;
        continuousMoveProvider.useGravity = true;    // restore built-in gravity
        swimmingEvaluator.enabled = false;

        _bgm?.SetUnderwater(false);
    }
}
