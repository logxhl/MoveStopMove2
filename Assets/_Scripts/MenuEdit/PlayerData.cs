using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public List<int> purchasedHair = new List<int>();
    public int equippedHair = -1;

    public List<int> purchasedWeapon = new List<int>();
    public int equippedWeapon = -1;

    public List<int> purchasedShield = new List<int>();
    public int equippedShield = -1;
}
