using UnityEngine;

public enum InventoryItemType
{
    None,
    Dagger,
    Bow,
    HealthPack,
    Key
}

[System.Serializable]
public class InventorySlot
{
    public InventoryItemType itemType;
    public int quantity;

    public bool IsStackable => itemType == InventoryItemType.HealthPack;

    public InventorySlot()
    {
        itemType = InventoryItemType.None;
        quantity = 0;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxHealth = 20;
    public int currentHealth = 20;

    public InventorySlot[] inventory = new InventorySlot[4];
    public int selectedSlot = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
            inventory[i] = new InventorySlot();

        inventory[0].itemType = InventoryItemType.Dagger;
        inventory[0].quantity = 1;
    }

    public void SaveHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }

    public void SaveInventorySlot(int index, InventorySlot slot)
    {
        if (index >= 0 && index < inventory.Length)
            inventory[index] = slot;
    }

    public InventorySlot GetInventorySlot(int index)
    {
        return inventory[index];
    }

    public void SetSelectedSlot(int index)
    {
        selectedSlot = index;
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}
