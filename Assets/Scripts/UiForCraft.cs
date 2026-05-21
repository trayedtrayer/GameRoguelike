using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Crafting;

public class UiForCraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Upgrade upgrade;
    public Crafting crafter;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI shortText;
    [SerializeField] private Image iconWeapon;
    private string description;
    public void Init(Upgrade up, Crafting craft)
    {
        upgrade = up;
        crafter = craft;
        RebuildCard();
    }

    private void Start()
    {
        if (upgrade != null)
            RebuildCard();
    }

    private void RebuildCard()
    {
        if (upgrade == null || upgrade.weapon == null) return;

        if (titleText != null)
            titleText.text = upgrade.weapon.weaponName;

        if (iconWeapon != null)
        {
            var sr = upgrade.weapon.GetComponent<SpriteRenderer>();
            if (sr != null) iconWeapon.sprite = sr.sprite;
        }

        BuildTexts(out string shortLine, out string fullText);

        if (shortText != null)
            shortText.text = shortLine;

        description = fullText;
    }

    private void BuildTexts(out string shortLine, out string fullText)
    {
        var w = upgrade.weapon;

        var sbShort = new StringBuilder();
        var sbFull = new StringBuilder();

        if (upgrade.damage != 0)
        {
            sbShort.Append("+Урон ");
            sbFull.AppendLine($"Урон: {w.damageBullet} → {upgrade.damage}");
        }

        if (upgrade.timeDelayShot > 0)
        {
            sbShort.Append("Скоростр. ");
            sbFull.AppendLine($"Задержка: {w.timeDelayShot:0.00} → {upgrade.timeDelayShot:0.00}");
        }

        if (upgrade.bulletPower != 0)
        {
            sbShort.Append("+Сила ");
            sbFull.AppendLine($"Сила пули: {w.forceBullet:0} → {upgrade.bulletPower}");
        }

        if (upgrade.spread > 0)
        {
            sbShort.Append("+Разброс ");
            sbFull.AppendLine($"Разброс: +{upgrade.spread:0.0}");
        }

        if (upgrade.countBullet > 0)
        {
            sbShort.Append("+Пули ");
            sbFull.AppendLine($"Пуль: +{upgrade.countBullet}");
        }

        if (upgrade.timeDelayStartShootMin != 0 || upgrade.timeDelayStartShootMax != 0)
        {
            sbShort.Append("Тепло ");
            sbFull.AppendLine($"Прогрев: {w.timeDelayStartShootMin:0.00}/{w.timeDelayStartShootMax:0.00} → {upgrade.timeDelayStartShootMin:0.00}/{upgrade.timeDelayStartShootMax:0.00}");
        }

        if (sbShort.Length == 0) sbShort.Append("Улучшение");
        if (sbFull.Length == 0) sbFull.Append("Улучшение параметров оружия");

        shortLine = sbShort.ToString().Trim();
        fullText = sbFull.ToString().Trim();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (crafter == null || upgrade == null) return;
        crafter.ShowName(upgrade.weapon.weaponName, upgrade.money, upgrade.items);
        crafter.ShowDescription(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (crafter == null) return;
        crafter.StopShowName();
        crafter.StopShowDescription();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (upgrade == null) return;

        var ps = Player.playerObject.GetComponent<PlayerStats>();
        if (ps != null && ps.CheckForMoneyItems(upgrade.items, upgrade.money))
        {
            upgrade.weapon.WeaponUpgrade(upgrade);
            crafter.Reopen();
        }
    }
}