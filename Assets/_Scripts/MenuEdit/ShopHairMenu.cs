using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShopHairMenu : MonoBehaviour
{
    [SerializeField] private List<Button> hairBtns;    // nút chọn tóc
    [SerializeField] private List<GameObject> hairs;   // các prefab tóc
    [SerializeField] private Button buyButton;         // nút mua

    private int previewIndex = -1;

    private void OnEnable()
    {
        // reset hiển thị về những gì đã equip
        PlayerVisualManager.instance.ApplyEquippedItems();
        previewIndex = -1;
    }

    private void Start()
    {
        // gán sự kiện click cho các nút tóc
        for (int i = 0; i < hairBtns.Count; i++)
        {
            int ind = i;
            hairBtns[i].onClick.AddListener(() => PreviewHair(ind));
        }

        // gán sự kiện cho nút mua
        buyButton.onClick.AddListener(BuyHair);
    }

    private void PreviewHair(int ind)
    {
        previewIndex = ind;
        ShowHair(ind);
        Debug.Log("Preview tóc: " + ind);
    }

    private void BuyHair()
    {
        if (previewIndex < 0) return;

        var data = DataManager.instance.data;

        // nếu chưa mua thì thêm vào danh sách purchased
        if (!data.purchasedHair.Contains(previewIndex))
        {
            data.purchasedHair.Add(previewIndex);
            Debug.Log("Đã mua tóc mới: " + previewIndex);
        }
        else
        {
            Debug.Log("Tóc này đã mua, chỉ trang bị lại: " + previewIndex);
        }

        // mặc luôn tóc này
        data.equippedHair = previewIndex;
        DataManager.instance.SaveData();
    }

    private void ShowHair(int ind)
    {
        for (int i = 0; i < hairs.Count; i++)
        {
            hairs[i].SetActive(i == ind);
        }
    }

    private void HideAllHair()
    {
        for (int i = 0; i < hairs.Count; i++)
        {
            hairs[i].SetActive(false);
        }
    }
}
