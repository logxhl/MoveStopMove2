using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopPantsManager : MonoBehaviour
{
    [SerializeField] private List<Button> pantBtns;
    [SerializeField] private List<Renderer> pantRenderers;
    [SerializeField] private List<Material> pantMaterials;
    private const string savePantKey = "SelectPant";

    private void Start()
    {
        for(int i = 0; i < pantBtns.Count; i++)
        {
            int ind = i;
            pantBtns[i].onClick.AddListener(() => SelectPants(ind));
        }

        int savedInd = PlayerPrefs.GetInt(savePantKey, 0);
        ShowPants(savedInd);
    }
    private void SelectPants(int ind)
    {
        ShowPants(ind);
        PlayerPrefs.SetInt(savePantKey, ind);
        PlayerPrefs.Save();
        Debug.Log("Da chon quan: " + ind);
    }
    private void ShowPants(int ind)
    {
        if(ind >= 0 && ind < pantMaterials.Count)
        {
            foreach(var renderer in pantRenderers)
            {
                renderer.material = pantMaterials[ind];
            }
        }
    }
}
