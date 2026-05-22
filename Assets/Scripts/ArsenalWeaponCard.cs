using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArsenalWeaponCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public Image weaponIcon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponStats;
    public Button takeButton;
    public TextMeshProUGUI takeButtonText;
    public Image cardBg;
    private WeaponMain weapon;
    private ArsenalChest chest;

    public void Init(WeaponMain _weapon, ArsenalChest _chest)
    {
        weapon = _weapon;
        chest = _chest;

        if (weaponIcon != null)
            weaponIcon.sprite = _weapon.spriteWeapon != null
                ? _weapon.spriteWeapon
                : _weapon.GetComponent<SpriteRenderer>()?.sprite;

        if (weaponName != null)
            weaponName.text = _weapon.weaponName;

        if (weaponStats != null)
            weaponStats.text = BuildStatsLine(_weapon);

        if (takeButton != null)
            takeButton.onClick.AddListener(OnTakeClicked);

        if (takeButtonText != null)
            takeButtonText.text = "Взять";
    }

    public void ShowEmpty()
    {
        if (takeButton != null) takeButton.gameObject.SetActive(false);
        if (weaponIcon != null) weaponIcon.gameObject.SetActive(false);
        if (weaponName != null) weaponName.text = "Арсенал пуст";
        if (weaponStats != null) weaponStats.text = "";
    }

    private void OnTakeClicked()
    {
        if (chest != null && weapon != null)
            chest.TakeWeapon(weapon);
    }

    private string BuildStatsLine(WeaponMain w)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($"Урон: {w.damageBullet}  ");
        if (w.countBullet > 1) sb.Append($"Пуль: {w.countBullet}  ");
        if (w.spread > 0) sb.Append($"Разброс: {w.spread:0.0}  ");
        sb.Append($"Задержка: {w.timeDelayShot:0.00}");
        if (w.lvlWeapon > 0) sb.Append($"  Ур.{w.lvlWeapon}");
        return sb.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardBg != null)
        {
            var c = cardBg.color;
            cardBg.color = new Color(c.r + 0.15f, c.g + 0.15f, c.b + 0.15f, c.a);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (weapon != null && cardBg != null)
        {
            cardBg.color = Color.white;
        }
    }
}