using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnAwake : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    private void Awake()
    {
        Teleport.Instance.tp(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
    }
}
