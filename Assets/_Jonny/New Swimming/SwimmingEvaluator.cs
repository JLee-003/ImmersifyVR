using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;


// USING DELTA VELOCITY OR VELOCITY???
public class SwimmingEvaluator : MonoBehaviour
{
    ActionBasedController controller;
    CharacterController characterController;
    [SerializeField] Transform head;

    bool measuring = false;

    Vector3 dv; // delta velocity
    Vector3 curr_v; // current velocity
    Vector3 prev_v; // preivous velocity

    Vector3 curr_pos;
    Vector3 prev_pos;

    Vector3 boost_vel;

    float decelerationFactor = 0.98f;


    // Start is called before the first frame update
    void Start()
    {
        characterController = head.GetComponent<CharacterController>();
        controller = GetComponent<ActionBasedController>();

    // Update is called once per frame
    void Update()
    {
        // Calculate delta velocity
        boost_vel = CalculateSpeedBoost();

        // Conditional: if controller is moving above X speed, apply boost using acceleration
        if (true) {
            ApplyBoost()
        }
    }

    private void FixedUpdate()
    {
        currentSpeed *= decelerationFactor;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, speedCap);
    }


    void CalculateSpeedBoost() {
        // Calculate current controller velocity
        prev_pos = curr_pos;
        curr_pos = transform.position;

        prev_v = curr_v;
        curr_v = new Vector3((curr_pos.x-prev_pos.x).Time.deltaTime, (curr_pos.y-prev_pos.y).Time.deltaTime, (curr_pos.z-prev_pos.z).Time.deltaTime);

        dv = new Vector3((curr_v.x - prev_v.x)/Time.deltaTime, (curr_v.y - prev_v.y)/Time.deltaTime, (curr_v.z - prev_v.z)/Time.deltaTime);

        Debug.Log(dv);

        // Flip current controller velocity to opposite direction
        return new Vector3(-1*dv.x, -1*dv.y, -1*dv.z)

    }

    void ApplyBoost()
    {
        // Apply boost to character controller using acceleration
        Vector3 move = boost_vel * Time.deltaTime;
        characterController.Move(move);
    }
}
