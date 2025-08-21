using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHairManagerPlayerPrefs : MonoBehaviour
{
    [SerializeField] private List<Button> hairBtns;
    [SerializeField] private List<GameObject> hairs;
    [SerializeField] private Button buyButton;

    private int previewIndex = -1;

    private void OnEnable()
    {
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        //PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        //PlayerVisualManagerPlayerPrefs.instance.RestoreSavedState();
        previewIndex = -1;
    }
    private void OnDisable()
    {
        PlayerVisualManagerPlayerPrefs.instance.SaveCurrentState();
    }
    private void Start()
    {
        for (int i = 0; i < hairBtns.Count; i++)
        {
            int ind = i;
            hairBtns[i].onClick.AddListener(() => PreviewHair(ind));
        }

        buyButton.onClick.AddListener(BuyHair);
    }

    private void PreviewHair(int ind)
    {
        PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        previewIndex = ind;
        ShowHair(ind);
        Debug.Log("Preview hair: " + ind);
    }

    private void BuyHair()
    {
        if (previewIndex < 0) return;

        string key = "PurchasedHair_" + previewIndex;

        if (PlayerPrefs.GetInt(key, 0) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            Debug.Log("Đã mua hair: " + previewIndex);
        }

        // Gỡ setfull khi chọn item lẻ
        PlayerPrefs.SetInt("EquippedSetFull", -1);

        // Trang bị tóc vừa mua
        PlayerPrefs.SetInt("EquippedHair", previewIndex);

        PlayerPrefs.Save();

        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
    }

    private void ShowHair(int ind)
    {
        for (int i = 0; i < hairs.Count; i++)
        {
            hairs[i].SetActive(i == ind);
        }
    }
}
