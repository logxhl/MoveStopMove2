using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPants : MonoBehaviour
{
    [SerializeField] private Renderer pantRenderer;
    [SerializeField] private Material[] pantMaterials;
    private const string savePantKeys = "SelectPant";

    private void Start()
    {
        int selectPant = PlayerPrefs.GetInt(savePantKeys);
        if(selectPant >= 0 && selectPant <= pantMaterials.Length)
        {
            pantRenderer.material = pantMaterials[selectPant];
        }
    }
}
