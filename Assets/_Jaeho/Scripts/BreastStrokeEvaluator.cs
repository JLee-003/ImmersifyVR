using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BreastStrokeEvaluator : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform hand;
    [SerializeField] float measureInterval;
    [SerializeField] float stabilityTolerance, arcLengthTolerance, heightTolerance, sizeMultiplier, speedMultipler;
    [SerializeField] float stabilityBoostAmount, arcLengthBoostAmount, heightBoostAmount;

    ActionBasedController controller;
    CharacterController characterController;

    float measureTimer = 0f;
    bool measuring = false;
    float actionDuration = 0f;
    float initialRadius;
    List<float> measuredRadii = new List<float>();
    float initialHeight;
    List<float> measuredHeights = new List<float>();
    Vector3 initialHandPos, endHandPos;

    public float boostDuration = 1f;
    float boostEndTime;
    Vector3 boostDirection;

    float totalBoost = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        controller = hand.GetComponent<ActionBasedController>();
        Debug.Log(controller);
        Debug.Log(controller.transform.position);
    }
    private void Update()
    {
        if (controller.selectAction.action.ReadValue<float>() > 0.1f)
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
        }
        if (measureTimer >= measureInterval)
        {
            Measure();
            measureTimer = 0f;
        }
        if (Time.time < boostEndTime)
        {
            Vector3 move = boostDirection * totalBoost * Time.deltaTime;
            characterController.Move(move);
        }
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
        Debug.Log("Measuring start");
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
        Debug.Log("Measuring stop");
        measuring = false;
        endHandPos = hand.position;
    }
    void ApplyBoost()
    {
        totalBoost = StabilityBoost() + ArcLengthBoost() + SizeBoost() + HeightBoost() + SpeedBoost();
        Debug.Log("Total Boost: " + totalBoost);

        boostDirection = initialHandPos - head.position;
        boostDirection.Normalize();
        boostEndTime = Time.time + boostDuration;
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
        Debug.Log($"Initial radius: {initialRadius}, Avg diff: {avg} - {stabilityTolerance} Tolerance");
        return boost;
    }
    float ArcLengthBoost()
    {
        float boost = 0f;
        Vector3 initialVector = initialHandPos - head.position;
        Vector3 endVector = endHandPos - head.position;
        float angle = Vector3.Angle(initialVector, endVector);
        boost = angle < arcLengthTolerance ? arcLengthBoostAmount : 0;
        Debug.Log($"Arc Angle: {angle} - {arcLengthTolerance} Tolerance from 90 deg.");
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
        Debug.Log($"Avg Radius: {avgRadius}, SizeBoost: {boost}");
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
        Debug.Log($"Initial height: {initialHeight}, Avg diff: {avg} - {heightTolerance} Tolerance");
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
        estimatedDistance = 0.5f * Mathf.PI * avgRadius;
        float avgSpeed = (estimatedDistance / actionDuration);
        boost = avgSpeed * speedMultipler;
        Debug.Log($"Avg Speed: {avgSpeed}, SpeedBoost: {boost}");
        return boost;
    }
}
