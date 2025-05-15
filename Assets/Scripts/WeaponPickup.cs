using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private WeaponItem weaponData;

    private void Awake()
    {
        weaponData = GetComponent<WeaponItem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            InventoryBar bar = FindObjectOfType<InventoryBar>();

            if (inventory != null && bar != null && weaponData != null)
            {
                // Prevent duplicates
                if (!inventory.weapons.Contains(weaponData.weaponType))
                {
                    inventory.weapons.Add(weaponData.weaponType);
                    int slot = inventory.GetWeaponCount() - 1;

                    bar.SetSlotSprite(slot, weaponData.icon);
                    Debug.Log("Added " + weaponData.weaponType + " to inventory.");
                }

                Destroy(gameObject);
            }
        }
    }
}
