using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickaxeClimb : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    Rigidbody rb;

    [SerializeField] Transform controllerTarget;

    Transform player;
    CharacterController characterController;
    ActionBasedContinuousMoveProvider moveProvider;

    [Header("Climb")]
    [SerializeField] float climbStrength = 1f;
    [SerializeField] float maxMovePerFixed = 0.25f;

    [Header("Disengage")]
    [SerializeField] float disengageDistance = 0.35f;

    [Header("Stability")]
    [SerializeField] float deadZone = 0.002f;

    bool engaged;
    Transform weakPoint;
    RigidbodyConstraints originalConstraints;
    float originalStepOffset;
    bool prevUseGravity;

    Vector3 currentControllerWorld;
    Vector3 lastControllerWorld;
    Vector3 engagedLocalController;
    bool hasSample;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalConstraints = rb.constraints;

        player = PlayerReferences.instance.playerObject.transform;
        characterController = player.GetComponent<CharacterController>();
        moveProvider = player.GetComponentInChildren<ActionBasedContinuousMoveProvider>();

        if (characterController)
            originalStepOffset = characterController.stepOffset;
    }

    void Update()
    {
        if (!engaged || !weakPoint) return;

        currentControllerWorld = controllerTarget.position;
        hasSample = true;
    }

    void FixedUpdate()
    {
        if (!engaged || !weakPoint || !hasSample) return;

        Vector3 localController = weakPoint.InverseTransformPoint(currentControllerWorld);

        float zPullAway = Mathf.Abs(localController.z - engagedLocalController.z);
        if (zPullAway > disengageDistance)
        {
            Disengage();
            return;
        }

        Vector3 controllerWorldDelta = currentControllerWorld - lastControllerWorld;

        Vector3 localDelta = weakPoint.InverseTransformVector(controllerWorldDelta);
        localDelta.z = 0f;

        if (localDelta.sqrMagnitude < deadZone * deadZone)
            return;

        Vector3 move = -weakPoint.TransformVector(localDelta) * climbStrength;

        if (move.magnitude > maxMovePerFixed)
            move = move.normalized * maxMovePerFixed;

        Vector3 playerPosBefore = player.position;
        characterController.Move(move);
        Vector3 actualRigMove = player.position - playerPosBefore;

        lastControllerWorld = currentControllerWorld + actualRigMove;
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
            prevUseGravity = moveProvider.useGravity;
            moveProvider.useGravity = false;
        }

        if (characterController)
            characterController.stepOffset = 0f;

        currentControllerWorld = controllerTarget.position;
        lastControllerWorld = currentControllerWorld;
        engagedLocalController = weakPoint.InverseTransformPoint(currentControllerWorld);
        hasSample = true;
    }

    public void Disengage()
    {
        engaged = false;

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        rb.constraints = originalConstraints;

        if (moveProvider)
            moveProvider.useGravity = prevUseGravity;

        if (characterController)
            characterController.stepOffset = originalStepOffset;

        weakPoint = null;
        hasSample = false;
    }
}