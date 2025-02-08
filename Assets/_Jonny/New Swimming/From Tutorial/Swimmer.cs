using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Swimmer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float swimForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;
    
    [Header("References")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] Transform trackingReference;
    [SerializeField] Transform leftControllerTransform; // Reference to the left controller's transform
    [SerializeField] Transform rightControllerTransform; // Reference to the right controller's transform

    CharacterController _characterController;
    float _cooldownTimer;
    Vector3 _velocity;
    Vector3 _previousLeftControllerPosition;
    Vector3 _previousRightControllerPosition;

    void Awake() {
        _characterController = GetComponent<CharacterController>();

        // Initialize previous positions for velocity calculation
        _previousLeftControllerPosition = leftControllerTransform.localPosition;
        _previousRightControllerPosition = rightControllerTransform.localPosition;
    }

    void FixedUpdate() {
        _cooldownTimer += Time.fixedDeltaTime;

        // Calculate the velocity of the controllers
        Vector3 leftHandVelocity = (leftControllerTransform.localPosition - _previousLeftControllerPosition) / Time.fixedDeltaTime;
        Vector3 rightHandVelocity = (rightControllerTransform.localPosition - _previousRightControllerPosition) / Time.fixedDeltaTime;

        // Update previous positions for the next frame
        _previousLeftControllerPosition = leftControllerTransform.localPosition;
        _previousRightControllerPosition = rightControllerTransform.localPosition;

        if (_cooldownTimer > minTimeBetweenStrokes
            && (leftControllerSwimReference.action.IsPressed() || rightControllerSwimReference.action.IsPressed())) // this or was an and; changed to be more accessible
        { 
            Vector3 localVelocity = leftHandVelocity + rightHandVelocity;
            Debug.Log(localVelocity);
            localVelocity *= -1;

            if (localVelocity.sqrMagnitude > minForce * minForce)
            {
                // Perform force
                Vector3 worldVelocity = trackingReference.TransformDirection(localVelocity);
                _velocity += worldVelocity * swimForce * Time.fixedDeltaTime;
                _cooldownTimer = 0f;
            }
        }

        // Apply drag force
        if (_velocity.sqrMagnitude > 0.01f) {
            _velocity -= _velocity * dragForce * Time.fixedDeltaTime;
        } else {
            _velocity = Vector3.zero;
        }

        // Move the character controller
        _characterController.Move(_velocity * Time.fixedDeltaTime);
    }
}

/*using UnityEngine;
using UnityEngine.InputSystem;
// using xr?

[RequireComponent(typeof(Rigidbody))]
public class Swimmer : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float swimForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;
    
    [Header("References")]
    [SerializeField] InputActionReference leftControllerSwimReference;
    [SerializeField] InputActionReference leftControllerVelocity;
    [SerializeField] InputActionReference rightControllerSwimReference;
    [SerializeField] InputActionReference rightControllerVelocity;
    [SerializeField] Transform trackingReference;

    Rigidbody _rigidbody;
    float _cooldownTimer;

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate() {
        _cooldownTimer += Time.fixedDeltaTime;

        if (_cooldownTimer > minTimeBetweenStrokes
            && (leftControllerSwimReference.action.IsPressed() || rightControllerSwimReference.action.IsPressed())) // this or was an and; changed to be more accessible
        { 

            Debug.Log("SWIMMING!!!");
            var leftHandVelocity = leftControllerVelocity.action.ReadValue<Vector3>();
            var rightHandVelocity = rightControllerVelocity.action.ReadValue<Vector3>();
            Vector3 localVelocity = leftHandVelocity + rightHandVelocity;
            localVelocity *= -1;

            if (localVelocity.sqrMagnitude > minForce * minForce)
            {
                // perform force
                Vector3 worldVelocity = trackingReference.TransformDirection(localVelocity);
                _rigidbody.AddForce(worldVelocity * swimForce, ForceMode.Acceleration);
                _cooldownTimer = 0f;
            }
        }

        if (_rigidbody.velocity.sqrMagnitude > 0.01f) {
            _rigidbody.AddForce(-_rigidbody.velocity * dragForce, ForceMode.Acceleration);
        }
    }
}
*/