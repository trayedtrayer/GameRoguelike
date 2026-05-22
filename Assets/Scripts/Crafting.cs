using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    public class CraftRecipe
    {
        public WeaponMain weapon;
        public int money;
        public List<DataBase.Item> items = new List<DataBase.Item>();
        public Color rarityColor;
        public int rarityIndex;
        public string description;
        public GameObject instance;
    }

    [Header("Окно крафта")]
    public GameObject mainWindow;

    [Header("Сетка рецептов")]
    public Transform recipeGrid;
    public GameObject recipeCardPrefab;

    [Header("Панель деталей")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailWeaponName;
    public TextMeshProUGUI detailStats;
    public TextMeshProUGUI detailCost;
    public TextMeshProUGUI detailRarity;
    public Image detailWeaponIcon;
    public Button craftButton;
    public TextMeshProUGUI craftButtonText;

    [Header("Инвентарь (для показа стоимости)")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI silverText;

    private List<CraftRecipe> recipes = new List<CraftRecipe>();
    private CraftRecipe selected;
    private PlayerStats ps;
    private bool isOpen = false;
    private List<GameObject> cards = new List<GameObject>();
    bool isGenerated;

    private void Awake()
    {
        DataBase.SetWeapons();
    }

    private void Start()
    {
        ps = Player.playerObject?.GetComponent<PlayerStats>();
        Close();
        GenerateRecipes();
        craftButton.onClick.AddListener(OnCraftClicked);
    }

    private void Update()
    {
        if (!isOpen && Input.GetKeyDown(KeyCode.K))
            Open();
        else if (isOpen && Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.K))
            Close();
    }

    private void GenerateRecipes()
    {
        if (!isGenerated)
        {
            recipes.Clear();
            foreach (var go in DataBase.weapons)
            {
                if (!WeaponStorage.InstanceWeaponStorage.HasWeapon(go.GetComponentInChildren<WeaponMain>().weaponName))
                {
                    var wm = go.GetComponentInChildren<WeaponMain>();
                    if (wm == null) continue;

                    CraftUpgradeData data = CraftSystem.GenerateUpgrade(wm);
                    var r = new CraftRecipe
                    {
                        weapon = data.weapon,
                        money = data.money,
                        items = data.requiredItems,
                        rarityColor = data.rarityColor,
                        rarityIndex = data.rarityIndex,
                        description = BuildDescription(wm)
                    };
                    recipes.Add(r);
                }
            }
            BuildCards();
            isGenerated = true;
        }
        else
        {

        }
    }

    private void BuildCards()
    {
        foreach (var c in cards) if (c != null) Destroy(c);
        cards.Clear();

        foreach (var recipe in recipes)
        {
            var go = Instantiate(recipeCardPrefab, recipeGrid);
            var ui = go.GetComponent<UiForCraft>();

            if (ui != null)
            {
                ui.Init(recipe);
                var capturedRecipe = recipe;
                capturedRecipe.instance = go.gameObject;
                ui.onCardClicked = () => SelectRecipe(capturedRecipe);
                ui.onCardHoverEnter = () =>
                {
                    selected = capturedRecipe;
                    detailPanel.SetActive(true);
                    RefreshDetailPanel();
                };
                ui.onCardHoverExit = () =>
                {

                };
            }
            var img = go.GetComponent<Image>();
            if (img != null) img.color = recipe.rarityColor;

            cards.Add(go);
        }
    }

    public void SelectRecipe(CraftRecipe recipe)
    {
        selected = recipe;
        detailPanel.SetActive(true);
        RefreshDetailPanel();
    }

    private void RefreshDetailPanel()
    {
        if (selected == null) return;

        var w = selected.weapon;

        detailWeaponName.text = w.weaponName;
        if (detailWeaponIcon != null && w.spriteWeapon != null)
            detailWeaponIcon.sprite = w.spriteWeapon;

        detailStats.text = BuildDescription(w);
        detailCost.text = BuildCostString(selected);

        bool canCraft = CanCraft(selected);
        craftButton.interactable = canCraft;
        craftButtonText.text = canCraft ? "Скрафтить" : "Не хватает ресурсов";
    }

    private void OnCraftClicked()
    {
        if (selected == null || ps == null) return;
        if (!CanCraft(selected)) return;
        ps.SpendItems(selected.items);
        ps.money -= selected.money;
        WeaponStorage.InstanceWeaponStorage?.AddWeapon(selected.weapon.weaponName);
        RefreshMoneyUI();
        RefreshDetailPanel();
        Destroy(selected.instance);
        Debug.Log($"[Crafting] Скрафтено: {selected.weapon.weaponName}");
    }

    private bool CanCraft(CraftRecipe r)
    {
        if (ps == null) return false;
        return ps.CheckForMoneyItems(r.items, r.money);
    }

    public void Open()
    {
        isOpen = true;
        mainWindow.transform.parent.gameObject.SetActive(true);
        mainWindow.SetActive(true);
        detailPanel.SetActive(false);
        selected = null;
        ps = Player.playerObject?.GetComponent<PlayerStats>();
        RefreshMoneyUI();
        GenerateRecipes();
        Time.timeScale = 0f;
    }

    public void Close()
    {
        isOpen = false;
        mainWindow.SetActive(false);
        mainWindow.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Reopen()
    {
        Close();
        Open();
    }

    private void RefreshMoneyUI()
    {
        if (ps == null) return;
        int[] golds = PlayerStats.ReturnSilverGold(ps.money);
        if (goldText != null) goldText.text = "" + golds[0];
        if (silverText != null) silverText.text = "" + golds[1];
    }

    private string BuildDescription(WeaponMain w)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Урон:       {w.damageBullet}");
        sb.AppendLine($"Пуль:       {w.countBullet}");
        sb.AppendLine($"Разброс:    {w.spread:0.0}");
        sb.AppendLine($"Задержка:   {w.timeDelayShot:0.000}");
        sb.AppendLine($"Скорость:   {w.forceBullet:0}");
        if (w.timeDelayStartShootMax > 0)
            sb.AppendLine($"Прогрев:    {w.timeDelayStartShootMin:0.0}–{w.timeDelayStartShootMax:0.0}");
        if (w.lvlWeapon > 0) sb.AppendLine($"Уровень:    {w.lvlWeapon}");
        return sb.ToString().TrimEnd();
    }

    private string BuildCostString(CraftRecipe r)
    {
        var sb = new System.Text.StringBuilder();
        if (r.money > 0)
        {
            int[] golds = PlayerStats.ReturnSilverGold(r.money);
            sb.AppendLine($"Золото: {golds[0]}  Серебро: {golds[1]}");
        }
        if (r.items != null)
            foreach (var item in r.items)
                sb.AppendLine($"{item.nameItem}  ×{item.countItem}");
        return sb.ToString().TrimEnd();
    }
}