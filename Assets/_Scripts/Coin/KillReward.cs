using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillReward : MonoBehaviour
{
    [SerializeField] private int coinReward = 5;
    private CoinManager coinManager;

    private void Awake()
    {
        coinManager = GetComponent<CoinManager>();
    }

    public void OnKill()
    {
        if (coinManager != null)
        {
            coinManager.AddCoin(coinReward);
        }
    }
}

