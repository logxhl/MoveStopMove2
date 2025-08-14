using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHairManager : MonoBehaviour
{
    [SerializeField] private List<Button> hairBtns;
    [SerializeField] private List<GameObject> hairs;
    private const string saveHairKey = "SelectHair";
    private void Start()
    {
        for(int i = 0; i < hairBtns.Count; i++)
        {
            int ind = i;
            hairBtns[i].onClick.AddListener(() => SelectHair(ind));
        }   
        int saveInd = PlayerPrefs.GetInt(saveHairKey, 0);
        ShowHair(saveInd);
    }

    private void SelectHair(int ind)
    {
        ShowHair(ind);
        PlayerPrefs.SetInt(saveHairKey, ind);
        PlayerPrefs.Save();
        Debug.Log("Da cho mu: " + ind);
    }

    private void ShowHair(int ind)
    {
        for (int i = 0; i < hairs.Count; i++)
        {
            hairs[i].SetActive(i == ind);
        }
    }
}
