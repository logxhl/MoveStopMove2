using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWeapon : MonoBehaviour
{
    [SerializeField] private WeaponManagerShop weaponManagerShop;
    [SerializeField] private WeaponDataSO[] weapons;
    
    public void BuyWeapon(int index)
    {
        WeaponDataSO weaponData = weapons[index];
        weaponManagerShop.EquipWeapon(weaponData.weaponPrefab);
        Debug.Log("Bougt: " + weaponData.weaponName);
    }
    
}
