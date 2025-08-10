using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    public int Coin { get; set; }
    public UnityEvent<int> onCoinChanged;
    [ContextMenu("Coin")]
    public void Add() => AddCoin(1);
    public void AddCoin(int amount)
    {
        Coin += amount;
        onCoinChanged?.Invoke(Coin);
    }
}
