using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopShieldManager : MonoBehaviour
{
    [SerializeField] private List<Button> shieldBtns;
    [SerializeField] private List<GameObject> shields;
    private const string saveShieldKey = "SelectShield";

    private void Start()
    {
        for(int i = 0; i < shieldBtns.Count; i++)
        {
            int ind = i;
            shieldBtns[i].onClick.AddListener(() => SelectShield(ind));
        }
        int saveInd = PlayerPrefs.GetInt(saveShieldKey, 0);
        ShowShield(saveInd);
    }

    private void ShowShield(int saveInd)
    {
        for(int i = 0; i < shields.Count; i++)
        {
            shields[i].SetActive(i == saveInd);
        }
    }

    private void SelectShield(int ind)
    {
        ShowShield(ind);
        PlayerPrefs.SetInt(saveShieldKey, ind);
        PlayerPrefs.Save();
        Debug.Log("Da chon shield: " + ind);
    }
}

