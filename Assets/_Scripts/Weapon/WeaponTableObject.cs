using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class WeaponTableObject : ScriptableObject
{
    public Weapon[] weapon;
    public int GetCountWeapon()
    {
        return weapon.Length;
    }

    public Weapon GetWeapon(int index)
    {
        return weapon[index];
    }
}
