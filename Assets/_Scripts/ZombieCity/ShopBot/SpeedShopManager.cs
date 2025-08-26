using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedShopManager : MonoBehaviour
{
    public Button btnBuySpeed;
    public TextMeshProUGUI txtSpeedPercent;
    private int speedLevel = 0;
    private void Start()
    {
        btnBuySpeed.onClick.AddListener(BuySpeed);
        UpdateSpeedText();
    }

    private void BuySpeed()
    {
        PlayerSceneZombie player = PlayerSceneZombie.instance;
        player.IncreaseSpeed(0.1f);
        speedLevel++;
        UpdateSpeedText();
    }

    private void UpdateSpeedText()
    {
        txtSpeedPercent.text = "+ " + (speedLevel * 10) + " % Speed";
    }
}
