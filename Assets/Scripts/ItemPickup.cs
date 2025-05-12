using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItemType itemType = InventoryItemType.None;
    public int amount = 1; // e.g. 1 for bow/key, up to 10 for health

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var bar = FindObjectOfType<InventoryBar>();
        if (bar == null) return;

        bar.PickupItem(itemType, amount);
        Destroy(gameObject);
    }
}
