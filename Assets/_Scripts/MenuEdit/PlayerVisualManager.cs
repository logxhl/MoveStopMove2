using UnityEngine;
using System.Collections.Generic;

public class PlayerVisualManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> hairs;
    //[SerializeField] private List<GameObject> pants;
    [SerializeField] private List<GameObject> shields;

    public static PlayerVisualManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ApplyEquippedItems()
    {
        var data = DataManager.instance.data;

        // hiển thị hair nếu đã equip
        ShowItem(hairs, data.equippedHair);

        // hiển thị pant nếu đã equip
        //ShowItem(pants, data.equippedPant);

        // hiển thị shield nếu đã equip
        ShowItem(shields, data.equippedShield);
    }

    private void ShowItem(List<GameObject> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetActive(i == index && index >= 0);
        }
    }
}
