using System.Collections.Generic;
using UnityEngine;

public static class CraftSystem
{
    private const int LEGENDARY_CHANCE = 90;
    private const int EPIC_CHANCE = 70;
    private const int UNCOMMON_CHANCE = 40;

    private static readonly float[] rarityMultipliers =
    {
        0.6f,   // common
        1.2f,   // uncommon
        1.8f,   // epic
        2.6f    // legendary
    };

    public static CraftUpgradeData GenerateUpgrade(WeaponMain weapon)
    {
        CraftUpgradeData data = new CraftUpgradeData();

        int type = Random.Range(0, 4);
        int rarityIndex = GetRarityIndex();

        float multiplier = rarityMultipliers[rarityIndex];

        data.weapon = weapon;
        data.type = type;
        data.rarityIndex = rarityIndex;
        data.rarityColor = DataBase.colors[rarityIndex];

        GenerateStats(data, weapon, multiplier);
        GenerateCost(data, rarityIndex, multiplier);

        return data;
    }

    private static int GetRarityIndex()
    {
        int roll = Random.Range(0, 100);

        if (roll > LEGENDARY_CHANCE) return 3;
        if (roll > EPIC_CHANCE) return 2;
        if (roll > UNCOMMON_CHANCE) return 1;
        return 0;
    }

    private static void GenerateStats(CraftUpgradeData data, WeaponMain weapon, float multiplier)
    {
        data.damage = Mathf.CeilToInt(weapon.damageBullet * (1f + 0.25f * multiplier));
        data.bulletPower = Mathf.CeilToInt(weapon.forceBullet * (1f + 0.15f * multiplier));

        switch (data.type)
        {
            case 0: //melee
                data.timeDelayShot = weapon.timeDelayShot * (1f - 0.08f * multiplier);
                break;

            case 1: //shotgun
                data.timeDelayShot = weapon.timeDelayShot * (1f - 0.07f * multiplier);
                if (multiplier > 1.5f)
                    data.countBullet += 1;
                if (multiplier > 2f)
                    data.spread += 1f;
                break;

            case 2: //rifle
                data.timeDelayShot = weapon.timeDelayShot * (1f - 0.1f * multiplier);
                if (multiplier > 1.5f)
                    data.countBullet += 1;
                break;

            case 3: //minigun
                data.timeDelayStartShootMin = weapon.timeDelayStartShootMin - 0.1f * multiplier;
                data.timeDelayStartShootMax = weapon.timeDelayStartShootMax - 0.05f * multiplier;
                break;
        }
    }

    private static void GenerateCost(CraftUpgradeData data, int rarityIndex, float multiplier)
    {
        data.money = Mathf.RoundToInt(60 * multiplier);

        int itemCount = rarityIndex + 1;

        for (int i = 0; i < itemCount; i++)
        {
            DataBase.Item.classResource resClass =
                (DataBase.Item.classResource)Mathf.Clamp(rarityIndex, 0, 3);

            DataBase.Item baseItem = DataBase.ReturnRandomItem(resClass);

            DataBase.Item newItem = new DataBase.Item();
            newItem.nameItem = baseItem.nameItem;
            newItem.countItem = Random.Range(1, 3 + rarityIndex);
            data.requiredItems.Add(newItem);
        }
    }
}