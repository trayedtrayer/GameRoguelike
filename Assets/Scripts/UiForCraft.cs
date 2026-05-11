using NUnit.Framework;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiForCraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Crafting.Upgrade upgrade;
    public TextMeshProUGUI textUpgrade;
    public Crafting crafter;
    string text;

    private void Start()
    {
        SetTexts();
    }

    public void SetTexts()
    {
        int type = upgrade.type;
        if (type == 0)
        {
            text += "увеличенная сила пули и увеличенная скорость махания";
        }
        if (type == 1)
        {
            text += "увеличенная сила пули ,увеличенная скорость стрельбы";
            text += upgrade.spread > 0 ? "увеличение кол-ва волн пуль" : "";
            text += upgrade.countBullet > 0 ? "увеличение кол-ва пуль" : "";
        }
        if (type == 2)
        {
            text += "увеличенная сила пули ,увеличенная скорость стрельбы";
            text += upgrade.countBullet > 0 ? "увеличение кол-ва пуль" : "";
        }
        if (type == 3)
        {
            text += "увеличенная сила пули ,улучшение теплообмена оружия";
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        crafter.ShowName(upgrade.weapon.weaponName, upgrade.money, upgrade.items);
        crafter.ShowDescription(text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        crafter.StopShowName();
        crafter.StopShowDescription();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Player.playerObject.GetComponent<PlayerStats>().CheckForMoneyItems(upgrade.items, upgrade.money))
        {
            upgrade.weapon.WeaponUpgrade(upgrade);
            crafter.Reopen();
        }
    }
}
