using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyText : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    // Start is called before the first frame update
    void Update()
    {
        moneyText.text = "Your Money: " + Money.Instance.money.ToString();
    }
}
