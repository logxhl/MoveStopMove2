using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantsChange : MonoBehaviour
{
    [SerializeField] private Renderer pantsRenderer;
    [SerializeField] private Material[] pantsMaterials;

    public void ChangePants(int index)
    {
        if(pantsRenderer != null && index >= 0 && index < pantsMaterials.Length)
        {
            pantsRenderer.material = pantsMaterials[index];
        }
    }
}
