using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int type;
    int value = 100;
    Money money;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            money = other.GetComponentInParent<Money>();
            money.AddMoney(value);
            Destroy(gameObject);
        }
    }
}
