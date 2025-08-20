using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopShieldMenu : MonoBehaviour
{
    [SerializeField] private List<Button> shieldBtns;
    [SerializeField] private List<GameObject> shields;
    [SerializeField] private Button buyButton;

    private int previewIndex = -1;

    private void OnEnable()
    {
        // reset hiển thị về những gì đã equip
        PlayerVisualManager.instance.ApplyEquippedItems();
        previewIndex = -1;
    }

    private void Start()
    {
        for (int i = 0; i < shieldBtns.Count; i++)
        {
            int ind = i;
            shieldBtns[i].onClick.AddListener(() => PreviewShield(ind));
        }
        buyButton.onClick.AddListener(BuyShield);
    }

    private void PreviewShield(int ind)
    {
        previewIndex = ind;
        ShowShield(ind);
        Debug.Log("Preview khiên: " + ind);
    }

    private void BuyShield()
    {
        if (previewIndex < 0) return;

        var data = DataManager.instance.data;
        if (!data.purchasedShield.Contains(previewIndex))
        {
            data.purchasedShield.Add(previewIndex);
        }
        data.equippedShield = previewIndex;
        DataManager.instance.SaveData();

        Debug.Log("Đã mua + trang bị khiên: " + previewIndex);
    }

    private void ShowShield(int ind)
    {
        for (int i = 0; i < shields.Count; i++)
        {
            shields[i].SetActive(i == ind);
        }
    }
}
