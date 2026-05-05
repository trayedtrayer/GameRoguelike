using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Linq;

public static class DataBase
{
    public static List<GameObject> weapons = new List<GameObject>();
    public static List<GameObject> droppableObjects = new List<GameObject>();
    public static List<GameObject> players = new List<GameObject>();
    public static List<GameObject> inactivePlayer = new List<GameObject>();
    public static List<Item> itemsList = new List<Item>();
    public static List<Item> itemsSorted = new List<Item>();
    public static List<Color> colors = new List<Color>();
    [Serializable]
    public class Item
    {
        [SerializeField]
        Sprite sprite;
        public int countItem;
        public string nameItem;
        public classResource classRes;
        public enum classResource
        {
            common = 0,
            uncommon = 1,
            epic = 2,
            legendary = 3
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
        public Item CreateItem(int _count, string _name, Sprite _sprite)
        {
            Item item = new Item();
            {
                item.countItem = _count;
                item.nameItem = _name;
                item.sprite = _sprite;
            }
            return item;
        }
    }

    public static Item ReturnRandomItem(Item.classResource _class)
    {
        Debug.Log(_class.ToString());
        List<Item> items = itemsList.FindAll(x => x.classRes == _class).ToList();//0 1 2 3
        return items[UnityEngine.Random.Range(0, items.Count)];
    }

    public static Item ReturnRandomItem()
    {
        return itemsList[UnityEngine.Random.Range(0, itemsList.Count)];
    }

    public static Sprite SpriteItem(string _nameItem)
    {
        foreach (Item item in itemsList)
        {
            if (item.nameItem == _nameItem)
            {
                return item.GetSprite();
            }
        }
        return null;
    }

    public static void SetWeapons()
    {
        colors.Add(Color.grey);
        colors.Add(Color.blue);
        colors.Add(new Color(0.78f, 0f, 0.97f, 1f));
        colors.Add(Color.yellow);
        if (weapons.Count == 0)
        {
            foreach (GameObject g in Resources.LoadAll("Weapons", typeof(GameObject)))
            {
                weapons.Add(g);
            }
        }
        if (players.Count == 0)
        {
            foreach (GameObject g in Resources.LoadAll("Players", typeof(GameObject)))
            {
                players.Add(g);
            }
        }
        if (inactivePlayer.Count == 0)
        {
            foreach (GameObject g in Resources.LoadAll("InactivePlayers", typeof(GameObject)))
            {
                inactivePlayer.Add(g);
            }
        }
        if (itemsList.Count == 0)
        {
            foreach (GameObject g in Resources.LoadAll("Items", typeof(GameObject)))
            {
                itemsList.Add(g.GetComponent<ItemScript>().item);
            }
        }
        List<Item> itemsSorted = itemsList.OrderBy(x => x.classRes).ToList();
    }

    public static GameObject GetWeapon(string nameWeapon)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            WeaponMain weapon = weapons[i].GetComponentInChildren<WeaponMain>();
            if (weapon.weaponName == nameWeapon)
            {
                return weapons[i];
            }
        }
        return null;
    }

    public static GameObject GetPlayer(int _idPlayer)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<PlayerStats>().GetPlayerId() == _idPlayer)
            {
                return players[i];
            }
        }
        return null;
    }

    public static GameObject GetInactivePlayer(int _idPlayer)
    {
        for (int i = 0; i < inactivePlayer.Count; i++)
        {
            if (inactivePlayer[i].GetComponent<InactivePlayerScript>().inactivePlayerId == _idPlayer)
            {
                return inactivePlayer[i];
            }
        }
        return null;
    }
}
