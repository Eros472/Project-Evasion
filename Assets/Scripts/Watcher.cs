using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour
{
    [Tooltip("If true, Watcher will patrol vertically instead of horizontally.")]
    public bool patrolsVertically = false;

    [Tooltip("LayerMask for wall/obstacle detection")]
    public LayerMask Blocking;

    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float patrolDistance = 1f;
    public float viewRadius = 5f;
    public float alertDuration = 4f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;

    public bool showFOV = true;
    public LineRenderer fovRenderer;
    public int fovResolution = 20;

    private Vector3 initialPosition;
    private Vector3 leftPoint;
    private Vector3 rightPoint;
    private Vector3 targetPoint;
    private bool movingRight = true;
    private float returnToIdleTimer = 0f;
    private bool returningToIdle = false;

    private int alertFrameIndex = 0;
    private float frameSwitchTimer = 0f;

    private Animator animator;
    private Transform playerTarget;

    private bool isAlerted = false;
    private float alertTimer = 0f;
    private bool isPausedAtEdge = false;
    private float patrolPauseTimer = 0f;
    private float patrolPauseDuration = 1f;
    private bool isChasing = false;

    // 🛡️ New: cooldown system
    private float attackCooldown = 1.0f;
    private float lastAttackTime = -999f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        if (patrolsVertically)
        {
            leftPoint = initialPosition + Vector3.down * patrolDistance;
            rightPoint = initialPosition + Vector3.up * patrolDistance;
        }
        else
        {
            leftPoint = initialPosition + Vector3.left * patrolDistance;
            rightPoint = initialPosition + Vector3.right * patrolDistance;
        }

        targetPoint = rightPoint;

        if (showFOV && fovRenderer != null)
        {
            fovRenderer.positionCount = fovResolution + 1;
        }

        animator.SetBool("IsAlerted", false);
        animator.SetBool("IsChasing", false);
    }

    private void Update()
    {
        UpdatePatrolPoints();

        if (returningToIdle)
        {
            returnToIdleTimer += Time.deltaTime;
            if (returnToIdleTimer >= 2f)
            {
                returningToIdle = false;
                returnToIdleTimer = 0f;
                animator.Play("Idle");
            }
        }

        if (isChasing)
        {
            ChasePlayer();
            return;
        }

        if (isAlerted)
        {
            alertTimer += Time.deltaTime;
            frameSwitchTimer += Time.deltaTime;

            if (frameSwitchTimer >= 1.0f && alertFrameIndex < 4)
            {
                frameSwitchTimer = 0f;
                alertFrameIndex++;
                animator.SetInteger("AlertFrame", alertFrameIndex);
            }

            if (!PlayerStillInView())
            {
                ResetAlertState();
            }
            else if (alertTimer >= alertDuration)
            {
                isChasing = true;
                animator.SetBool("IsChasing", true);
            }
        }
        else
        {
            Patrol();

            if (DetectPlayer())
            {
                isAlerted = true;
                alertTimer = 0f;
                alertFrameIndex = 0;
                frameSwitchTimer = 0f;
                animator.SetBool("IsAlerted", true);
                animator.SetInteger("AlertFrame", 0);
            }
        }

        if (showFOV && fovRenderer != null)
        {
            DrawFieldOfView();
        }
    }

    private void Patrol()
    {
        if (isPausedAtEdge)
        {
            patrolPauseTimer += Time.deltaTime;
            if (patrolPauseTimer >= patrolPauseDuration)
            {
                isPausedAtEdge = false;
                patrolPauseTimer = 0f;

                movingRight = !movingRight;
                targetPoint = movingRight ? rightPoint : leftPoint;
                Vector3 localScale = transform.localScale;
                localScale.x = movingRight ? 1 : -1;
                transform.localScale = localScale;
            }
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
        {
            isPausedAtEdge = true;

            if (!patrolsVertically)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = movingRight ? 1 : -1;
                transform.localScale = localScale;
            }
        }
    }

    private void ChasePlayer()
    {
        if (playerTarget == null || !PlayerStillInView())
        {
            isChasing = false;
            animator.SetBool("IsChasing", false);
            ResetAlertState();
            returningToIdle = true;
            returnToIdleTimer = 0f;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, chaseSpeed * Time.deltaTime);

        Vector3 direction = playerTarget.position - transform.position;
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);

        if (Vector3.Distance(transform.position, playerTarget.position) < 0.5f && Time.time - lastAttackTime >= attackCooldown)
        {
            Player player = playerTarget.GetComponent<Player>()
                ?? playerTarget.GetComponentInParent<Player>()
                ?? playerTarget.GetComponentInChildren<Player>();

            if (player != null)
            {
                Debug.Log("[Watcher] Attacking player — applying damage.");
                player.TakeDamage(5);
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.LogWarning("[Watcher] Player script not found on target.");
            }
        }
    }

    private bool DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerTarget = hit.transform;
                return true;
            }
        }
        return false;
    }

    private bool PlayerStillInView()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerTarget = hit.transform;
                return true;
            }
        }
        return false;
    }

    private void ResetAlertState()
    {
        isAlerted = false;
        alertTimer = 0f;
        alertFrameIndex = 0;
        frameSwitchTimer = 0f;
        animator.SetBool("IsAlerted", false);
        animator.SetInteger("AlertFrame", 0);
    }

    private void DrawFieldOfView()
    {
        if (fovRenderer == null) return;

        if (fovRenderer.positionCount != fovResolution + 1)
        {
            fovRenderer.positionCount = fovResolution + 1;
        }

        float angleStep = 90f / fovResolution;
        float startingAngle = -45f;

        for (int i = 0; i <= fovResolution; i++)
        {
            float angle = startingAngle + angleStep * i;
            Vector3 dir = DirFromAngle(angle, true);
            Vector3 pos = transform.position + dir * viewRadius;

            fovRenderer.SetPosition(i, pos);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }

        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.left * patrolDistance, transform.position + Vector3.right * patrolDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & Blocking) != 0)
        {
            isPausedAtEdge = true;
            patrolPauseTimer = 0f;

            if (patrolsVertically)
            {
                Vector3 localScale = transform.localScale;
                localScale.y = movingRight ? 1 : -1;
                transform.localScale = localScale;
            }
            else
            {
                Vector3 localScale = transform.localScale;
                localScale.x = movingRight ? 1 : -1;
                transform.localScale = localScale;
            }
        }
    }

    private void UpdatePatrolPoints()
    {
        if (patrolsVertically)
        {
            leftPoint = initialPosition + Vector3.down * patrolDistance;
            rightPoint = initialPosition + Vector3.up * patrolDistance;
        }
        else
        {
            leftPoint = initialPosition + Vector3.left * patrolDistance;
            rightPoint = initialPosition + Vector3.right * patrolDistance;
        }

        if (!isChasing && !isAlerted)
        {
            targetPoint = movingRight ? rightPoint : leftPoint;
        }
    }
}
