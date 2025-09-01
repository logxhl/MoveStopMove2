using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPantManagerPlayerPrefs : MonoBehaviour
{
    [SerializeField] private List<Button> pantBtns;
    [SerializeField] private List<Material> pantMaterials;
    [SerializeField] private Renderer pantRenderer;
    [SerializeField] private Button buyButton;
    private int previewInd = -1;

    private void OnEnable()
    {
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
        //PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        //PlayerVisualManagerPlayerPrefs.instance.RestoreSavedState();
        previewInd = -1;
        if (pantBtns.Count > 0)
        {
            PreviewPant(0);
        }
    }
    private void OnDisable()
    {
        PlayerVisualManagerPlayerPrefs.instance.SaveCurrentState();
    }
    private void Start()
    {
        for(int i = 0; i < pantBtns.Count; i++)
        {
            int ind = i;
            pantBtns[i].onClick.AddListener(() => PreviewPant(ind));
        }
        buyButton.onClick.AddListener(BuyPant);
    }

    private void BuyPant()
    {
        if (previewInd < 0) return;
        string key = "PurchasedPant_" + previewInd;
        if(PlayerPrefs.GetInt(key, 0) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
        }
        PlayerPrefs.SetInt("EquippedSetFull", -1);
        PlayerPrefs.SetInt("EquippedPant", previewInd);
        PlayerPrefs.Save();
        PlayerVisualManagerPlayerPrefs.instance.ApplyEquippedItems();
    }

    private void PreviewPant(int ind)
    {
        //PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        //previewInd = ind;
        //ShowPant(ind);
        PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        previewInd = ind;
        ShowPant(ind);

        HighlightPant(ind); // <-- bật/tắt viền vàng đúng button
        Debug.Log("Preview pant: " + ind);
    }

    private void ShowPant(int ind)
    {
        if(pantRenderer != null && ind >= 0 && ind < pantMaterials.Count)
        {
            pantRenderer.material = pantMaterials[ind];
        }
    }
    private void HighlightPant(int ind)
    {
        for (int i = 0; i < pantBtns.Count; i++)
        {
            Outline outline = pantBtns[i].GetComponent<Outline>();

            if (outline == null)
            {
                // nếu button chưa có Outline thì tự thêm
                outline = pantBtns[i].gameObject.AddComponent<Outline>();
                outline.effectColor = Color.yellow; // màu vàng
                outline.effectDistance = new Vector2(5f, 5f); // độ dày
            }

            // chỉ bật Outline cho item đang chọn
            outline.enabled = (i == ind);
        }
    }
}
