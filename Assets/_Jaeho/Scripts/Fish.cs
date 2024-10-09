using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int type;
    int value = 100;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Money.Instance.AddMoney(value);
            Destroy(gameObject);
        }
    }
}
