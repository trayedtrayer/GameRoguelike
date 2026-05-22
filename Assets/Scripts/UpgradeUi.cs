using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Меню прокачки, открывается по Tab.
/// Три вкладки: Общие / Оружие / Специальные.
/// Создаёт кнопки для каждого узла UpgradeNodeData.
/// </summary>
public class UpgradeUI : MonoBehaviour
{
    [Header("Главное окно")]
    public GameObject mainWindow;

    [Header("Кнопки вкладок")]
    public Button tabGeneral;
    public Button tabWeapon;
    public Button tabSpecial;

    [Header("Контент вкладок (родители карточек)")]
    public Transform contentGeneral;
    public Transform contentWeapon;
    public Transform contentSpecial;

    [Header("Префаб карточки апгрейда")]
    public GameObject upgradeNodePrefab;

    [Header("UI — очки развития")]
    public TextMeshProUGUI pointsText;

    [Header("Панель деталей (справа)")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailDescription;
    public TextMeshProUGUI detailCost;
    public Button detailBuyButton;
    public TextMeshProUGUI detailBuyButtonText;
    private UpgradeBranch currentBranch = UpgradeBranch.General;

    private UpgradeNodeData selectedNode;
    private List<UpgradeNodeUI> spawnedCards = new List<UpgradeNodeUI>();

    private bool isOpen = false;

    private void Start()
    {
        mainWindow.SetActive(false);
        detailPanel.SetActive(false);

        tabGeneral.onClick.AddListener(() => SwitchBranch(UpgradeBranch.General));
        tabWeapon.onClick.AddListener(() => SwitchBranch(UpgradeBranch.Weapon));
        tabSpecial.onClick.AddListener(() => SwitchBranch(UpgradeBranch.Special));

        detailBuyButton.onClick.AddListener(OnBuyClicked);

        BuildAllCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen) CloseMenu();
            else OpenMenu();
        }
    }

    public void OpenMenu()
    {
        isOpen = true;
        mainWindow.SetActive(true);
        RefreshPoints();
        RefreshAllCards();
        SwitchBranch(currentBranch);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isOpen = false;
        mainWindow.SetActive(false);
        detailPanel.SetActive(false);
        selectedNode = null;
        Time.timeScale = 1f;
    }

    private void SwitchBranch(UpgradeBranch branch)
    {
        currentBranch = branch;

        contentGeneral.gameObject.SetActive(branch == UpgradeBranch.General);
        contentWeapon.gameObject.SetActive(branch == UpgradeBranch.Weapon);
        contentSpecial.gameObject.SetActive(branch == UpgradeBranch.Special);

        // Подсветка активной вкладки
        SetTabActive(tabGeneral, branch == UpgradeBranch.General);
        SetTabActive(tabWeapon, branch == UpgradeBranch.Weapon);
        SetTabActive(tabSpecial, branch == UpgradeBranch.Special);
    }

    private void SetTabActive(Button btn, bool active)
    {
        var colors = btn.colors;
        colors.normalColor = active ? new Color(0.3f, 0.6f, 1f) : new Color(0.2f, 0.2f, 0.2f);
        btn.colors = colors;
    }

    private void BuildAllCards()
    {
        if (UpgradeManager.Instance == null) return;
        foreach (var node in UpgradeManager.Instance.allNodes)
        {
            Transform parent = GetParentForBranch(node.branch);
            GameObject go = Instantiate(upgradeNodePrefab, parent);
            UpgradeNodeUI ui = go.GetComponent<UpgradeNodeUI>();
            ui.Init(node, this);
            spawnedCards.Add(ui);
        }
    }

    private Transform GetParentForBranch(UpgradeBranch branch) => branch switch
    {
        UpgradeBranch.General => contentGeneral,
        UpgradeBranch.Weapon => contentWeapon,
        UpgradeBranch.Special => contentSpecial,
        _ => contentGeneral
    };

    private void RefreshAllCards()
    {
        foreach (var card in spawnedCards)
            card.Refresh();
    }

    public void SelectNode(UpgradeNodeData node)
    {
        selectedNode = node;
        detailPanel.SetActive(true);
        RefreshDetailPanel();
    }

    private void RefreshDetailPanel()
    {
        if (selectedNode == null) return;

        UpgradeManager mgr = UpgradeManager.Instance;
        int currentLevel = mgr.GetCurrentLevel(selectedNode);
        int maxLevel = selectedNode.MaxLevel;

        detailName.text = selectedNode.upgradeName;

        string lvlStr = $"Уровень: {currentLevel} / {maxLevel}";
        float nextValue = currentLevel < maxLevel ? selectedNode.valuesPerLevel[currentLevel] : 0f;
        detailDescription.text = $"{selectedNode.description}\n{lvlStr}\nСледующий бонус: +{nextValue}";

        bool canBuy = currentLevel < maxLevel && mgr.IsRequirementMet(selectedNode);

        if (!canBuy || currentLevel >= maxLevel)
        {
            detailCost.text = currentLevel >= maxLevel ? "Максимальный уровень" : "Требование не выполнено";
            detailBuyButton.interactable = false;
            detailBuyButtonText.text = "Недоступно";
        }
        else
        {
            UpgradeLevelCost cost = selectedNode.levelCosts[currentLevel];
            string costStr = $"Стоимость:\n{cost.levelPoints} очк. развития";
            if (cost.items != null && cost.items.Count > 0)
            {
                foreach (var item in cost.items)
                    costStr += $"\n{item.itemName} x{item.count}";
            }
            detailCost.text = costStr;

            bool affordable = mgr.CanAfford(selectedNode);
            detailBuyButton.interactable = affordable;
            detailBuyButtonText.text = affordable ? "Улучшить" : "Не хватает ресурсов";
        }
    }

    public void OnBuyClicked()
    {
        if (selectedNode == null) return;
        bool success = UpgradeManager.Instance.TryPurchase(selectedNode);
        if (success)
        {
            RefreshPoints();
            RefreshAllCards();
            RefreshDetailPanel();
        }
    }

    public void RefreshPoints()
    {
        if (UpgradeManager.Instance != null && pointsText != null)
            pointsText.text = $"Очки развития: {UpgradeManager.Instance.developmentPoints}";
    }
}