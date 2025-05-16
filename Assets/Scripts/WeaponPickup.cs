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

            if (inventory != null && weaponData != null)
            {
                if (!inventory.HasWeapon(weaponData.weaponType))
                {
                    inventory.EquipWeaponFromPickup(weaponData.weaponType);
                }

                Destroy(gameObject);
            }
        }
    }
}
