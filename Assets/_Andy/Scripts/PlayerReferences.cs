using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    public static PlayerReferences instance;

    public GameObject playerObject;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
}
