using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickaxeClimb : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    Rigidbody rb;
    [SerializeField] Transform controllerTarget;

    [Header("Player")]
    CharacterController characterController;
    ActionBasedContinuousMoveProvider moveProvider;

    [Header("Climb Settings")]
    [SerializeField] float climbStrength = 1f;
    [SerializeField] float maxMovePerFixed = 0.25f;
    [SerializeField] float disengageDistance = 0.35f;
    [SerializeField] float deadZone = 0.0005f;

    bool engaged;

    Transform weakPoint;
    RigidbodyConstraints originalConstraints;

    // NEW: track controller local position over time
    Vector3 lastLocalController;
    Vector3 engagedLocalController; // baseline for disengage axis

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalConstraints = rb.constraints;

        characterController = PlayerReferences.instance.playerObject.GetComponent<CharacterController>();
        moveProvider = PlayerReferences.instance.playerObject.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
    }

    void FixedUpdate()
    {
        if (!engaged) return;
        if (!weakPoint) return;

        // Controller position in weakpoint local space
        Vector3 localController = weakPoint.InverseTransformPoint(controllerTarget.position);

        // Disengage check: ONLY along local Z, relative to engage baseline
        float zPullAway = Mathf.Abs(localController.z - engagedLocalController.z);
        if (zPullAway > disengageDistance)
        {
            Disengage();
            return;
        }

        // NEW: Move based on controller DELTA since last frame (XY only)
        Vector3 deltaLocal = localController - lastLocalController;

        // Ignore Z for climbing (reserved for disengage)
        deltaLocal.z = 0f;

        if (deltaLocal.sqrMagnitude < deadZone * deadZone)
        {
            lastLocalController = localController; // still update to avoid accumulating tiny drift
            return;
        }

        // Convert local delta to world direction. IMPORTANT: TransformVector (not TransformPoint).
        Vector3 worldDelta = weakPoint.TransformVector(deltaLocal);

        // Climb: player moves opposite hand motion
        Vector3 move = -worldDelta * climbStrength;

        // Clamp per physics step
        if (move.magnitude > maxMovePerFixed)
            move = move.normalized * maxMovePerFixed;

        characterController.Move(move);

        // Update for next step
        lastLocalController = localController;
    }

    public void TryEngage(Transform wp)
    {
        if (engaged) return;
        weakPoint = wp;
        Engage();
    }

    void Engage()
    {
        Debug.Log("Pickaxe Engaged");
        engaged = true;

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;

        if (moveProvider)
            moveProvider.useGravity = false;

        // NEW: initialize controller tracking in weakpoint space
        Vector3 localController = weakPoint.InverseTransformPoint(controllerTarget.position);
        lastLocalController = localController;
        engagedLocalController = localController;
    }

    void Disengage()
    {
        Debug.Log("Pickaxe Disengaged");
        engaged = false;

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        rb.constraints = originalConstraints;

        if (moveProvider)
            moveProvider.useGravity = true;

        weakPoint = null;
    }
}