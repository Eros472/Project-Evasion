using UnityEngine;

public class GoblinShooter : MonoBehaviour
{
    [Header("Detection & Combat")]
    public float detectionRadius = 5f;
    public float fireRate = 2f;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public LayerMask playerMask;

    private float fireCooldown = 0f;
    private Transform player;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (PlayerInRange())
        {
            FacePlayer(); // Keeps Goblin facing live direction

            if (fireCooldown <= 0f)
            {
                animator.SetTrigger("ShootTrigger");
                fireCooldown = fireRate;
            }
        }
    }


    /// <summary>
    /// Called by an Animation Event at the end of the Goblin_Shoot animation.
    /// </summary>
    public void FireArrow()
    {
        if (player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;

        // Get angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust 180° since arrow sprite points LEFT by default
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.Euler(0, 0, angle + 180f));

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = direction * 3f;

        Debug.Log($"Arrow fired at angle: {angle + 180f}");
    }




    private bool PlayerInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask);
        if (hit != null)
        {
            player = hit.transform;
            return true;
        }

        player = null;
        return false;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        // Flip X scale based on player's position
        scale.x = player.position.x < transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
