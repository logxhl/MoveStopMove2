using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSetFullManager : MonoBehaviour
{
    [SerializeField] private List<Button> setFullBtns;
    [SerializeField] private List<FullSetInfo> fullSets;
    [SerializeField] private Renderer[] setFullRenderer;
    [SerializeField] private Button buyButton;
    private int previewInd = -1;

    private void OnEnable()
    {
        PlayerVisualManagerPlayerPrefs.instance.SaveCurrentState();
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        previewInd = -1;
    }
    private void OnDisable()
    {
        PlayerVisualManagerPlayerPrefs.instance.RestoreSavedState();
    }
    private void Start()
    {
        for (int i = 0; i < setFullBtns.Count; i++)
        {
            int ind = i;
            setFullBtns[i].onClick.AddListener(() => PreviewSetFull(ind));
        }
        buyButton.onClick.AddListener(BuySetFull);
    }

    private void BuySetFull()
    {
        if (previewInd < 0) return;

        string key = "PurchasedSetFull_" + previewInd;
        if (PlayerPrefs.GetInt(key, 0) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            Debug.Log("Đã mua setfull: " + previewInd);
        }

        PlayerPrefs.SetInt("EquippedSetFull", previewInd);
        //PlayerPrefs.SetInt("EquippedSetFull", -1);

        // gỡ các item lẻ khi đã chọn fullset
        PlayerPrefs.SetInt("EquippedHair", -1);
        PlayerPrefs.SetInt("EquippedPant", -1);
        PlayerPrefs.SetInt("EquippedShield", -1);

        PlayerPrefs.Save();
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
    }

    private void PreviewSetFull(int ind)
    {
        previewInd = ind;
        ShowSetFull(ind);
        Debug.Log("Preview setfull: " + previewInd);
    }

    private void ShowSetFull(int ind)
    {
        // Ẩn toàn bộ đồ lẻ khi preview
        PlayerVisualManagerPlayerPrefs.instance.HideAllSingleItems();

        // Bật đúng fullset đang preview
        for (int i = 0; i < fullSets.Count; i++)
        {
            bool active = (i == ind);
            fullSets[i].hairFull.SetActive(active);
            fullSets[i].shieldFull.SetActive(active);
            fullSets[i].tailFull.SetActive(active);
            fullSets[i].wingFull.SetActive(active);
        }

        // Đổi material pant & body
        if (ind >= 0 && ind < fullSets.Count)
        {
            if (setFullRenderer.Length > 0 && setFullRenderer[0] != null)
                setFullRenderer[0].material = fullSets[ind].pantFull;   // pant

            if (setFullRenderer.Length > 1 && setFullRenderer[1] != null)
                setFullRenderer[1].material = fullSets[ind].initialFull; // body
        }
    }
}
