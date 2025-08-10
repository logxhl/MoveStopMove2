using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public Sprite icon;
    public GameObject modelPrefab;
    public GameObject visual;
    public float damage = 10f;
    public float speed = 12f;
}