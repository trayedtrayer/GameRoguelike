using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ChestScript : MonoBehaviour
{
    public GameObject chestGrid;
    public GameObject chestBg;
    public GameObject prefabChestUi;
    public GameObject nameWindow;
    public TextMeshProUGUI goldMoneyCount;
    public TextMeshProUGUI silverMoneyCount;
    PlayerStats playerStats;
    public void OpenChest(List<DataBase.Item> listItems)
    {
        chestBg.SetActive(true);
        StopShowName();
        for (int i = 0; i < chestGrid.transform.childCount; i++)
        {
            Destroy(chestGrid.transform.GetChild(i).gameObject);
        }
        foreach (DataBase.Item _item in listItems)
        {
            GameObject t = Instantiate(prefabChestUi, chestGrid.transform);
            t.GetComponent<Image>().sprite = _item.GetSprite();
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + _item.countItem;
            print(_item.nameItem);
            t.GetComponent<UiEnter>().nameItem = _item.nameItem;
            t.GetComponent<UiEnter>().chestScript = this;
        }
        int[] money = playerStats.ReturnSilverGold();
        goldMoneyCount.text = "" + money[0];
        silverMoneyCount.text = "" + money[1];
        nameWindow.SetActive(true);
    }
    public void CloseChest()
    {
        chestBg.SetActive(false);
    }
    public void ShowName(string name)
    {
        nameWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + name;
    }
    public void StopShowName()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            playerStats = collision.GetComponent<PlayerStats>();
            OpenChest(collision.GetComponent<PlayerStats>().listItems);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            CloseChest();
        }
    }
}
