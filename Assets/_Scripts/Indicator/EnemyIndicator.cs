using TMPro;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
    public TextMeshProUGUI textCoinEnemy;

    private TextMeshProUGUI enemyCoinText;

    public void Init(TextMeshProUGUI enemyCoin)
    {
        enemyCoinText = enemyCoin;
    }

    private void Update()
    {
        if (enemyCoinText != null)
        {
            textCoinEnemy.text = enemyCoinText.text;
        }
    }
}
