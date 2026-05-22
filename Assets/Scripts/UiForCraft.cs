using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiForCraft : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Action onCardClicked;
    public Action onCardHoverEnter;
    public Action onCardHoverExit;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI shortText;
    [SerializeField] private Image iconWeapon;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onCardHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onCardHoverExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onCardClicked?.Invoke();
    }

    public void Init(Crafting.CraftRecipe recipe)
    {
        if (recipe == null || recipe.weapon == null) return;

        if (titleText != null)
            titleText.text = recipe.weapon.weaponName;

        if (iconWeapon != null)
        {
            if (recipe.weapon.spriteWeapon != null)
            {
                iconWeapon.sprite = recipe.weapon.spriteWeapon;
            }
            else
            {
                var sr = recipe.weapon.GetComponent<SpriteRenderer>();
                if (sr != null) iconWeapon.sprite = sr.sprite;
            }
        }
        if (shortText != null)
            shortText.text = BuildShortLine(recipe);
    }

    private string BuildShortLine(Crafting.CraftRecipe recipe)
    {
        var w = recipe.weapon;
        var sb = new StringBuilder();

        if (w.damageBullet > 0) sb.Append("Урон " + w.damageBullet + "\n");
        if (w.timeDelayShot > 0) sb.Append("Скоростр. " + w.timeDelayShot + "\n");
        if (w.forceBullet > 0) sb.Append("Сила " + w.forceBullet + "\n");
        if (w.spread > 0) sb.Append("Разброс " + w.spread + "\n");
        if (w.countBullet > 1) sb.Append("Пули " + w.countBullet + "\n");
        if (w.timeDelayStartShootMax > 0) sb.Append("Прогрев " + w.damageBullet + "\n");

        if (sb.Length == 0) sb.Append("Улучшение");

        return sb.ToString().Trim();
    }
}