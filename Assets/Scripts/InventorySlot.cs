[System.Serializable]
public class InventorySlot
{
    public InventoryItemType itemType = InventoryItemType.None;
    public int quantity = 0;

    public bool IsStackable =>
        itemType == InventoryItemType.HealthPack;
}

