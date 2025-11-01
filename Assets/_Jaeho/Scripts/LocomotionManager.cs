using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionManager : MonoBehaviour
{
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

    void Start()
    {
        continuousMoveProvider = GetComponentInChildren<ActionBasedContinuousMoveProvider>();
        swimmingEvaluator = GetComponent<LineSwimmer>();
        characterController = GetComponent<CharacterController>();

        continuousMoveProvider.moveSpeed = normalWalkSpeed;
        swimmingEvaluator.enabled = false;

        // Land defaults
        Physics.gravity = new Vector3(0f, -9.8f, 0f);
        continuousMoveProvider.useGravity = true;

        _bgm = FindObjectOfType<BackgroundMusic>();
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
            inWater = false;

            continuousMoveProvider.moveSpeed = normalWalkSpeed;
            continuousMoveProvider.useGravity = true;    // restore built-in gravity
            swimmingEvaluator.enabled = false;

            Physics.gravity = new Vector3(0f, -9.8f, 0f);
            _bgm?.SetUnderwater(false);
        }
    }
}
