using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InventoryItemType selectedItem = GameManager.Instance.GetSelectedItemType();

            if (selectedItem == InventoryItemType.Dagger)
            {
                MeleeAttack();
            }
            else if (selectedItem == InventoryItemType.Bow)
            {
                RangedAttack();
            }
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Melee attack triggered (dagger).");
        // Add dagger animation or effects if desired
    }

    private void RangedAttack()
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            rb.velocity = dir * arrowSpeed;
        }
    }
}

