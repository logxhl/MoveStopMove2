using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseWepon : MonoBehaviour
{
    public static ChooseWepon instance;
    public WeaponTableObject weaponData;
    public Image imgWeapon;
    public int count;
    public TextMeshProUGUI nameWeapon;
    public TextMeshProUGUI unLock;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI coin;

    [Header("Weapon Holder(Right Hand)")]
    public Transform weaponHolder;
    private GameObject currentWeapon;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        if (weaponHolder.childCount > 0)
        {
            currentWeapon = weaponHolder.GetChild(0).gameObject;
        }
    }

    private void Start()
    {
        count = 0;

    }
    public void NextButton()
    {
        count++;
        if(count > (weaponData.GetCountWeapon() -1))
        {
            count = 0;
        }
        UpdateWeapon(count);
    }
    public void BackButton()
    {
        count--;
        if(count < 0)
        {
            count = weaponData.GetCountWeapon() - 1;
        }
        UpdateWeapon(count);
    }
    public void UpdateWeapon(int count)
    {
        nameWeapon.text = weaponData.GetWeapon(count).nameWepon;
        unLock.text = weaponData.GetWeapon(count).unlock;
        damage.text = weaponData.GetWeapon(count).damage;
        imgWeapon.sprite = weaponData.GetWeapon(count).spite;
        coin.text = weaponData.GetWeapon(count).coin.ToString();
    }
    public void BuyWeapon()
    {
        Weapon selectedWeapon = weaponData.GetWeapon(count);
        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(selectedWeapon.weaponPrefab, weaponHolder);
        //currentWeapon.transform.localPosition = Vector3.zero;
        //currentWeapon.transform.localRotation = Quaternion.identity;
        //currentWeapon.transform.localScale = Vector3.one;
        Debug.Log("Equiped: " + selectedWeapon.nameWepon);
    }
}
