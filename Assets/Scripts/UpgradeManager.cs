using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    [Header("Все узлы (или пусто — загрузятся из Resources/UpgradeNodes)")]
    public List<UpgradeNodeData> allNodes;
    public Dictionary<string, int> purchasedLevels = new Dictionary<string, int>();
    public int developmentPoints = 0;

    public float bonusDamagePercent = 0f;
    public int bonusMaxHp = 0;
    public float bonusSpeedPercent = 0f;
    public float bonusDropPercent = 0f;
    public float bonusShieldMax = 0f;
    public float bonusHpRegenPerKill = 0f;
    public float bonusShieldRegenSpeed = 1f;//1 нет изменений
    public float bonusCritChancePercent = 0f;

    public float bonusRifleDamagePercent = 0f;
    public int bonusShotgunCountBullet = 0;
    public float bonusShotgunSpread = 0f;
    public bool bonusPenetration = false;
    public float bonusBulletSpeedPercent = 0f;
    public float bonusMinigunWarmupSpeed = 0f;

    public float bonusExplosionChance = 0f;
    public bool bonusReflect = false;
    public float bonusBurnChance = 0f;
    public float bonusFreezeChance = 0f;

    public bool skill_P1_BerserkMode = false;
    public bool skill_P1_IronSkin = false;
    public bool skill_P2_ShadowStep = false;
    public float skill_P2_TwinShotChance = 0f;

    public int activePlayerId = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (allNodes == null || allNodes.Count == 0)
            allNodes = Resources.LoadAll<UpgradeNodeData>("UpgradeNodes").ToList();

        foreach (var node in allNodes)
            if (!purchasedLevels.ContainsKey(node.name))
                purchasedLevels[node.name] = 0;
    }

    public void LoadFromSave(int points, List<UpgradeSaveEntry> entries)
    {
        developmentPoints = points;
        if (entries != null)
            foreach (var e in entries)
                if (purchasedLevels.ContainsKey(e.nodeId))
                    purchasedLevels[e.nodeId] = e.level;
        RecalculateBonuses();
    }

    public List<UpgradeSaveEntry> GetSaveEntries()
    {
        return purchasedLevels
            .Where(kv => kv.Value > 0)
            .Select(kv => new UpgradeSaveEntry { nodeId = kv.Key, level = kv.Value })
            .ToList();
    }

    public void AddDevelopmentPoint()
    {
        developmentPoints++;
        print($"[UpgradeManager] Очков развития: {developmentPoints}");
        FindObjectOfType<UpgradeUI>()?.RefreshPoints();
    }

    public bool TryPurchase(UpgradeNodeData node)
    {
        if (node == null) return false;

        int currentLevel = GetCurrentLevel(node);
        if (currentLevel >= node.MaxLevel)
        {
            print($"[Upgrade] {node.upgradeName}: максимум");
            return false;
        }
        if (!IsRequirementMet(node))
        {
            print($"[Upgrade] {node.upgradeName}: зависимость не выполнена");
            return false;
        }

        if (node.playerIdFilter != 0 && node.playerIdFilter != activePlayerId)
        {
            print($"[Upgrade] {node.upgradeName}: скилл другого игрока");
            return false;
        }

        UpgradeLevelCost cost = node.levelCosts[currentLevel];

        if (developmentPoints < cost.levelPoints)
        {
            print($"[Upgrade] Мало очков: нужно {cost.levelPoints}, есть {developmentPoints}");
            return false;
        }

        PlayerStats ps = GetPlayerStats();
        if (ps == null) return false;

        List<DataBase.Item> needed = ConvertCost(cost.items);
        if (!ps.CheckForMoneyItems(needed, 0))
        {
            print($"[Upgrade] Не хватает материалов для {node.upgradeName}");
            return false;
        }

        developmentPoints -= cost.levelPoints;
        ps.SpendItems(needed);
        purchasedLevels[node.name] = currentLevel + 1;
        RecalculateBonuses();
        ApplyBonusesToPlayer(ps);
        print($"[Upgrade] {node.upgradeName} → уровень {purchasedLevels[node.name]}");
        return true;
    }

    public int GetCurrentLevel(UpgradeNodeData node) => purchasedLevels.TryGetValue(node.name, out int lvl) ? lvl : 0;

    public bool IsRequirementMet(UpgradeNodeData node)
    {
        if (string.IsNullOrEmpty(node.requiredNodeId)) return true;
        return purchasedLevels.TryGetValue(node.requiredNodeId, out int lvl)
               && lvl >= node.requiredNodeMinLevel;
    }

    public bool CanAfford(UpgradeNodeData node)
    {
        int cur = GetCurrentLevel(node);
        print(IsRequirementMet(node));
        print(cur >= node.MaxLevel);
        print(node.playerIdFilter != 0 && node.playerIdFilter == activePlayerId);
        print(developmentPoints < node.levelCosts[cur].levelPoints);
        if (cur >= node.MaxLevel) return false;
        if (developmentPoints < node.levelCosts[cur].levelPoints) return false;
        if (node.playerIdFilter != 0 && node.playerIdFilter == activePlayerId) return false;
        if (developmentPoints < node.levelCosts[cur].levelPoints) return false;
        var ps = GetPlayerStats();
        if (ps == null) return false;
        return ps.CheckForMoneyItems(ConvertCost(node.levelCosts[cur].items), 0);
    }

    public bool IsVisibleForCurrentPlayer(UpgradeNodeData node)
    {
        return node.playerIdFilter == 0 || node.playerIdFilter == activePlayerId;
    }

    private void RecalculateBonuses()
    {
        bonusDamagePercent = bonusMaxHp = 0;
        bonusSpeedPercent = bonusDropPercent = bonusShieldMax = 0;
        bonusHpRegenPerKill = bonusCritChancePercent = 0;
        bonusShieldRegenSpeed = 1f;
        bonusRifleDamagePercent = bonusShotgunSpread = bonusBulletSpeedPercent = bonusMinigunWarmupSpeed = 0;
        bonusShotgunCountBullet = 0;
        bonusPenetration = bonusReflect = false;
        bonusExplosionChance = bonusBurnChance = bonusFreezeChance = 0;
        bonusSpeedPercent = 0f;
        skill_P1_BerserkMode = skill_P1_IronSkin = skill_P2_ShadowStep = false;
        skill_P2_TwinShotChance = 0;
        foreach (var node in allNodes)
        {
            int lvl = GetCurrentLevel(node);
            if (lvl <= 0) continue;

            float total = 0f;
            for (int i = 0; i < lvl && i < node.valuesPerLevel.Count; i++)
                total += node.valuesPerLevel[i];

            Apply(node.statType, total);
        }
    }

    private void Apply(UpgradeStatType t, float v)
    {
        switch (t)
        {
            case UpgradeStatType.DamagePercent: bonusDamagePercent += v; break;
            case UpgradeStatType.MaxHp: bonusMaxHp += Mathf.RoundToInt(v); break;
            case UpgradeStatType.SpeedPercent: bonusSpeedPercent += v; break;
            case UpgradeStatType.DropPercent: bonusDropPercent += v; break;
            case UpgradeStatType.ShieldMax: bonusShieldMax += v; break;
            case UpgradeStatType.HpRegenPerKill: bonusHpRegenPerKill += v; break;
            case UpgradeStatType.ShieldRegenSpeed: bonusShieldRegenSpeed *= v; break; 
            case UpgradeStatType.CritChancePercent: bonusCritChancePercent += v; break;
            case UpgradeStatType.RifleDamagePercent: bonusRifleDamagePercent += v; break;
            case UpgradeStatType.ShotgunCountBullet: bonusShotgunCountBullet += Mathf.RoundToInt(v); break;
            case UpgradeStatType.ShotgunSpread: bonusShotgunSpread += v; break;
            case UpgradeStatType.Penetration: bonusPenetration = v > 0; break;
            case UpgradeStatType.BulletSpeedPercent: bonusBulletSpeedPercent += v; break;
            case UpgradeStatType.MinigunWarmupSpeed: bonusMinigunWarmupSpeed += v; break;
            case UpgradeStatType.ExplosionChance: bonusExplosionChance += v; break;
            case UpgradeStatType.Reflect: bonusReflect = v > 0; break;
            case UpgradeStatType.BurnChance: bonusBurnChance += v; break;
            case UpgradeStatType.FreezeChance: bonusFreezeChance += v; break;
            case UpgradeStatType.P1_BerserkMode: skill_P1_BerserkMode = v > 0; break;
            case UpgradeStatType.P1_IronSkin: skill_P1_IronSkin = v > 0; break;
            case UpgradeStatType.P2_ShadowStep: skill_P2_ShadowStep = v > 0; break;
            case UpgradeStatType.P2_TwinShot: skill_P2_TwinShotChance += v; break;
        }
    }

    public void ApplyBonusesToPlayer(PlayerStats ps)
    {
        if (ps == null) return;
        ps.ApplyUpgradeBonuses(bonusMaxHp, bonusShieldMax, bonusShieldRegenSpeed);
        ps.GetComponent<PlayerController>().Apply(bonusSpeedPercent);
    }

    private List<DataBase.Item> ConvertCost(List<ItemCost> costs)
    {
        var r = new List<DataBase.Item>();
        if (costs == null) return r;
        foreach (var c in costs)
        {
            var item = new DataBase.Item();
            item.nameItem = c.itemName;
            item.countItem = c.count;
            r.Add(item);
        }
        return r;
    }
    private PlayerStats GetPlayerStats() =>
        Player.playerObject != null ? Player.playerObject.GetComponent<PlayerStats>() : null;
}

[System.Serializable]
public class UpgradeSaveEntry
{
    public string nodeId;
    public int level;
}