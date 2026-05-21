using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public class Upgrade
    {
        public int damage;
        public float timeDelayStartShootMin;
        public float timeDelayStartShootMax;
        public float timeDelayShot;
        public float spread;
        public float timeDelaySpray;
        public int countBullet;
        public int bulletPower;
        public Color color;
        public int type;
        public WeaponMain weapon;
        public int money;
        public List<DataBase.Item> items = new List<DataBase.Item>();
    }

    //1 2 3 4 5 6
    List<Upgrade> upgrade;
    public GameObject parentOfCrafts;
    public GameObject prefabUiCraft;
    public GameObject mainWindow;
    public GameObject nameWindow;
    public GameObject descriptionCraft;
    public TextMeshProUGUI goldMoneyCount;
    public TextMeshProUGUI silverMoneyCount;
    public GameObject itemWindow;
    public GameObject prefabItem;
    PlayerStats playerStats;

    private void Awake()
    {
        DataBase.SetWeapons();
    }

    private void Start()
    {
        SetWeaponsToCraft();
        Close();
        playerStats = Player.playerObject.GetComponent<PlayerStats>();
    }

    public void SetWeaponsToCraft()
    {
        List<GameObject> weapons = DataBase.weapons;
        foreach (GameObject weapon in weapons)
        {
            WeaponMain weaponMain = weapon.GetComponentInChildren<WeaponMain>();
            CraftUpgradeData data = CraftSystem.GenerateUpgrade(weaponMain);
            Upgrade upgrade = new Upgrade();
            upgrade.damage = data.damage;
            upgrade.timeDelayShot = data.timeDelayShot;
            upgrade.timeDelayStartShootMin = data.timeDelayStartShootMin;
            upgrade.timeDelayStartShootMax = data.timeDelayStartShootMax;
            upgrade.spread = data.spread;
            upgrade.timeDelaySpray = data.timeDelaySpray;
            upgrade.countBullet = data.countBullet;
            upgrade.bulletPower = data.bulletPower;
            upgrade.money = data.money;
            upgrade.items = data.requiredItems;
            upgrade.weapon = data.weapon;
            upgrade.type = data.type;
            upgrade.color = data.rarityColor;
            GameObject t = Instantiate(prefabUiCraft, parentOfCrafts.transform);
            t.GetComponent<UiForCraft>().upgrade = upgrade;
            t.GetComponent<UiForCraft>().crafter = this;
            t.GetComponent<Image>().color = upgrade.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            Open();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            Close();
        }
    }

    public void Open()
    {
        for (int i = 0; i < parentOfCrafts.transform.childCount; i++)
        {
            parentOfCrafts.transform.GetChild(i).gameObject.SetActive(true);
        }
        mainWindow.SetActive(true);
    }
    public void Close()
    {
        for (int i = 0; i < parentOfCrafts.transform.childCount; i++)
        {
            parentOfCrafts.transform.GetChild(i).gameObject.SetActive(false);
        }
        mainWindow.SetActive(false);
    }

    public void Reopen()
    {
        playerStats.RespawnWeapon();
        for (int i = 0; i < parentOfCrafts.transform.childCount; i++)
        {
            Destroy(parentOfCrafts.transform.GetChild(i).gameObject);
        }
        SetWeaponsToCraft();
        Open();

    }

    public void ShowName(string _name, int money, List<DataBase.Item> items)
    {
        nameWindow.SetActive(true);
        nameWindow.GetComponentInChildren<TextMeshProUGUI>().text = _name;
        int[] _money;
        _money = PlayerStats.ReturnSilverGold(money);
        goldMoneyCount.text = "" + _money[0];
        silverMoneyCount.text = "" + _money[1];
        if (itemWindow.transform.childCount == 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                GameObject t = Instantiate(prefabItem, itemWindow.transform);
                t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = " " + items[i].countItem;
                t.GetComponent<Image>().sprite = items[i].GetSprite();
            }
        }
    }

    public void StopShowName()
    {
        nameWindow.SetActive(false);
        for (int i = 0; i < itemWindow.transform.childCount; i++)
        {
            Destroy(itemWindow.transform.GetChild(i).gameObject);
        }
    }

    public void ShowDescription(string _description)
    {
        descriptionCraft.SetActive(true);
        descriptionCraft.GetComponentInChildren<TextMeshProUGUI>().text = _description;
    }

    public void StopShowDescription()
    {
        descriptionCraft.SetActive(false);
    }
}
