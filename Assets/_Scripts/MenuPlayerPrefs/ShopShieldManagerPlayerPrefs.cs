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
        //PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        //PlayerVisualManagerPlayerPrefs.instance.RestoreSavedState();
        previewIndex = -1;
        if (shieldBtns.Count > 0)
        {
            PreviewShield(0);
        }
    }
    private void OnDisable()
    {
        PlayerVisualManagerPlayerPrefs.instance.SaveCurrentState();
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
        //PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        //previewIndex = ind;
        //ShowShield(ind); // chỉ preview
        //Debug.Log("Preview shield: " + ind);
        PlayerVisualManagerPlayerPrefs.instance.ShowFullSet(-1);
        previewIndex = ind;
        ShowShield(ind);

        HighlightShield(ind); // <-- bật/tắt viền vàng đúng button
        Debug.Log("Preview shield: " + ind);
    }
    private void HighlightShield(int ind)
    {
        for (int i = 0; i < shieldBtns.Count; i++)
        {
            Outline outline = shieldBtns[i].GetComponent<Outline>();

            if (outline == null)
            {
                // nếu button chưa có Outline thì tự thêm
                outline = shieldBtns[i].gameObject.AddComponent<Outline>();
                outline.effectColor = Color.yellow; // màu vàng
                outline.effectDistance = new Vector2(5f, 5f); // độ dày
            }

            // chỉ bật Outline cho item đang chọn
            outline.enabled = (i == ind);
        }
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
        // Gỡ setfull khi chọn item lẻ
        PlayerPrefs.SetInt("EquippedSetFull", -1);
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
