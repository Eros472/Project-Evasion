using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    public InventoryItemType itemType = InventoryItemType.None;
    public int amount = 1;

    private bool isPlayerInRange = false;
    private InventoryBar inventoryBar;
    private Text pickupPrompt;

    private void Start()
    {
        inventoryBar = FindObjectOfType<InventoryBar>();
        pickupPrompt = GameObject.Find("PickupPrompt")?.GetComponent<Text>();

        if (pickupPrompt != null)
            pickupPrompt.enabled = false;
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            if (pickupPrompt != null)
            {
                pickupPrompt.text = $"Press E to pick up {itemType}";
                pickupPrompt.enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (inventoryBar != null)
                {
                    inventoryBar.PickupItem(itemType, amount);
                    if (pickupPrompt != null)
                        pickupPrompt.enabled = false;

                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (pickupPrompt != null)
                pickupPrompt.enabled = false;
        }
    }
}


