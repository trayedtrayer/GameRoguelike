using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public class Upgrade
    {
        //0-timeDelayStartShootMin, 1-timeDelayStartShootMax, 2-timeDelayShot, 3-spread, 4-timeDelaySpray, 5-countBullet
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
        public Upgrade ReturnUpgradeByClass(int _type, WeaponMain _weapon)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            float multiplier = 0;
            multiplier = chance > 90 ? 4 : 0;
            multiplier = chance < 90 && chance >= 70 ? 2.51f : multiplier;
            multiplier = chance < 70 && chance >= 40 ? 1.51f : multiplier;
            multiplier = chance < 40 ? 0.51f : multiplier;
            for (int i = 0; i < Mathf.Round(multiplier); i++)
            {
                DataBase.Item item = DataBase.ReturnRandomItem((DataBase.Item.classResource)i);
                item.countItem = 2 * (int)Mathf.Round(multiplier - i);
                print(item);
                items.Add(item);
            }
            float j = 0.5f * multiplier;
            money = UnityEngine.Random.Range(Mathf.CeilToInt(50 * multiplier) - 50, Mathf.CeilToInt(50 * multiplier) + 50);
            if (money <= 0)
            {
                money = 35;
            }
            color = DataBase.colors[type];
            Upgrade upgrade = new Upgrade();
            upgrade.damage = Mathf.CeilToInt(_weapon.damageBullet * multiplier);
            if (type == 0)
            {
                bulletPower = (int)Mathf.Round(_weapon.forceBullet * (1 + (0.15f * j)));
                timeDelayShot = _weapon.timeDelayShot * (1f - 0.05f * multiplier);
            }
            if (type == 1)
            {
                bulletPower = (int)Mathf.Round(_weapon.forceBullet * (1 + (0.15f * j)));
                timeDelayShot = _weapon.timeDelayShot * (1f - 0.05f * multiplier);
                if (UnityEngine.Random.Range(0, 100) > 80 && Mathf.Round(multiplier) == 4)
                {
                    spread += 1;
                }
                if (UnityEngine.Random.Range(0, 100) > 70 && Mathf.Round(multiplier) == 3 || Mathf.Round(multiplier) == 3)
                {
                    countBullet += 1;
                }
            }
            if (type == 2)
            {
                bulletPower = (int)Mathf.Round(_weapon.forceBullet * (1 + (0.15f * j)));
                timeDelayShot = _weapon.timeDelayShot * (1f - 0.05f * multiplier);
                if (UnityEngine.Random.Range(0, 100) > 80 && Mathf.Round(multiplier) == 4)
                {
                    spread += 1;
                }
            }
            if (type == 3)
            {
                bulletPower = (int)Mathf.Round(_weapon.forceBullet * (1 + (0.15f * j)));
                timeDelayStartShootMin = _weapon.timeDelayShot - 0.15f * multiplier;
                timeDelayStartShootMax = _weapon.timeDelayShot - 0.05f * multiplier; ;
            }
            upgrade.bulletPower = bulletPower;
            upgrade.timeDelayShot = timeDelayShot;
            upgrade.spread = spread;
            upgrade.timeDelaySpray = timeDelaySpray;
            upgrade.timeDelayStartShootMin = timeDelayStartShootMin;
            upgrade.timeDelayStartShootMax = timeDelayStartShootMax;
            upgrade.type = _type;
            upgrade.weapon = _weapon;
            upgrade.color = DataBase.colors[(int)Mathf.Round(multiplier) - 1];
            upgrade.money = money;
            upgrade.items = items;
            return upgrade;
        }
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
            Upgrade upgrade = new Upgrade();
            int type = UnityEngine.Random.Range(0, 4);
            upgrade = upgrade.ReturnUpgradeByClass(type, weapon.GetComponentInChildren<WeaponMain>());
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
