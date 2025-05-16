using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None = -1,   // 👈 Add this
    Dagger = 0,
    Bow = 1
}


public class PlayerInventory : MonoBehaviour
{
    public int currentWeaponIndex = -1;
    public List<WeaponType> weapons = new List<WeaponType>();

    public bool weaponEquipped = false;
    public int currentDamage = 0;

    public InventoryBar inventoryBar;
    public GameObject daggerObject;
    public GameObject bowObject;

    private Player player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        player = FindObjectOfType<Player>();

        if (UIManager.Instance != null)
            inventoryBar = UIManager.Instance.inventoryBar;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestorePlayerInventory(this);

            if (GameManager.Instance.savedEquippedState &&
                GameManager.Instance.savedEquippedIndex >= 0 &&
                GameManager.Instance.savedEquippedIndex < weapons.Count)
            {
                currentWeaponIndex = GameManager.Instance.savedEquippedIndex;
                weaponEquipped = true;
                EquipCurrentWeapon();
                inventoryBar?.SelectSlot(currentWeaponIndex);
            }
        }

        HideAllWeapons(); // Always start clean
    }

    void Update()
    {
        if (inventoryBar == null && UIManager.Instance != null)
            inventoryBar = UIManager.Instance.inventoryBar;

        if (inventoryBar == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleWeaponSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ToggleWeaponSlot(1);

        if (Input.GetKeyDown(KeyCode.Space))
            UseCurrentWeapon();
    }

    private void ToggleWeaponSlot(int slot)
    {
        if (slot >= weapons.Count)
        {
            Debug.Log("Attempted to select empty weapon slot.");
            return;
        }

        if (currentWeaponIndex == slot && weaponEquipped)
        {
            Debug.Log("De-equipping weapon from slot " + slot);
            weaponEquipped = false;
            currentWeaponIndex = -1;

            HideAllWeapons();
            inventoryBar.ClearSelectedSlot();

            if (player != null)
            {
                player.EquipWeapon(WeaponType.None);
                Debug.Log("Player weapon should now be empty: " + player.currentWeapon);
            }
        }
        else
        {
            currentWeaponIndex = slot;
            weaponEquipped = true;
            EquipCurrentWeapon();
            inventoryBar.SelectSlot(slot);
            Debug.Log("Equipped weapon from slot " + slot);
        }
    }

    public void EquipCurrentWeapon()
    {
        if (!weaponEquipped)
        {
            Debug.LogWarning("[PlayerInventory] Ignoring EquipCurrentWeapon() — weaponEquipped is false.");
            return;
        }

        if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Count)
        {
            Debug.LogWarning("[PlayerInventory] Invalid index.");
            return;
        }

        WeaponType weapon = weapons[currentWeaponIndex];

        HideAllWeapons();

        if (weapon == WeaponType.Dagger && daggerObject != null)
            daggerObject.SetActive(true);
        else if (weapon == WeaponType.Bow && bowObject != null)
            bowObject.SetActive(true);

        currentDamage = 0;
        WeaponItem item = (weapon == WeaponType.Dagger ? daggerObject : bowObject)?.GetComponent<WeaponItem>();
        if (item != null) currentDamage = item.damage;

        if (player != null)
            player.EquipWeapon(weapon);

        Debug.Log("Equipped: " + weapon);
    }

    private void HideAllWeapons()
    {
        if (daggerObject != null) daggerObject.SetActive(false);
        if (bowObject != null) bowObject.SetActive(false);
    }

    private void UseCurrentWeapon()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Count) return;

        WeaponType weapon = weapons[currentWeaponIndex];

        switch (weapon)
        {
            case WeaponType.Dagger:
                Debug.Log("Stabbing with dagger!");
                break;
            case WeaponType.Bow:
                Debug.Log("Using bow!");
                break;
        }
    }

    public void EquipWeaponFromPickup(WeaponType type)
    {
        if (!weapons.Contains(type))
        {
            weapons.Add(type);

            int slot = weapons.IndexOf(type);
            inventoryBar?.AddWeaponIcon(slot, type);
            Debug.Log("Set icon for slot " + slot + ": " + type);
        }

        int equippedSlot = weapons.IndexOf(type);
        currentWeaponIndex = equippedSlot;
        weaponEquipped = true;

        EquipCurrentWeapon();
        inventoryBar?.SelectSlot(equippedSlot);

        Debug.Log("Picked up and equipped: " + type);
    }

    public bool HasWeapon(WeaponType weapon)
    {
        return weapons.Contains(weapon);
    }

    public int GetWeaponCount()
    {
        return weapons.Count;
    }
}
