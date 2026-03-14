using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalibrationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Sprite armsSide;
    [SerializeField] private Sprite armsUp;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI instructions;
    
    [Header("Settings")]
    [SerializeField] private int holdTime = 5;
    
    private enum CalibrationState
    {
        ArmsSide,
        ArmsUp,
        Complete
    }
    
    private CalibrationState currentState = CalibrationState.ArmsSide;
    private float holdTimer = 0f;
    private bool isHoldingPosition = false;
    private float maxLeftDistance = 0f;
    private float maxRightDistance = 0f;

    void Start()
    {
        StartArmsSideCalibration();
    }

    void Update()
    {
        if (currentState == CalibrationState.Complete) return;
        
        bool positionCorrect = CheckPosition();
        
        if (positionCorrect)
        {
            if (!isHoldingPosition)
            {
                isHoldingPosition = true;
                holdTimer = 0f;
                maxLeftDistance = 0f;
                maxRightDistance = 0f;
            }
            
            TrackMaximumDistances();
            
            holdTimer += Time.deltaTime;
            
            int remainingTime = Mathf.CeilToInt(holdTime - holdTimer);
            UpdateInstructionsWithTimer(remainingTime);
            
            if (holdTimer >= holdTime)
            {
                OnPositionHeld();
            }
        }
        else
        {
            if (isHoldingPosition)
            {
                isHoldingPosition = false;
                holdTimer = 0f;
                maxLeftDistance = 0f;
                maxRightDistance = 0f;
                UpdateInstructions();
            }
        }
    }
    
    private void StartArmsSideCalibration()
    {
        currentState = CalibrationState.ArmsSide;
        displayImage.sprite = armsSide;
        UpdateInstructions();
    }
    
    private void StartArmsUpCalibration()
    {
        currentState = CalibrationState.ArmsUp;
        displayImage.sprite = armsUp;
        holdTimer = 0f;
        isHoldingPosition = false;
        maxLeftDistance = 0f;
        maxRightDistance = 0f;
        UpdateInstructions();
    }
    
    private void TrackMaximumDistances()
    {
        if (PlayerReferences.instance == null) return;
        
        Transform headset = PlayerReferences.instance.cameraTransform;
        Transform leftController = PlayerReferences.instance.leftController?.transform;
        Transform rightController = PlayerReferences.instance.rightController?.transform;
        
        if (headset == null) return;
        
        if (leftController != null)
        {
            float leftDist = Vector3.Distance(headset.position, leftController.position);
            if (leftDist > maxLeftDistance)
            {
                maxLeftDistance = leftDist;
            }
        }
        
        if (rightController != null)
        {
            float rightDist = Vector3.Distance(headset.position, rightController.position);
            if (rightDist > maxRightDistance)
            {
                maxRightDistance = rightDist;
            }
        }
    }
    
    private bool CheckPosition()
    {
        if (PlayerReferences.instance == null) return false;
        
        Transform headset = PlayerReferences.instance.cameraTransform;
        Transform leftController = PlayerReferences.instance.leftController?.transform;
        Transform rightController = PlayerReferences.instance.rightController?.transform;
        
        if (headset == null) return false;
        
        if (currentState == CalibrationState.ArmsSide)
        {
            bool leftInPosition = false;
            bool rightInPosition = false;
            
            if (leftController != null)
            {
                float lateralDistance = Mathf.Abs(leftController.position.z - headset.position.z);
                Debug.Log(lateralDistance);
                leftInPosition = lateralDistance > 0.4;
            }
            
            if (rightController != null)
            {
                float lateralDistance = Mathf.Abs(rightController.position.z - headset.position.z);
                rightInPosition = lateralDistance > 0.4f;
            }
            
            return leftInPosition || rightInPosition;
        }
        else if (currentState == CalibrationState.ArmsUp)
        {
            bool leftInPosition = false;
            bool rightInPosition = false;
            
            if (leftController != null)
            {
                float heightDifference = leftController.position.y - headset.position.y;
                leftInPosition = heightDifference > 0.2f;
            }
            
            if (rightController != null)
            {
                float heightDifference = rightController.position.y - headset.position.y;
                rightInPosition = heightDifference > 0.2f;
            }
            
            return leftInPosition || rightInPosition;
        }
        
        return false;
    }
    
    private void OnPositionHeld()
    {
        if (currentState == CalibrationState.ArmsSide)
        {
            float maxDistance = Mathf.Max(maxLeftDistance, maxRightDistance);
            
            if (maxDistance > 0f)
            {
                CalibrationMeasurements.Instance.armLength = maxDistance;
                CalibrationMeasurements.Instance.comfortReach = maxDistance * 0.7f;
                
                StartArmsUpCalibration();
            }
        }
        else if (currentState == CalibrationState.ArmsUp)
        {
            float maxDistance = Mathf.Max(maxLeftDistance, maxRightDistance);
            
            if (maxDistance > 0f)
            {
                CalibrationMeasurements.Instance.upReachLength = maxDistance;
                
                currentState = CalibrationState.Complete;
                instructions.text = "Calibration Complete!";
                
                StartCoroutine(LoadVRTutorial());
            }
        }
    }
    
    private void UpdateInstructions()
    {
        if (currentState == CalibrationState.ArmsSide)
        {
            instructions.text = "Hold your arms out to the side, like a T-pose";
        }
        else if (currentState == CalibrationState.ArmsUp)
        {
            instructions.text = "Hold your arms straight up";
        }
    }
    
    private void UpdateInstructionsWithTimer(int remainingTime)
    {
        if (currentState == CalibrationState.ArmsSide)
        {
            instructions.text = $"Hold your arms out to the side, like a T-pose ({remainingTime}s)";
        }
        else if (currentState == CalibrationState.ArmsUp)
        {
            instructions.text = $"Hold your arms straight up ({remainingTime}s)";
        }
    }
    
    private IEnumerator LoadVRTutorial()
    {
        yield return new WaitForSeconds(1f);
        SceneLoader.Instance.LoadNewScene("VRTutorial");
    }
}
