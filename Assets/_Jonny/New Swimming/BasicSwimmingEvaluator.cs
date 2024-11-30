using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;


public class BasicSwimmingEvaluator : MonoBehaviour
{
    ActionBasedController controller;
    CharacterController characterController;
    [SerializeField] Transform head;
    [SerializeField] float velocityScale;

    Vector3 curr_v; // current velocity
    Vector3 total_v; // total velocity

    Vector3 curr_pos;
    Vector3 prev_pos;

    Vector3 boost_vel;

    float decelerationFactor = 0.98f;

    float speedFactor;
    
    bool flag = false;

    float startTime;


    // Start is called before the first frame update
    void Start()
    {
        characterController = head.GetComponent<CharacterController>();
        controller = GetComponent<ActionBasedController>();
        speedFactor = 0;
    }

    void OnEnable()
    {
        prev_pos = transform.localPosition;
        curr_pos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate delta velocity

        prev_pos = curr_pos;
        curr_pos = transform.localPosition;

        curr_v = (curr_pos-prev_pos)/Time.deltaTime;

        // Conditional: if controller is moving above X speed, apply boost using acceleration
        if (curr_v.magnitude > 0.5) {
            AddToTotalVelocity();
            if (flag == false) {
                startTime = Time.time;
            }
            flag = true;
        }
        else if (flag == true){
            flag = false;
            boost_vel = CalculateSpeedBoost();
            speedFactor = 1; // moved here!
            total_v = new Vector3(0,0,0);
        }

        speedFactor *= decelerationFactor;
        ApplyBoost();

    }

    void AddToTotalVelocity() {
        total_v += curr_v;
    }

    Vector3 CalculateSpeedBoost() {
        // Calculate average velocity
        Vector3 avg_v = total_v/(Time.time-startTime) * velocityScale; // add scaling here
        // Debug.Log(avg_v.magnitude);

        // Flip current controller velocity to opposite direction
        return new Vector3(-1*avg_v.x, -1*avg_v.y, -1*avg_v.z);

    }

    void ApplyBoost()
    {
        // Apply boost to character controller using acceleration
        Vector3 move = boost_vel * speedFactor * Time.deltaTime * CalculateVectorProjection();
        characterController.Move(move);
    }

    float CalculateVectorProjection() {
        Vector3 v1 = total_v/(Time.time-startTime) * velocityScale; // avg_v
        Vector3 v2 = transform.forward;
        v1.Normalize();
        v2.Normalize();

        Debug.Log(Vector3.Dot(v1,v2));
        
        if ( Vector3.Dot(v1, v2) > 0) return Vector3.Dot(v1, v2);
        else return 0.1f;

        return 0;
    }
}
