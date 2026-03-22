using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickaxeClimb : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    Rigidbody rb;

    Transform controllerTarget;

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

    ForceGrab forceGrab;

    bool engaged;
    Transform weakPoint;
    RigidbodyConstraints originalConstraints;
    float originalStepOffset;

    Vector3 currentControllerWorld;
    Vector3 lastControllerWorld;
    Vector3 engagedLocalController;
    bool hasSample;

    public static PickaxeClimb engagedPickaxe = null;
    public static PickaxeClimb leftPickaxe = null;
    public static PickaxeClimb rightPickaxe = null;
    public static bool rightEngaged = false;
    public static bool leftEngaged = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalConstraints = rb.constraints;

        forceGrab = GetComponent<ForceGrab>();

        player = PlayerReferences.instance.playerObject.transform;

        if (forceGrab.isRight)
        {
            controllerTarget = PlayerReferences.instance.rightController.transform;
            rightPickaxe = this;
        }
        else
        {
            controllerTarget = PlayerReferences.instance.leftController.transform;
            leftPickaxe = this;
        }

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


        if (engagedPickaxe != this) return;

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
    static bool AnyPickaxeEngaged()
    {
        return leftEngaged || rightEngaged;
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

        if (forceGrab.isRight)
        {
            rightEngaged = true;
        }

        else
        {
            leftEngaged = true;
        }

        engagedPickaxe = this;

        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePosition;

        if (moveProvider)
        {
            moveProvider.useGravity = false;
        }

        if (characterController)
        {
            characterController.stepOffset = 0f;
        }

        currentControllerWorld = controllerTarget.position;
        lastControllerWorld = currentControllerWorld;
        engagedLocalController = weakPoint.InverseTransformPoint(currentControllerWorld);
        hasSample = true;

        HapticFeedbackManager.Instance?.InitiateHapticFeedback(!forceGrab.isRight, forceGrab.isRight, 1.0f, 0.5f);
    }

    public void Disengage()
    {
        engaged = false;

        if (forceGrab.isRight)
        {
            rightEngaged = false;
        }
        else
        {
            leftEngaged = false;
        }

        // hand off movement control if the other pickaxe is still engaged
        if (engagedPickaxe == this)
        {
            if (forceGrab.isRight)
            {
                engagedPickaxe = (leftPickaxe != null && leftPickaxe.engaged) ? leftPickaxe : null;
            }
            else
            {
                engagedPickaxe = (rightPickaxe != null && rightPickaxe.engaged) ? rightPickaxe : null;
            }
        }

        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;

        rb.constraints = originalConstraints;

        if (!AnyPickaxeEngaged())
        {
            if (moveProvider)
            {
                moveProvider.useGravity = true;
            }

            if (characterController)
            {
                characterController.stepOffset = originalStepOffset;
            }
        }

        weakPoint = null;
        hasSample = false;

        HapticFeedbackManager.Instance?.InitiateHapticFeedback(!forceGrab.isRight, forceGrab.isRight, 0.7f, 0.1f);
    }
}