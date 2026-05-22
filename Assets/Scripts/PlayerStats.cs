using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Player
{
    public static GameObject playerObject;
}

public class PlayerStats : MonoBehaviour
{
    [Header("Stats Player")]
    int exp;
    public int expForNewLvl;
    public int maxHp;
    int level = 1;
    int hp;
    float shield;
    float shieldCooldown;
    bool canBeCooldoownShield;
    public float maxShieldCooldown;
    public int currentLvl = 0;
    public float maxShield;
    public int money;
    public bool isMortal;
    public GameObject cameraFollow;
    public string namePlayer;
    public int playerId;
    public GameObject deadBody;

    [Header("Базовые значения (до прокачки)")]
    public int baseMaxHp;
    public float baseMaxShield;

    [Header("UI And Other")]
    public Image shieldBar;
    public Image shieldCooldownBar;
    public Image hpBar;
    public Image expBar;
    public Image loadBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI lvlText;
    public TextMeshProUGUI textWeapon;
    public GameObject parentArrows;
    public GameObject prefabArrow;
    public Hand hand;
    public List<DataBase.Item> listItems;

    private void Awake()
    {
        Player.playerObject = gameObject;
        listItems = new List<DataBase.Item>();
        Time.timeScale = 1;
        if (baseMaxHp == 0) baseMaxHp = maxHp;
        if (baseMaxShield == 0) baseMaxShield = maxShield;
    }

    private void Start()
    {
        shield = maxShield;
        hp = maxHp;
        RefreshTexts();
        StartCoroutine(ShieldEnum());
        XmlSaver.GameStats stats = XmlSaver.Read();
        DataBase.SetWeapons();
        if (stats != null)
        {
            LoadSave(stats);
        }
        else
        {
            GetComponentInChildren<Hand>().SetActiveWeapon(0);
        }
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.ApplyBonusesToPlayer(this);
    }

    void LoadSave(XmlSaver.GameStats stats)
    {
        hp = stats.hp;
        shield = stats.shield;
        currentLvl = stats.currentLvl;
        level = stats.lvl;
        exp = stats.xp;
        expForNewLvl = stats.xpForNewLvl;
        playerId = stats.idPlayer;
        listItems = stats.items;
        money = stats.money;

        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(stats.weaponOneName), 0);
        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(stats.weaponTwoName), 1);
        if (UpgradeManager.Instance != null && stats.upgradeLevels != null)
        {
            UpgradeManager.Instance.LoadFromSave(
                stats.developmentPoints,
                stats.upgradeLevels
            );
            UpgradeManager.Instance.ApplyBonusesToPlayer(this);
        }

        RefreshTexts();
    }

    public void ApplyUpgradeBonuses(int bonusMaxHp, float bonusShieldMax, float bonusShieldRegeneration)
    {
        maxHp = baseMaxHp + bonusMaxHp;
        maxShield = baseMaxShield + bonusShieldMax;
        shieldCooldown = shieldCooldown - bonusShieldRegeneration;
        hp = Mathf.Clamp(hp, 1, maxHp);
        shield = Mathf.Clamp(shield, 0, maxShield);
        RefreshTexts();
    }


    void AddExp(int _exp)
    {
        if (exp + _exp < expForNewLvl)
        {
            exp += _exp;
        }
        else
        {
            int temp = _exp - (expForNewLvl - exp);
            exp = temp;
            level += 1;
            expForNewLvl = Mathf.RoundToInt(expForNewLvl * 1.15f);

            if (UpgradeManager.Instance != null)
                UpgradeManager.Instance.AddDevelopmentPoint();

            Debug.Log($"[PlayerStats] Уровень {level}! Очков развития: {UpgradeManager.Instance?.developmentPoints}");
        }
    }

    public void SpendItems(List<DataBase.Item> itemsToSpend)
    {
        foreach (var cost in itemsToSpend)
        {
            for (int i = 0; i < listItems.Count; i++)
            {
                if (listItems[i].nameItem == cost.nameItem)
                {
                    listItems[i].countItem -= cost.countItem;
                    if (listItems[i].countItem <= 0)
                        listItems.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void SwapPlayer(int _playerId, Transform _placeSpawn)
    {
        GameObject _inactivePlayer = DataBase.GetInactivePlayer(playerId);
        GameObject.Find("StartLocHead").GetComponent<StartLocScript>().CheckPoses(playerId, _inactivePlayer);
        playerId = _playerId;
        XmlSaver.Write(this, hand);
        GameObject player = DataBase.GetPlayer(_playerId);
        GameObject _player = Instantiate(player, _placeSpawn.position, _placeSpawn.rotation);
        _player.GetComponent<PlayerStats>().playerId = _playerId;
        cameraFollow.GetComponent<CameraFollowing>().SetObjectFollowing(_player.transform);
        _player.GetComponent<PlayerStats>().GetCam(cameraFollow);
        Destroy(_placeSpawn.gameObject);
        GameObject.Find("StartLocHead").GetComponent<StartLocScript>().CheckPoses(_playerId, _inactivePlayer);
        Destroy(gameObject);
    }

    public void RespawnWeapon()
    {
        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(hand.GunOne.GetComponentInChildren<WeaponMain>().weaponName), 0);
        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(hand.GunTwo.GetComponentInChildren<WeaponMain>().weaponName), 1);
    }

    public void GetCam(GameObject _cam)
    {
        cameraFollow = _cam;
    }

    public void RemoveHp(int _hpRemove)
    {
        if (!isMortal)
        {
            if (shield <= 0)
            {
                hp -= _hpRemove;
                GetComponentInChildren<Animator>().Play("Hit");
                BecomeImmortal();
                Invoke("BecomeMortal", 0.1f);
                HitForShield();
                RefreshTexts();
            }
            else
            {
                RemoveShield(_hpRemove);
            }
        }
        if (hp <= 0)
        {
            Dead();
        }
    }

    void RemoveShield(int _hpRemove)
    {
        if (shield - _hpRemove < 0)
        {
            shield = 0;
            RemoveHp((int)Mathf.Abs(shield - _hpRemove));
        }
        else
        {
            shield -= _hpRemove;
        }
        HitForShield();
    }

    void Dead()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        hp = maxHp;
        shield = maxShield;
        XmlSaver.Write(this, hand);
        shield = 0;
        hp = 0;
        GameObject t = Instantiate(deadBody, transform.position, Quaternion.identity);
        cameraFollow.GetComponent<CameraFollowing>().SetObjectFollowing(t.transform);
        Destroy(gameObject);
    }

    void AddHp(int _hp)
    {
        hp = Mathf.Min(hp + _hp, maxHp);
    }

    public int GetHp() => hp;
    public float GetShield() => shield;
    public int GetXp() => exp;
    public int GetXpForNewLvl() => expForNewLvl;
    public int GetLvl() => level;
    public int GetCurrentGameLvl() => currentLvl;
    public void CompleteLevel() { currentLvl += 1; }
    public int GetPlayerId() => playerId;
    public void ChangePlayerId(int _playerId) { playerId = _playerId; }
    public void AddMoney(int _countMoney) { money += _countMoney;  }

    public void RefreshTexts()
    {
        if (hpText != null) hpText.text = hp + "/" + maxHp;
        if (shieldText != null) shieldText.text = Mathf.FloorToInt(shield) + "/" + maxShield;
        if (expText != null) expText.text = exp + "/" + expForNewLvl;
        if (shieldBar != null) shieldBar.fillAmount = shield / maxShield;
        if (shieldCooldownBar != null) shieldCooldownBar.fillAmount = 1f - (shieldCooldown / maxShieldCooldown);
        if (hpBar != null) hpBar.fillAmount = (float)hp / maxHp;
        if (expBar != null) expBar.fillAmount = (float)exp / expForNewLvl;
        if (lvlText != null) lvlText.text = "" + level;
    }

    public void SetHandText(WeaponOnTheGround _weapon)
    {
        if (_weapon == null)
            textWeapon.text = " ";
        else
            hand.ShowWeapon(textWeapon, _weapon);
    }

    public void SetPlayerText(int _idPlayer)
    {
        textWeapon.text = _idPlayer == -1 ? "" : "Press E to play for " + DataBase.GetPlayer(_idPlayer).GetComponent<PlayerStats>().namePlayer;
    }

    public void AddFunds(int _exp, int _hp, int _money)
    {
        AddHp(_hp);
        AddExp(_exp);
        AddMoney(_money);
        RefreshTexts();
    }

    public void BecomeImmortal() { isMortal = true; }
    public void BecomeMortal() { isMortal = false; }

    public void SetArrows(GameObject[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject t = Instantiate(prefabArrow, parentArrows.transform);
            t.GetComponent<ArrowScript>().enemy = enemies[i];
        }
    }

    public List<DataBase.Item> GetPlayerInventory() => listItems;

    public void AddItem(DataBase.Item _item)
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            if (listItems[i].nameItem == _item.nameItem)
            {
                listItems[i].countItem += _item.countItem;
                return;
            }
        }
        listItems.Add(_item);
    }

    public int GetMoneyCount() => money;

    public static int[] ReturnSilverGold(int money)
    {
        return new int[] { money / 100, money % 100 };
    }

    public int[] ReturnSilverGold()
    {
        return new int[] { money / 100, money % 100 };
    }

    public List<bool> PlayerBuilds()
    {
        return new List<bool>();
    }

    public bool CheckForMoneyItems(List<DataBase.Item> items, int _money)
    {
        List<DataBase.Item> itemsAccept = new List<DataBase.Item>(items);
        bool isMoney = money >= _money;

        for (int i = 0; i < listItems.Count; i++)
        {
            for (int u = 0; u < items.Count; u++)
            {
                if (listItems[i].nameItem == items[u].nameItem &&
                    listItems[i].countItem >= items[u].countItem)
                {
                    itemsAccept.Remove(items[u]);
                }
            }
        }

        return itemsAccept.Count == 0 && isMoney;
    }

    void HitForShield()
    {
        shieldCooldown = maxShieldCooldown;
        canBeCooldoownShield = true;
    }

    IEnumerator ShieldEnum()
    {
        while (true)
        {
            if (canBeCooldoownShield)
            {
                shieldCooldown -= 0.02f;
                RefreshTexts();
                yield return new WaitForSeconds(0.02f);
            }
            if (shieldCooldown <= 0)
            {
                canBeCooldoownShield = false;
                if (shield < maxShield)
                {
                    shield += 0.02f;
                    RefreshTexts();
                }
                else
                {
                    shield = maxShield;
                    RefreshTexts();
                }
                yield return new WaitForSeconds(0.02f);
            }
            yield return null;
        }
    }

    public void AddCamera(GameObject _gameObject)
    {
        cameraFollow = _gameObject;
    }
}