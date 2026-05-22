using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalChest : MonoBehaviour
{
    [Header("UI")]
    public GameObject chestWindow;
    public Transform weaponGrid;
    public GameObject weaponCardPrefab;

    private bool isOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>();

    private void Start()
    {
        chestWindow.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isOpen) CloseChest();
            else OpenChest();
        }
    }

    public void OpenChest()
    {
        isOpen = true;
        chestWindow.SetActive(true);
        Time.timeScale = 0f;
        BuildCards();
    }

    public void CloseChest()
    {
        isOpen = false;
        chestWindow.SetActive(false);
        Time.timeScale = 1f;
        ClearCards();
    }

    private void BuildCards()
    {
        ClearCards();
        if (WeaponStorage.InstanceWeaponStorage == null) return;

        var names = WeaponStorage.InstanceWeaponStorage.WeaponNames;
        if (names.Count == 0)
        {
            var empty = Instantiate(weaponCardPrefab, weaponGrid);
            empty.GetComponent<ArsenalWeaponCard>()?.ShowEmpty();
            spawnedCards.Add(empty);
            return;
        }

        foreach (var wName in names)
        {
            GameObject prefab = DataBase.GetWeapon(wName);
            if (prefab == null) continue;

            var card = Instantiate(weaponCardPrefab, weaponGrid);
            var cardScript = card.GetComponent<ArsenalWeaponCard>();
            if (cardScript != null)
            {
                cardScript.Init(prefab.GetComponentInChildren<WeaponMain>(), this);
            }
            spawnedCards.Add(card);
        }
    }

    private void ClearCards()
    {
        foreach (var c in spawnedCards)
            if (c != null) Destroy(c);
        spawnedCards.Clear();
    }

    public void TakeWeapon(WeaponMain weapon)
    {
        var hand = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Hand>();
        if (hand == null) return;
        WeaponStorage.InstanceWeaponStorage.TakeWeapon(weapon.weaponName);
        WeaponStorage.InstanceWeaponStorage.AddWeapon(hand.GetNameActiveWeapon());
        int slot = hand.activeWeapon;
        GameObject weaponPrefab = DataBase.GetWeapon(weapon.weaponName);
        if (weaponPrefab != null) hand.CreateWeaponForSave(weaponPrefab, slot);
        BuildCards();
        Debug.Log($"[Arsenal] Взято: {weapon.weaponName} в слот {slot}");
    }
}