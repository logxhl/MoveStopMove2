using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerShop : MonoBehaviour
{
    [SerializeField] private Transform weaponHolder; //RightHand
    [SerializeField] private GameObject currentWeapon;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(weaponPrefab, weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;
    }

}
