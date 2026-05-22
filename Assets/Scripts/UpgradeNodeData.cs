using System.Collections.Generic;
using UnityEngine;

public enum UpgradeStatType
{

    DamagePercent,
    MaxHp,
    SpeedPercent,
    DropPercent,
    ShieldMax,
    HpRegenPerKill,
    ShieldRegenSpeed,
    CritChancePercent,

    RifleDamagePercent,
    ShotgunCountBullet,
    ShotgunSpread,
    Penetration,
    BulletSpeedPercent,
    MinigunWarmupSpeed,

    ExplosionChance,
    Reflect,
    BurnChance,
    FreezeChance,

    P1_BerserkMode,
    P1_IronSkin,
    P2_ShadowStep,
    P2_TwinShot,
}

public enum UpgradeBranch
{
    General,
    Weapon,
    Special,
    PlayerSkill
}

[CreateAssetMenu(fileName = "UpgradeNode", menuName = "UpgradeSystem/UpgradeNode")]
public class UpgradeNodeData : ScriptableObject
{
    [Header("Отображение")]
    public string upgradeName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public UpgradeBranch branch;

    [Header("0 = для всех, 1 = только Игрок 1, 2 = только Игрок 2")]
    public int playerIdFilter = 0;

    [Header("Стоимость за каждый уровень")]
    public List<UpgradeLevelCost> levelCosts;

    [Header("Значение бонуса на каждом уровне")]
    public List<float> valuesPerLevel;

    [Header("Тип стата")]
    public UpgradeStatType statType;

    [Header("ID ScriptableObject-родителя (пусто = нет)")]
    public string requiredNodeId;

    [Header("Минимальный уровень родителя")]
    public int requiredNodeMinLevel = 1;

    public int MaxLevel => levelCosts != null ? levelCosts.Count : 0;
}

[System.Serializable]
public class UpgradeLevelCost
{
    public int levelPoints;
    public List<ItemCost> items;
}

[System.Serializable]
public class ItemCost
{
    public string itemName;
    public int count;
}