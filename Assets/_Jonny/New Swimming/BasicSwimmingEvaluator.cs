using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;


// USING DELTA VELOCITY OR VELOCITY???
public class BasicSwimmingEvaluator : MonoBehaviour
{
    ActionBasedController controller;
    CharacterController characterController;
    [SerializeField] Transform head;

    bool measuring = false;

    Vector3 curr_v; // current velocity
    Vector3 prev_v; // preivous velocity
    Vector3 total_v; // total velocity

    Vector3 curr_pos;
    Vector3 prev_pos;

    Vector3 boost_vel;

    float decelerationFactor = 0.98f;

    float speedFactor;
    
    bool flag;

    // Start is called before the first frame update
    void Start()
    {
        characterController = head.GetComponent<CharacterController>();
        controller = GetComponent<ActionBasedController>();
        speedFactor = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate delta velocity
        boost_vel = CalculateSpeedBoost();

        // Conditional: if controller is moving above X speed, apply boost using acceleration
        if (boost_vel.x + boost_vel.y + boost_vel.z > 3) {
            AddToTotalVelocity();
            flag = true;
        }
        else if (flag == true){
            flag = false;
            CalculateSpeedBoost();
            speedFactor = 1; // moved here!
            total_v = new Vector3(0,0,0);
        }

        speedFactor *= decelerationFactor;
        ApplyBoost();

    }

    void AddToTotalVelocity() {
        prev_pos = curr_pos;
        curr_pos = transform.localPosition; // here

        prev_v = curr_v;
        curr_v = new Vector3((curr_pos.x-prev_pos.x)/Time.deltaTime, (curr_pos.y-prev_pos.y)/Time.deltaTime, (curr_pos.z-prev_pos.z)/Time.deltaTime);

        total_v += curr_v;
    }

    Vector3 CalculateSpeedBoost() {
        // Calculate average velocity
        Vector3 avg_v = total_v/Time.deltaTime;

        // Flip current controller velocity to opposite direction
        return new Vector3(-1*avg_v.x, -1*avg_v.y, -1*avg_v.z);

    }

    void ApplyBoost()
    {
        // Apply boost to character controller using acceleration
        Vector3 move = boost_vel * speedFactor; // * Time.deltaTime
        characterController.Move(move);
    }
}
