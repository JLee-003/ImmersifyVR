using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationMeasurements : MonoBehaviour
{
    public static CalibrationMeasurements Instance { get; private set; }

    public float armLength;
    public float comfortReach;
    public float upReachLength;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // TODO - SET ACTUAL DEFAULT VALUES
        armLength = 0.8f;
        comfortReach = armLength * 0.6f;
        upReachLength = 0.4f;
    }
}
