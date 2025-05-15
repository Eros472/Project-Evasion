using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Dagger,
    Bow
}

public class PlayerInventory : MonoBehaviour
{
    public int currentWeaponIndex = 0;
    public List<WeaponType> weapons = new List<WeaponType>();
    private bool weaponEquippedByPlayer = false;
    private bool weaponEquipped = false; // Tracks whether something is actively equipped
    private int lastSelectedSlot = -1;   // Tracks previous slot selection




    public InventoryBar inventoryBar; // Drag your UI bar GameObject in Inspector
    public GameObject daggerObject;
    public GameObject bowObject;

    void Start()
    {
        // Load saved weapons if they exist
        if (GameManager.Instance != null && GameManager.Instance.savedWeapons.Count > 0)
        {
            weapons = new List<WeaponType>(GameManager.Instance.savedWeapons);
        }

        // 🔒 Don't equip anything yet — wait for user input
        if (daggerObject != null) daggerObject.SetActive(false);
        if (bowObject != null) bowObject.SetActive(false);
    }


    void Update()
    {
        if (inventoryBar == null) return;

        int selectedSlot = inventoryBar.GetSelectedSlot();

        // Handle slot key press (1–4)
        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Toggle logic
            if (selectedSlot == lastSelectedSlot && weaponEquipped)
            {
                // De-equip
                Debug.Log("De-equipping weapon.");
                weaponEquipped = false;
                currentWeaponIndex = -1;
                HideAllWeapons();
            }
            else if (selectedSlot < weapons.Count)
            {
                // Equip new weapon
                currentWeaponIndex = selectedSlot;
                weaponEquipped = true;
                EquipCurrentWeapon();
                Debug.Log("Equipped from slot " + selectedSlot);
            }
            else
            {
                // Invalid/empty slot — de-equip
                Debug.Log("Empty slot selected. Clearing equipped weapon.");
                weaponEquipped = false;
                currentWeaponIndex = -1;
                HideAllWeapons();
            }

            lastSelectedSlot = selectedSlot;
        }

        if (Input.GetMouseButtonDown(0) && weaponEquipped)
        {
            UseCurrentWeapon();
        }
    }


    private void HideAllWeapons()
    {
        if (daggerObject != null) daggerObject.SetActive(false);
        if (bowObject != null) bowObject.SetActive(false);
    }









    /*public void AddWeapon(WeaponType newWeapon)
    {
        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            Debug.Log($"Added {newWeapon} to inventory.");

            int index = weapons.Count - 1;

            if (inventoryBar != null)
            {
                inventoryBar.AddWeaponIcon(index, newWeapon); 
            }
        }
    }*/


    public int GetWeaponCount()
    {
        return weapons.Count;
    }

    public void EquipCurrentWeapon()
    {
        if (weapons.Count == 0) return;

        WeaponType weapon = weapons[currentWeaponIndex];
        Debug.Log("Equipped: " + weapon);

        if (daggerObject != null)
        {
            bool show = (weapon == WeaponType.Dagger);
            daggerObject.SetActive(show);
            Debug.Log("Dagger visibility set to: " + show);
        }

        if (bowObject != null)
        {
            bool show = (weapon == WeaponType.Bow);
            bowObject.SetActive(show);
            Debug.Log("Bow visibility set to: " + show);
        }
    }


    private void UseCurrentWeapon()
    {
        if (weapons.Count == 0) return;

        WeaponType weapon = weapons[currentWeaponIndex];
        switch (weapon)
        {
            case WeaponType.Dagger:
                Debug.Log("Stabbing with dagger!");
                // TODO: Add dagger animation or logic
                break;
            case WeaponType.Bow:
                Debug.Log("Firing arrow!");
                // TODO: Add bow firing logic
                break;
        }
    }
}
