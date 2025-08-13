using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantsChange : MonoBehaviour
{
    [SerializeField] private Renderer pantsRenderer;
    [SerializeField] private Material[] pantsMaterials;
    private const string KEY = "SelectedPant";

    private void Start()
    {
        int savedIndex = PlayerPrefs.GetInt(KEY, 0);
        ChangePants(savedIndex);
    }

    public void ChangePants(int index)
    {
        if (pantsRenderer != null && index >= 0 && index < pantsMaterials.Length)
        {
            pantsRenderer.material = pantsMaterials[index];
        }
    }
}
