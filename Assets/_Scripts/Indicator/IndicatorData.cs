using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class IndicatorData
{
    public RectTransform indicatorUI;
    public EnemyAI enemyAI;
    public Image indica; // Component Image của indicator
    public Image oval;   // Component Image của oval

    public IndicatorData(RectTransform ui, EnemyAI enemy)
    {
        indicatorUI = ui;
        enemyAI = enemy;

        // ✅ CÁCH TÌM IMAGE AN TOÀN HƠN
        FindImageComponents();
    }

    private void FindImageComponents()
    {
        Image[] allImages = indicatorUI.GetComponentsInChildren<Image>();

        // Gán Image components
        if (allImages.Length > 0)
        {
            indica = allImages[0]; // Image đầu tiên
        }

        if (allImages.Length > 1)
        {
            oval = allImages[1]; // Image thứ hai
        }

        // Cảnh báo nếu không tìm thấy
        if (indica == null) Debug.LogWarning("⚠️ Indica Image not found!");
        if (oval == null) Debug.LogWarning("⚠️ Oval Image not found!");
    }
}