using UnityEngine;
using TMPro; 

public class CoinSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText; 
    private int coin = 0; 

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coin.ToString();
        }
    }

    public int GetCoin()
    {
        return coin;
    }
}
