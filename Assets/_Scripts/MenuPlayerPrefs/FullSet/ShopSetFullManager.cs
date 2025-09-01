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
    [SerializeField] private Button selectButton;
    //private int equippedPreviewSetFull = -1;
    private void OnEnable()
    {
        PlayerVisualManagerPlayerPrefs.instance.SaveCurrentState();
        PlayerVisualManagerPlayerPrefs.instance.HideAllSingleItems();
        //PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        previewInd = -1;
        if (setFullBtns.Count > 0)
        {
            PreviewSetFull(0);
        }
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
        //previewInd = ind;
        //ShowSetFull(ind);
        //Debug.Log("Preview setfull: " + previewInd);
        PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        previewInd = ind;
        ShowSetFull(ind);

        HighlightSetFull(ind); // <-- bật/tắt viền vàng đúng button
        Debug.Log("Preview setfull: " + ind);
    }
    private void HighlightSetFull(int ind)
    {
        for (int i = 0; i < setFullBtns.Count; i++)
        {
            Outline outline = setFullBtns[i].GetComponent<Outline>();

            if (outline == null)
            {
                // nếu button chưa có Outline thì tự thêm
                outline = setFullBtns[i].gameObject.AddComponent<Outline>();
                outline.effectColor = Color.yellow; // màu vàng
                outline.effectDistance = new Vector2(5f, 5f); // độ dày
            }

            // chỉ bật Outline cho item đang chọn
            outline.enabled = (i == ind);
        }
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
