using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Shop/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    public string weaponName;
    public int price;
    public GameObject weaponPrefab;
}
