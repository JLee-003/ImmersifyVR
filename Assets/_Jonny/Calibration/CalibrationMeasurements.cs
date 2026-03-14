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

        // TODO - SET DEFAULT VALUES
        
    }
}
