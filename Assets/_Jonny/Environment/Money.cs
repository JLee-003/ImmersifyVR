using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    public int money = 1000;
    public TextMeshProUGUI moneyText;
    // Start is called before the first frame update
    public static Money Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public void AddMoney(int amount) {
        money += amount;
    }
    // Update is called once per frame
    void Update()
    {
        moneyText.text = "Your Money: " + money.ToString();
    }
}
