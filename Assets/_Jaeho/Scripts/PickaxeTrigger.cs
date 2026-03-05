using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeTrigger : MonoBehaviour
{
    [SerializeField] PickaxeClimb climb;
    [SerializeField] LayerMask weakPointLayer;

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & weakPointLayer) != 0)
        {
            climb.TryEngage(other.transform);
        }
    }
}
