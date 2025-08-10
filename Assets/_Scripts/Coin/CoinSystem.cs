using UnityEngine;
using TMPro; // dùng TextMeshPro, nếu không dùng thì đổi sang UnityEngine.UI

public class CoinSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText; // Text hiển thị coin
    private int coin = 0; // Số coin hiện tại

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
