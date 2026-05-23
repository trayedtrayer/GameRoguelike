using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeNodeUI : MonoBehaviour, IPointerDownHandler
{
    [Header("UI элементы")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;

    private static readonly Color colorMaxed = new Color(1f, 0.85f, 0f, 1f);
    private static readonly Color colorAffordable = new Color(0.2f, 0.8f, 0.2f, 1f);
    private static readonly Color colorLocked = new Color(0.4f, 0.4f, 0.4f, 1f);
    private static readonly Color colorNormal = new Color(0.15f, 0.15f, 0.25f, 1f);

    private UpgradeNodeData node;
    private UpgradeUI upgradeUI;

    public void Init(UpgradeNodeData _node, UpgradeUI _ui)
    {
        node = _node;
        upgradeUI = _ui;
        Refresh();
    }

    public void Refresh()
    {
        if (node == null || UpgradeManager.Instance == null) return;

        int currentLevel = UpgradeManager.Instance.GetCurrentLevel(node);
        int maxLevel = node.MaxLevel;

        if (nameText != null)
            nameText.text = node.upgradeName;

        if (levelText != null)
            levelText.text = $"{currentLevel}/{maxLevel}";

        if (iconImage != null && node.icon != null)
            iconImage.sprite = node.icon;

        if (backgroundImage != null)
        {
            if (currentLevel >= maxLevel)
                backgroundImage.color = colorMaxed;
            else if (!UpgradeManager.Instance.IsRequirementMet(node))
                backgroundImage.color = colorLocked;
            else if (UpgradeManager.Instance.CanAfford(node))
                backgroundImage.color = colorAffordable;
            else
                backgroundImage.color = colorNormal;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (upgradeUI != null && node != null)
            upgradeUI.SelectNode(node);
    }
}