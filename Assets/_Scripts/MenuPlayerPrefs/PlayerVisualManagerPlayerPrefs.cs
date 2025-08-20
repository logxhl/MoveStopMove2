using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualManagerPlayerPrefs : MonoBehaviour
{
    [SerializeField] private List<GameObject> hairs;
    [SerializeField] private List<GameObject> shields;

    [Header("Pant Settings")]
    [SerializeField] private Renderer pantRenderer;
    [SerializeField] private List<Material> pantMaterials;

    public static PlayerVisualManagerPlayerPrefs instance;

    private void Awake()
    {
        instance = this;
    }

    public void ApplyEquippedItems()
    {
        // lấy index đã trang bị trong PlayerPrefs
        int equippedHair = PlayerPrefs.GetInt("EquippedHair", -1);
        int equippedShield = PlayerPrefs.GetInt("EquippedShield", -1);
        int equippedPant = PlayerPrefs.GetInt("EquippedPant", -1);

        ShowItem(hairs, equippedHair);
        ShowItem(shields, equippedShield);
        ShowPant(equippedPant);
    }

    private void ShowPant(int index)
    {
        if (pantRenderer != null && index >= 0 && index < pantMaterials.Count)
        {
            pantRenderer.material = pantMaterials[index];
        }
    }

    private void ShowItem(List<GameObject> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(i == index && index >= 0);
        }
    }
}
