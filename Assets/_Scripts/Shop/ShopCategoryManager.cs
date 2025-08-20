using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCategoryManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string categoryKey = "Hat"; // "Hat", "Pant", "Shirt"...

    [Header("Items")]
    [SerializeField] private List<ShopEntry> entries = new();

    [Header("UI")]
    [SerializeField] private Button actionButton;         // nút xanh
    [SerializeField] private TextMeshProUGUI actionLabel; // text trên nút xanh

    // Keys lưu
    private string OwnedKey => $"{categoryKey}_OwnedIds";
    private string EquippedKey => $"{categoryKey}_EquippedId";

    private HashSet<string> owned = new();
    private string selectedId;

    private void OnEnable()
    {
        LoadOwned();
        WireTiles();

        // Hiển thị cái đang mặc (hoặc cái đầu tiên)
        var equippedId = PlayerPrefs.GetString(EquippedKey, entries.Count > 0 ? entries[0].id : "");
        if (!string.IsNullOrEmpty(equippedId))
            Preview(equippedId);
        else if (entries.Count > 0)
            Preview(entries[0].id);

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionButton);

        RefreshLocks();
    }

    private void WireTiles()
    {
        foreach (var e in entries)
        {
            string id = e.id;
            e.tileButton.onClick.RemoveAllListeners();
            e.tileButton.onClick.AddListener(() => Preview(id));
        }
    }

    private void Preview(string id)
    {
        selectedId = id;

        // Hiển thị object đúng item
        foreach (var e in entries)
            if (e.previewObject) e.previewObject.SetActive(e.id == id);

        // Đổi text nút
        bool isOwned = owned.Contains(id);
        if (actionLabel)
            actionLabel.text = isOwned ? "Equip" : "Buy";
    }

    private void OnActionButton()
    {
        if (string.IsNullOrEmpty(selectedId)) return;

        if (owned.Contains(selectedId))
        {
            Equip(selectedId);
        }
        else
        {
            owned.Add(selectedId);
            SaveOwned();
            RefreshLocks();
            Equip(selectedId);
        }

        // Cập nhật lại label
        if (actionLabel)
            actionLabel.text = owned.Contains(selectedId) ? "Equip" : "Buy";
    }

    private void Equip(string id)
    {
        PlayerPrefs.SetString(EquippedKey, id);
        PlayerPrefs.Save();
        Debug.Log($"[{categoryKey}] Equip: {id}");
    }

    private void LoadOwned()
    {
        owned.Clear();
        string csv = PlayerPrefs.GetString(OwnedKey, "");
        if (!string.IsNullOrEmpty(csv))
        {
            foreach (var token in csv.Split(','))
                if (!string.IsNullOrEmpty(token)) owned.Add(token);
        }

        // Mặc định: cho free item đầu tiên
        if (entries.Count > 0 && owned.Count == 0)
        {
            owned.Add(entries[0].id);
            SaveOwned();
            PlayerPrefs.SetString(EquippedKey, entries[0].id);
            PlayerPrefs.Save();
        }
    }

    private void SaveOwned()
    {
        string csv = string.Join(",", owned);
        PlayerPrefs.SetString(OwnedKey, csv);
        PlayerPrefs.Save();
    }

    private void RefreshLocks()
    {
        foreach (var e in entries)
        {
            if (e.lockIcon)
                e.lockIcon.gameObject.SetActive(!owned.Contains(e.id));
        }
    }
}
