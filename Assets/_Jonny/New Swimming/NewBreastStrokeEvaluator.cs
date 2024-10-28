using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class NewBreastStrokeEvaluator : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform hand;
    float measureInterval = 0.05f;
    float stabilityTolerance = 0.19f, arcLengthTolerance = 30f, heightTolerance = 0.1f, sizeMultiplier = 1f, speedMultipler = 2f;
    float stabilityBoostAmount = 0.75f, arcLengthBoostAmount = 3f, heightBoostAmount = 0.75f;

    ActionBasedController controller;
    CharacterController characterController;

    float measureTimer = 0f;
    bool measuring = false;
    float actionDuration = 0f;
    float initialRadius;
    List<float> measuredRadii = new List<float>();
    float initialHeight;
    List<float> measuredHeights = new List<float>();
    Vector3 initialHandPos, endHandPos, initialHeadPos;

    Vector3 boostDirection;

    float speedCap = 10f;
    float decelerationFactor = 0.98f;
    float totalBoost = 0f;
    public float currentSpeed = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        controller = hand.GetComponent<ActionBasedController>();
    }
    private void Update()
    {   
        // Do I need to use mod to take care of the case where the controller is rotated over 360 degrees? Like, 450 degrees? Because I don't think people can rotate their hands that much.
        if (Math.Abs(Math.Abs(controller.transform.rotation.eulerAngles.z) - 90 ) <= 20) 
        {
            if (!measuring)
            {
                StartMeasuring();
            }
        }
        else if (measuring)
        {
            StopMeasuring();
            ApplyBoost();
        }
        if (measuring)
        {
            actionDuration += Time.deltaTime;
            measureTimer += Time.deltaTime;
            if (measureTimer >= measureInterval)
            {
                Measure();
                measureTimer = 0f;
            }
        }
        
        Vector3 move = boostDirection * currentSpeed * Time.deltaTime;
        characterController.Move(move);
    }
    private void FixedUpdate()
    {
        currentSpeed *= decelerationFactor;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, speedCap);
    }

    void StartMeasuring()
    {
        measuring = true;
        measureTimer = 0f;
        actionDuration = 0f;
        measuredRadii.Clear();
        measuredHeights.Clear();
        initialRadius = Vector3.Distance(head.position, hand.position);
        initialHeight = hand.position.y;
        initialHandPos = hand.position;
        initialHeadPos = head.position;
        //Debug.Log("-----------Divider-----------");
        //Debug.Log("Measuring start");
    }
    void Measure()
    {
        float dist = Vector3.Distance(head.position, hand.position);
        float height = hand.position.y;
        measuredRadii.Add(dist);
        measuredHeights.Add(height);
    }
    void StopMeasuring()
    {
        //Debug.Log("Measuring stop");
        measuring = false;
        endHandPos = hand.position;
    }
    void ApplyBoost()
    {
        totalBoost = StabilityBoost() + ArcLengthBoost() + SizeBoost() + HeightBoost() + SpeedBoost();
        //Debug.Log($"Total Boost: {totalBoost}");

        boostDirection = initialHandPos - initialHeadPos;
        boostDirection.Normalize();

        currentSpeed = totalBoost;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, speedCap);
    }
    float StabilityBoost()
    {
        float boost;
        float sum = 0f;
        foreach (float radius in measuredRadii)
        {
            sum += Mathf.Abs(initialRadius - radius);
        }
        float avg = sum / measuredRadii.Count;
        boost = avg < stabilityTolerance ? stabilityBoostAmount : 0;
        //Debug.Log($"Avg Radius diff: {avg}, StabilityBoost: {boost}");
        return boost;
    }
    float ArcLengthBoost()
    {
        float boost = 0f;
        Vector3 leveledEndHandPos = new Vector3(endHandPos.x, initialHandPos.y, endHandPos.z);
        Vector3 initialVector = initialHandPos - head.position;
        Vector3 leveledEndVector = leveledEndHandPos - head.position;
        float angle = Vector3.Angle(initialVector, leveledEndVector);
        float diffFromRightAngle = Mathf.Abs(90 - angle);
        boost = diffFromRightAngle < arcLengthTolerance ? arcLengthBoostAmount : 0;
        //Debug.Log($"Angle: {angle}, ArcLengthBoost: {boost}");
        return boost;
    }
    float SizeBoost()
    {
        float boost;
        float avgRadius;
        float radiiSum = initialRadius;
        foreach (float radius in measuredRadii)
        {
            radiiSum += radius;
        }
        avgRadius = radiiSum / (measuredRadii.Count + 1);
        boost = avgRadius * sizeMultiplier;
        //Debug.Log($"Avg Radius: {avgRadius}, SizeBoost: {boost}");
        return boost;
    }
    float HeightBoost()
    {
        float boost;
        float sum = 0f;
        foreach (float height in measuredHeights)
        {
            sum += Mathf.Abs(initialHeight - height);
        }
        float avg = sum / measuredHeights.Count;
        boost = avg < heightTolerance ? heightBoostAmount : 0;
        //Debug.Log($"Avg Height diff: {avg}, HeightBoost: {boost}");
        return boost;
    }
    float SpeedBoost()
    {
        float boost = 0f;
        float estimatedDistance;
        float avgRadius;
        float radiiSum = initialRadius;
        foreach (float radius in measuredRadii)
        {
            radiiSum += radius;
        }
        avgRadius = radiiSum / (measuredRadii.Count + 1);

        Vector3 initialVector = initialHandPos - head.position;
        Vector3 endVector = endHandPos - head.position;
        float angle = Vector3.Angle(initialVector, endVector);

        estimatedDistance = (angle/360) * (2 * Mathf.PI * avgRadius);
        float avgSpeed = (estimatedDistance / actionDuration);
        boost = avgSpeed * speedMultipler;
        //Debug.Log($"Avg Speed: {avgSpeed}, SpeedBoost: {boost}");
        return boost;
    }
}
