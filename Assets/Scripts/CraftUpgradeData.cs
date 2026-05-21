using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftUpgradeData
{
    public int damage;
    public float timeDelayStartShootMin;
    public float timeDelayStartShootMax;
    public float timeDelayShot;
    public float spread;
    public float timeDelaySpray;
    public int countBullet;
    public int bulletPower;

    public int money;
    public List<DataBase.Item> requiredItems = new();

    public WeaponMain weapon;
    public int type;               // 0-3
    public int rarityIndex;        // 0-3
    public Color rarityColor;
}