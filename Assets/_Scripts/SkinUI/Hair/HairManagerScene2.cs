using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairManagerScene2 : MonoBehaviour
{
    public GameObject[] hairs;
    private string saveHairKey = "SelectHair";

    private void Start()
    {
        int selectHair = PlayerPrefs.GetInt(saveHairKey, 0);
        foreach(var hair in hairs)
        {
            hair.SetActive(false);
        }
        if(selectHair >= 0 && selectHair < hairs.Length)
        {
            hairs[selectHair].SetActive(true);
        }
    }
}
