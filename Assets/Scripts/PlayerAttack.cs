using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float meleeRange = 1.0f;
    public int daggerDamage = 4;
    public int bowDamage = 3;
    public float attackCooldown = 0.5f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private float lastAttackTime;
    private InventoryBar inventoryBar;

    private void Start()
    {
        inventoryBar = FindObjectOfType<InventoryBar>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            UseCurrentWeapon();
        }
    }

    void UseCurrentWeapon()
    {
        var selected = inventoryBar.GetSelectedItemType();

        switch (selected)
        {
            case InventoryItemType.Dagger:
                PerformMeleeAttack();
                break;

            case InventoryItemType.Bow:
                ShootArrow();
                break;

            default:
                Debug.Log("No usable weapon in selected slot.");
                break;
        }
    }

    void PerformMeleeAttack()
    {
        Debug.Log("Stab!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(daggerDamage);
                }
            }
        }
    }

    void ShootArrow()
    {
        Debug.Log("Shoot!");

        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            rb.velocity = arrowSpawnPoint.right * 10f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
        }
    }
}
