using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickaxeClimb : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    Rigidbody rb;

    [SerializeField] Transform controllerTarget;

    CharacterController characterController;
    ActionBasedContinuousMoveProvider moveProvider;

    [Header("Climb")]
    [SerializeField] float climbStrength = 1f;
    [SerializeField] float maxMovePerFixed = 0.25f;

    [Header("Disengage (local Z pull-away)")]
    [SerializeField] float disengageDistance = 0.35f;

    [Header("Stability")]
    [SerializeField] float deadZone = 0.006f;
    [SerializeField] bool pullOnly = true;
    [SerializeField] float pullThreshold = 0.0015f;

    [Header("Anti-jitter (CharacterController vs Wall)")]
    [SerializeField] string playerLayerName = "Player";
    [SerializeField] string climbWallLayerName = "ClimbWall";
    [SerializeField] bool ignoreWallCollisionWhileEngaged = true;
    [SerializeField] bool setStepOffsetZeroWhileEngaged = true;

    bool engaged;
    Transform weakPoint;
    RigidbodyConstraints originalConstraints;

    bool prevMoveProviderEnabled;
    bool prevUseGravity;

    float originalStepOffset;
    bool wallCollisionIgnored;

    Vector3 currentLocalController;
    Vector3 lastLocalController;
    Vector3 engagedLocalController;
    bool hasSample;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalConstraints = rb.constraints;

        characterController = PlayerReferences.instance.playerObject.GetComponent<CharacterController>();
        moveProvider = PlayerReferences.instance.playerObject.GetComponentInChildren<ActionBasedContinuousMoveProvider>();

        if (characterController) originalStepOffset = characterController.stepOffset;
    }

    void Update()
    {
        if (!engaged || !weakPoint) return;
        currentLocalController = weakPoint.InverseTransformPoint(controllerTarget.position);
        hasSample = true;
    }

    void FixedUpdate()
    {
        if (!engaged || !weakPoint || !hasSample) return;

        float zPullAway = Mathf.Abs(currentLocalController.z - engagedLocalController.z);
        if (zPullAway > disengageDistance)
        {
            Disengage();
            return;
        }

        Vector3 deltaLocal = currentLocalController - lastLocalController;
        lastLocalController = currentLocalController;

        deltaLocal.z = 0f;

        if (deltaLocal.sqrMagnitude < deadZone * deadZone)
            return;

        if (pullOnly)
        {
            if (Mathf.Abs(deltaLocal.x) < pullThreshold && Mathf.Abs(deltaLocal.y) < pullThreshold)
                return;
        }

        Vector3 worldDelta = weakPoint.TransformVector(deltaLocal);
        Vector3 move = -worldDelta * climbStrength;

        if (move.magnitude > maxMovePerFixed)
            move = move.normalized * maxMovePerFixed;

        characterController.Move(move);
    }

    public void TryEngage(Transform wp)
    {
        if (engaged) return;
        weakPoint = wp;
        Engage();
    }

    void Engage()
    {
        engaged = true;

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;

        if (moveProvider)
        {
            prevMoveProviderEnabled = moveProvider.enabled;
            prevUseGravity = moveProvider.useGravity;

            moveProvider.useGravity = false;
            moveProvider.enabled = false;
        }

        if (characterController && setStepOffsetZeroWhileEngaged)
            characterController.stepOffset = 0f;

        if (ignoreWallCollisionWhileEngaged)
            SetWallCollisionIgnore(true);

        currentLocalController = weakPoint.InverseTransformPoint(controllerTarget.position);
        lastLocalController = currentLocalController;
        engagedLocalController = currentLocalController;
        hasSample = true;
    }

    public void Disengage()
    {
        engaged = false;

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        rb.constraints = originalConstraints;

        if (moveProvider)
        {
            moveProvider.enabled = prevMoveProviderEnabled;
            moveProvider.useGravity = prevUseGravity;
        }

        if (characterController && setStepOffsetZeroWhileEngaged)
            characterController.stepOffset = originalStepOffset;

        if (ignoreWallCollisionWhileEngaged)
            SetWallCollisionIgnore(false);

        weakPoint = null;
        hasSample = false;
    }

    void SetWallCollisionIgnore(bool ignore)
    {
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int wallLayer = LayerMask.NameToLayer(climbWallLayerName);

        if (playerLayer < 0 || wallLayer < 0) return;

        Physics.IgnoreLayerCollision(playerLayer, wallLayer, ignore);
        wallCollisionIgnored = ignore;
    }
}