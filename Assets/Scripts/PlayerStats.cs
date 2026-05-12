using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

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
    int mana;
    public int currentLvl = 0;
    public int maxMana;
    public int money;
    public bool isMortal;
    GameObject cameraFollow;
    public string namePlayer;
    public int playerId;
    public GameObject deadBody;
    [Header("UI And Other")]
    public Image manaBar;
    public Image hpBar;
    public Image expBar;
    public Image loadBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
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
    }

    private void Start()
    {
        mana = maxMana;
        hp = maxHp;
        RefreshTexts();
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
    }

    void LoadSave(XmlSaver.GameStats stats)
    {
        hp = stats.hp;
        mana = stats.mana;
        currentLvl = stats.currentLvl;
        level = stats.lvl;
        exp = stats.xp;
        expForNewLvl = stats.xpForNewLvl;
        playerId = stats.idPlayer;
        listItems = stats.items;
        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(stats.weaponOneName), 0);
        GetComponentInChildren<Hand>().CreateWeaponForSave(DataBase.GetWeapon(stats.weaponTwoName), 1);
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
        print(_playerId);
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

    public bool IsEnoughMana(int _needMana)
    {
        if (_needMana <= mana)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ManaRemove(int _manaRemove)
    {
        mana -= _manaRemove;
        RefreshTexts();
    }

    public void RemoveHp(int _hpRemove)
    {
        if (!isMortal)
        {
            hp -= _hpRemove;
            GetComponentInChildren<Animator>().Play("Hit");
            BecomeImmortal();
            Invoke("BecomeMortal", 0.1f);
            RefreshTexts();
        }
        if (hp <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
        hp = maxHp;
        mana = maxMana;
        XmlSaver.Write(this, hand);
        mana = 0;
        hp = 0;
        GameObject t = Instantiate(deadBody, transform.position, Quaternion.identity);
        cameraFollow.GetComponent<CameraFollowing>().SetObjectFollowing(t.transform);
        Destroy(gameObject);
    }

    void AddExp(int _exp)
    {
        if (exp + _exp < expForNewLvl)
        {
            print(exp + "----" + _exp);
            exp += _exp;
        }
        else
        {
            int temp = _exp - (expForNewLvl - exp);
            exp = temp;
            level += 1;
        }
    }

    void AddHp(int _hp)
    {
        if (hp + _hp >= maxHp)
        {
            hp = maxHp;
        }
        else
        {
            hp += _hp;
        }
    }

    public int GetHp()
    {
        return hp;
    }

    public int GetMana()
    {
        return mana;
    }

    public int GetXp()
    {
        return exp;
    }

    public int GetXpForNewLvl()
    {
        return expForNewLvl;
    }

    public int GetLvl()
    {
        return level;
    }

    public int GetCurrentGameLvl()
    {
        return currentLvl;
    }

    public void CompleteLevel()
    {
        currentLvl += 1;
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public void ChangePlayerId(int _playerId)
    {
        playerId = _playerId;
    }

    public void RefreshTexts()
    {
        hpText.text = hp + "/" + maxHp;
        manaText.text = mana + "/" + maxMana;
        expText.text = exp + "/" + expForNewLvl;
        manaBar.fillAmount = (float)mana / maxMana;
        hpBar.fillAmount = (float)hp / maxHp;
        expBar.fillAmount = (float)exp / expForNewLvl;
        lvlText.text = "" + level;
    }

    public void SetHandText(WeaponOnTheGround _weapon)
    {
        if (_weapon == null)
        {
            textWeapon.text = " ";
        }
        else
        {
            hand.ShowWeapon(textWeapon, _weapon);
        }

    }

    public void SetPlayerText(int _idPlayer)
    {
        if (_idPlayer == -1)
        {
            textWeapon.text = "";
        }
        else
        {
            textWeapon.text = "Press E to play for " + DataBase.GetPlayer(_idPlayer).GetComponent<PlayerStats>().namePlayer;
        }
    }

    public void AddFunds(int _exp, int _hp)
    {
        AddHp(_hp);
        AddExp(_exp);
        RefreshTexts();
    }

    public void BecomeImmortal()
    {
        isMortal = true;
    }

    public void BecomeMortal()
    {
        isMortal = false;
    }

    public void SetArrows(GameObject[] enemies)
    {
        print(enemies.Length);
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject t = Instantiate(prefabArrow, parentArrows.transform);
            t.GetComponent<ArrowScript>().enemy = enemies[i];
        }
    }

    public List<DataBase.Item> GetPlayerInventory()
    {
        return listItems;
    }

    public void AddItem(DataBase.Item _item)
    {
        bool found = false;
        for (int i = 0; i < listItems.Count; i++)
        {
            if (listItems[i].nameItem == _item.nameItem)
            {
                listItems[i].countItem += _item.countItem;
                found = true;
            }
        }
        if (!found)
        {
            listItems.Add(_item);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MagneticObjects>())
        {
            GetComponent<PlayerStats>().AddFunds(collision.GetComponent<MagneticObjects>().expAdd, collision.GetComponent<MagneticObjects>().hpAdd);
            if (collision.GetComponent<ItemScript>())
            {
                GetComponent<PlayerStats>().AddItem(collision.GetComponent<ItemScript>().item);
            }
            Destroy(collision.gameObject);
        }
    }

    public int GetMoneyCount()
    {
        return money;
    }

    public static int[] ReturnSilverGold(int money)
    {
        int[] goldSilv = new int[2];
        goldSilv[0] = money / 100;
        goldSilv[1] = money % 100;
        return goldSilv;
    }
    public int[] ReturnSilverGold()
    {
        int[] goldSilv = new int[2];
        goldSilv[0] = money / 100;
        goldSilv[1] = money % 100;
        return goldSilv;
    }

    public List<bool> PlayerBuilds()
    {
        List<bool> s = new List<bool>();
        Transform t = GameObject.Find("ListBuildings").transform;
        for (int i = 0; i < t.childCount; i++)
        {
            s.Add(t.GetChild(i).gameObject.activeSelf);
        }
        return s;
    }

    public bool CheckForMoneyItems(List<DataBase.Item> items, int _money)
    {
        List<DataBase.Item> itemsAccept = new(items);
        bool isMoney = money >= _money;
        bool accept = false;
        for (int i = 0; i < listItems.Count; i++)
        {
            for (int u = 0; u < items.Count; u++)
            {
                if ((listItems[i].nameItem == items[u].nameItem) && (listItems[i].countItem >= items[u].countItem))
                {
                    itemsAccept.Remove(items[u]);
                }
            }
        }
        return itemsAccept.Count == 0 && isMoney ? true : false;
    }
}
