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

    [Header("Full Set Settings")]
    [SerializeField] private List<FullSetInfo> fullSets;
    [SerializeField] private Renderer[] setFullRenderers;

    //Luu trang thai ban dau
    [HideInInspector] public int savedHair = -1;
    [HideInInspector] public int savedShield = -1;
    [HideInInspector] public int savedPant = -1;
    //[SerializeField] private Material savedPantMaterial;
    //[SerializeField] private Material savedInitialMaterial;

    [Header("Default Initial Material")]
    [SerializeField] private Material defaultInitialMaterial;


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
        int equippedSetFull = PlayerPrefs.GetInt("EquippedSetFull", -1);
        Debug.Log("hair: " + equippedHair);
        Debug.Log("pant: " + equippedPant);
        Debug.Log("shield: " + equippedShield);
        Debug.Log("SetFull: " + equippedSetFull);

        if (equippedSetFull >= 0 && equippedSetFull < fullSets.Count)
        {
            HideAllSingleItems();
            ShowFullSet(equippedSetFull);
        }
        else
        {
            ShowFullSet(-1);

            ShowItem(hairs, equippedHair);
            ShowItem(shields, equippedShield);
            ShowPant(equippedPant);
            if (setFullRenderers.Length > 1 && setFullRenderers[1] != null && defaultInitialMaterial != null)
                setFullRenderers[1].material = defaultInitialMaterial;
        }
    }

    public void SaveCurrentState()
    {
        savedHair = PlayerPrefs.GetInt("EquippedHair", -1);
        savedShield = PlayerPrefs.GetInt("EquippedShield", -1);
        savedPant = PlayerPrefs.GetInt("EquippedPant", -1);
        //PlayerPrefs.SetInt("EquippedSetFull", -1);


        //if (setFullRenderers.Length > 0 && setFullRenderers[0] != null)
        //    savedPantMaterial = setFullRenderers[0].material;

        //if (setFullRenderers.Length > 1 && setFullRenderers[1] != null)
        //    savedInitialMaterial = setFullRenderers[1].material;
    }

    public void RestoreSavedState()
    {

        // load lại dữ liệu đã lưu
        ShowItem(hairs, savedHair);
        ShowItem(shields, savedShield);
        ShowPant(savedPant);

        // pant vẫn dùng material đã lưu từ index
        // initial thì reset về default
        if (setFullRenderers.Length > 1 && setFullRenderers[1] != null && defaultInitialMaterial != null)
            setFullRenderers[1].material = defaultInitialMaterial;

        // tắt toàn bộ fullset
        for (int i = 0; i < fullSets.Count; i++)
        {
            fullSets[i].hairFull.SetActive(false);
            fullSets[i].shieldFull.SetActive(false);
            fullSets[i].tailFull.SetActive(false);
            fullSets[i].wingFull.SetActive(false);
        }
    }


    //public void ShowFullSet(int equippedSetFull)
    //{
    //    for(int i = 0; i < fullSets.Count; i++)
    //    {
    //        bool active = (i == equippedSetFull);
    //        fullSets[i].hairFull.SetActive(active);
    //        fullSets[i].shieldFull.SetActive(active);
    //        fullSets[i].tailFull.SetActive(active);
    //        fullSets[i].wingFull.SetActive(active);
    //    }

    //    if(equippedSetFull >= 0 && equippedSetFull < fullSets.Count)
    //    {
    //        if(setFullRenderers.Length > 0 && setFullRenderers[0] != null)
    //        {
    //            setFullRenderers[0].material = fullSets[equippedSetFull].pantFull;
    //        }
    //        if(setFullRenderers.Length > 1 && setFullRenderers[1] != null)
    //        {
    //            setFullRenderers[1].material = fullSets[equippedSetFull].initialFull;
    //        }
    //    }
    //    if(equippedSetFull == -1)
    //    {
    //        setFullRenderers[1].material = defaultInitialMaterial;
    //    }
    //}
    public void ShowFullSet(int equippedSetFull)
    {
        for (int i = 0; i < fullSets.Count; i++)
        {
            bool active = (i == equippedSetFull);

            if (fullSets[i].hairFull != null)
                fullSets[i].hairFull.SetActive(active);
            if (fullSets[i].shieldFull != null)
                fullSets[i].shieldFull.SetActive(active);
            if (fullSets[i].tailFull != null)
                fullSets[i].tailFull.SetActive(active);
            if (fullSets[i].wingFull != null)
                fullSets[i].wingFull.SetActive(active);
        }

        if (equippedSetFull >= 0 && equippedSetFull < fullSets.Count)
        {
            if (setFullRenderers.Length > 0 && setFullRenderers[0] != null)
                setFullRenderers[0].material = fullSets[equippedSetFull].pantFull;

            if (setFullRenderers.Length > 1 && setFullRenderers[1] != null)
                setFullRenderers[1].material = fullSets[equippedSetFull].initialFull;
        }
        else if (equippedSetFull == -1)
        {
            if (setFullRenderers.Length > 1 && setFullRenderers[1] != null)
                setFullRenderers[1].material = defaultInitialMaterial;
        }
    }


    private void ShowPant(int index)
    {
        if (pantRenderer != null && index >= 0 && index < pantMaterials.Count)
        {
            pantRenderer.material = pantMaterials[index];
        }
    }

    //private void ShowItem(List<GameObject> list, int index)
    //{
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        list[i].SetActive(i == index && index >= 0);
    //        //if (i == index)
    //        //{
    //        //    Debug.Log(list[i].name);
    //        //}
    //    }
    //}
    private void ShowItem(List<GameObject> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) continue; // bỏ qua nếu object đã bị destroy
            list[i].SetActive(i == index && index >= 0);
        }
    }


    public void HideAllSingleItems()
    {
        ShowItem(hairs, -1);
        ShowItem(shields, -1);
        ShowPant(-1);
    }
}
