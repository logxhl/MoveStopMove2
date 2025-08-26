using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldShopManager : MonoBehaviour
{
    public PlayerSceneZombie player;
    public void BuyShield()
    {
        player.shieldCount++;
        UIManager.instance.UpdateShield(player.shieldCount);
    }
}
