using UnityEngine;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour
{
    public Image[] slotImages;
    public Text[] quantityTexts;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);

        if (Input.GetKeyDown(KeyCode.Space)) UseSelectedItem();
    }

    private void SelectSlot(int index)
    {
        GameManager.Instance.SetSelectedSlot(index);
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = (i == GameManager.Instance.GetSelectedSlot()) ? selectedColor : normalColor;

            var slot = GameManager.Instance.GetInventorySlot(i);
            quantityTexts[i].text = (slot.IsStackable && slot.quantity > 0)
                ? slot.quantity.ToString() : "";
        }
    }

    public void PickupItem(InventoryItemType itemType, int amount)
    {
        var inventory = GameManager.Instance.inventory;

        if (itemType == InventoryItemType.HealthPack)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].itemType == InventoryItemType.HealthPack && inventory[i].quantity < 10)
                {
                    inventory[i].quantity = Mathf.Min(inventory[i].quantity + amount, 10);
                    UpdateUI();
                    return;
                }
            }
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].itemType == InventoryItemType.None)
            {
                inventory[i].itemType = itemType;
                inventory[i].quantity = amount;
                UpdateUI();
                return;
            }
        }

        Debug.Log("No space to add item: " + itemType);
    }

    private void UseSelectedItem()
    {
        int index = GameManager.Instance.GetSelectedSlot();
        var slot = GameManager.Instance.GetInventorySlot(index);

        if (slot.itemType == InventoryItemType.HealthPack && slot.quantity > 0)
        {
            Player player = FindObjectOfType<Player>();
            player.Heal(2);

            slot.quantity--;
            if (slot.quantity <= 0)
                slot.itemType = InventoryItemType.None;

            UpdateUI();
        }
        else if (slot.itemType == InventoryItemType.Key)
        {
            Door door = FindObjectOfType<Door>();
            if (door != null && door.IsPlayerInRange())
            {
                door.Unlock();

                slot.quantity--;
                if (slot.quantity <= 0)
                    slot.itemType = InventoryItemType.None;

                UpdateUI();
            }
        }
    }

    public InventorySlot GetSelectedSlot() => GameManager.Instance.GetInventorySlot(GameManager.Instance.GetSelectedSlot());
    public InventoryItemType GetSelectedItemType() => GetSelectedSlot().itemType;
}
