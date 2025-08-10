using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        if (coinManager != null)
        {
            coinManager.onCoinChanged.AddListener(UpdateCointText);
            UpdateCointText(coinManager.Coin);
        }
    }

    public void UpdateCointText(int newCoinValue)
    {
        coinManager.Coin = newCoinValue;
        coinText.text = newCoinValue.ToString();
    }
    private void OnDestroy()
    {
        if (ReferenceEquals(coinManager, null))
            return;
        coinManager.onCoinChanged.RemoveListener(UpdateCointText);
    }
}
