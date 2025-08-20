using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopShieldManagerPlayerPrefs : MonoBehaviour
{
    [SerializeField] private List<Button> shieldBtns;
    [SerializeField] private List<GameObject> shields;
    [SerializeField] private Button buyButton;

    private int previewIndex = -1;

    private void OnEnable()
    {
        // reset về đúng shield đã equip
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
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
        ShowShield(ind); // chỉ preview
        Debug.Log("Preview shield: " + ind);
    }

    private void BuyShield()
    {
        if (previewIndex < 0) return;

        string key = "PurchasedShield_" + previewIndex;

        if (PlayerPrefs.GetInt(key, 0) == 0)
        {
            // chưa mua => mua
            PlayerPrefs.SetInt(key, 1);
            Debug.Log("Đã mua shield: " + previewIndex);
        }

        // mặc luôn
        PlayerPrefs.SetInt("EquippedShield", previewIndex);
        PlayerPrefs.Save();

        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
    }

    private void ShowShield(int ind)
    {
        for (int i = 0; i < shields.Count; i++)
        {
            shields[i].SetActive(i == ind);
        }
    }
}
