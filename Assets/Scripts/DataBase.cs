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
}
