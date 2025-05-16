using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class HoodedGuy : MonoBehaviour
{
    public bool patrolsVertically = false;
    public float patrolDistance = 2f;
    public float moveSpeed = 2f;
    public float lookDuration = 2f;
    public float vanishDelay = 3.5f;

    public int attackDamage = 10;
    public float attackRadius = 1.5f;
    public float reappearDistance = 1.5f;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    public GameObject detectionCone;

    private PolygonCollider2D coneCollider;
    private Transform player;
    private Vector3 startPosition;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;
    private bool movingForward = true;

    private float lookTimer = 0f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 lastMoveDirection = Vector2.right;
    private Vector2 lookDirection = Vector2.right;
    private bool lookingRight = true;

    private Color originalColor;
    private float fadeDuration = 0.5f;

    private readonly Vector3 coneOffsetRight = new Vector3(0.702f, -0.012f, 0f);
    private readonly Vector3 coneOffsetLeft = new Vector3(-0.702f, -0.012f, 0f);
    private readonly Vector3 coneOffsetUp = new Vector3(-0.014f, 0.605f, 0f);
    private readonly Vector3 coneOffsetDown = new Vector3(-0.014f, -0.605f, 0f);

    private enum State { WalkPatrol, LookPatrol, Spotted }
    private State currentState = State.WalkPatrol;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        startPosition = transform.position;
        pointA = startPosition + (patrolsVertically ? Vector3.down : Vector3.left) * patrolDistance;
        pointB = startPosition + (patrolsVertically ? Vector3.up : Vector3.right) * patrolDistance;
        targetPoint = pointB;

        lastMoveDirection = (targetPoint - transform.position).normalized;
        lookDirection = lastMoveDirection;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (detectionCone != null)
        {
            detectionCone.SetActive(false);
            coneCollider = detectionCone.GetComponent<PolygonCollider2D>();
        }
    }

    void FixedUpdate()
    {
        if (currentState == State.WalkPatrol)
            WalkPatrolPhysics();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.LookPatrol:
                LookPatrol();
                break;
            case State.Spotted:
                break;
        }
    }

    void WalkPatrolPhysics()
    {
        animator.SetBool("AtPatrolPoint", false);
        animator.Play("WalkPatrol");

        Vector2 moveDir = (targetPoint - transform.position);
        if (moveDir.sqrMagnitude > 0.001f)
        {
            lastMoveDirection = moveDir.normalized;
            lookDirection = lastMoveDirection;
            spriteRenderer.flipX = patrolsVertically ? false : lastMoveDirection.x < 0;
        }

        rb.MovePosition(rb.position + lastMoveDirection * moveSpeed * Time.fixedDeltaTime);
        UpdateConeBasedOnEyeDirection();

        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            targetPoint = movingForward ? pointA : pointB;
            movingForward = !movingForward;
            currentState = State.LookPatrol;
            lookTimer = 0f;
        }
    }

    void LookPatrol()
    {
        animator.SetBool("AtPatrolPoint", true);
        animator.Play("LookPatrol");

        lookTimer += Time.deltaTime;

        if (lookTimer < lookDuration / 2f && !lookingRight)
        {
            lookingRight = true;
            lookDirection = patrolsVertically ? Vector2.up : Vector2.right;
            UpdateConeBasedOnEyeDirection();
        }
        else if (lookTimer >= lookDuration / 2f && lookingRight)
        {
            lookingRight = false;
            lookDirection = patrolsVertically ? Vector2.down : Vector2.left;
            UpdateConeBasedOnEyeDirection();
        }

        if (PlayerInSight())
        {
            currentState = State.Spotted;
            animator.SetTrigger("Disappear");
            StartCoroutine(ReappearAndStrike());
        }
        else if (lookTimer >= lookDuration)
        {
            currentState = State.WalkPatrol;
        }
    }

    bool PlayerInSight()
    {
        if (player == null || coneCollider == null)
            return false;

        return coneCollider.OverlapPoint(player.position);
    }

    IEnumerator ReappearAndStrike()
    {
        SpriteRenderer coneRenderer = detectionCone != null ? detectionCone.GetComponent<SpriteRenderer>() : null;
        yield return StartCoroutine(FadeBoth(spriteRenderer, coneRenderer, 0f));
        yield return new WaitForSeconds(vanishDelay);

        Vector2 reappearPos = FindSafePositionNearPlayer();
        transform.position = reappearPos;

        yield return StartCoroutine(FadeBoth(spriteRenderer, coneRenderer, 1f));

        animator.SetTrigger("ReappearAttack");
        yield return new WaitForSeconds(0.4f);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            Player p = hit.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(attackDamage);
                Debug.Log("[HoodedGuy] Player hit for " + attackDamage);
            }
        }

        yield return new WaitForSeconds(0.8f);
        animator.SetTrigger("ResumePatrol");
        currentState = State.WalkPatrol;
    }

    private Vector2 FindSafePositionNearPlayer()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * reappearDistance;
            Vector2 candidate = (Vector2)player.position + offset;

            if (Physics2D.OverlapCircle(candidate, 0.1f, groundLayer))
                return candidate;
        }

        return player.position;
    }

    IEnumerator FadeBoth(SpriteRenderer npcRenderer, SpriteRenderer coneRenderer, float targetAlpha)
    {
        float startAlpha = npcRenderer.color.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color npcColor = npcRenderer.color;
            npcColor.a = alpha;
            npcRenderer.color = npcColor;

            if (coneRenderer != null)
            {
                Color coneColor = coneRenderer.color;
                coneColor.a = alpha;
                coneRenderer.color = coneColor;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Color finalNpc = npcRenderer.color;
        finalNpc.a = targetAlpha;
        npcRenderer.color = finalNpc;

        if (coneRenderer != null)
        {
            Color finalCone = coneRenderer.color;
            finalCone.a = targetAlpha;
            coneRenderer.color = finalCone;
        }
    }

    private void UpdateConeBasedOnEyeDirection()
    {
        if (detectionCone == null) return;

        Vector2 clamped = ClampToCardinal(lookDirection);
        lookDirection = clamped;

        if (clamped == Vector2.right)
            detectionCone.transform.localPosition = coneOffsetRight;
        else if (clamped == Vector2.left)
            detectionCone.transform.localPosition = coneOffsetLeft;
        else if (clamped == Vector2.up)
            detectionCone.transform.localPosition = coneOffsetUp;
        else if (clamped == Vector2.down)
            detectionCone.transform.localPosition = coneOffsetDown;

        float angle = Mathf.Atan2(clamped.y, clamped.x) * Mathf.Rad2Deg;
        detectionCone.transform.localRotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    private Vector2 ClampToCardinal(Vector2 dir)
    {
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? (dir.x > 0 ? Vector2.right : Vector2.left) : (dir.y > 0 ? Vector2.up : Vector2.down);
    }

    public void EnableLightCone()
    {
        if (detectionCone != null) detectionCone.SetActive(true);
    }

    public void DisableLightCone()
    {
        if (detectionCone != null) detectionCone.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
