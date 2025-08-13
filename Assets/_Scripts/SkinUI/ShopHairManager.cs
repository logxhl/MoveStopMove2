using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHairManager : MonoBehaviour
{
    [SerializeField] private List<Button> hairBtns;
    [SerializeField] private List<GameObject> hairs;

    private void Start()
    {
        for(int i = 0; i < hairBtns.Count; i++)
        {
            int ind = i;
            hairBtns[i].onClick.AddListener(() => SelectHair(ind));
        }   
    }

    private void SelectHair(int ind)
    {
        foreach(var hair in hairs)
        {
            hair.SetActive(false);
        }
        hairs[ind].SetActive(true);
    }
}
