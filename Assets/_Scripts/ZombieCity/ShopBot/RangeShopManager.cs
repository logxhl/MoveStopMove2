using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RangeShopManager : MonoBehaviour
{
    public Button btnBuyRange;
    public TextMeshProUGUI txtRangePercent;
    private int rangeLevel = 0;
    private void Start()
    {
        btnBuyRange.onClick.AddListener(BuyRange);
        UpdateRangeText();
    }

    private void UpdateRangeText()
    {
        txtRangePercent.text = "+ " + (rangeLevel * 10) + " % Range";
    }

    private void BuyRange()
    {
        PlayerSceneZombie player = PlayerSceneZombie.instance;
        player.IncreaseRange(0.1f);
        rangeLevel++;
        UpdateRangeText();
    }
}
