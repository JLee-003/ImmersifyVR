using UnityEngine;
using UnityEngine.InputSystem;
// using xr?

[RequireComponent(typeof(Rigidbody))]
public class Swimmer : MonoBehaviour
{
    [Header("Values")] // do i need ;?
    [SerializeField] float swimForce = 2f;
    [SerializeField] float dragForce = 1f;
    [SerializeField] float minForce;
    [SerializeField] float minTimeBetweenStrokes;
    
    [Header("References")] // do i need ;?

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

    void fixedUpdate() {
        _cooldownTimer += Time.fixedDeltaTime;
        if (_cooldownTimer > minTimeBetweenStrokes
            && leftControllerSwimReference.action.IsPressed()
            && rightControllerSwimReference.action.IsPressed()) // may need to change this to be more accessible for people with one hand
        { 

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
