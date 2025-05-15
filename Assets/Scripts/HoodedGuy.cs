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
    public float detectionRange = 5f;
    public float detectionAngle = 45f;

    public Transform player;
    public GameObject detectionCone;

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

        if (detectionCone != null)
            detectionCone.SetActive(false);
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
        if (player == null) return false;

        Vector2 toPlayer = player.position - transform.position;
        float angle = Vector2.Angle(lookDirection, toPlayer.normalized);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, detectionRange, LayerMask.GetMask("Player"));
        return hit.collider != null && hit.collider.CompareTag("Player") && angle < detectionAngle;
    }

    IEnumerator FadeBoth(SpriteRenderer npcRenderer, SpriteRenderer coneRenderer, float targetAlpha)
    {
        float startAlpha = npcRenderer.color.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            if (npcRenderer != null)
                npcRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            if (coneRenderer != null)
                coneRenderer.color = new Color(coneRenderer.color.r, coneRenderer.color.g, coneRenderer.color.b, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (npcRenderer != null)
            npcRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
        if (coneRenderer != null)
            coneRenderer.color = new Color(coneRenderer.color.r, coneRenderer.color.g, coneRenderer.color.b, targetAlpha);
    }

    IEnumerator ReappearAndStrike()
    {
        SpriteRenderer coneRenderer = detectionCone != null ? detectionCone.GetComponent<SpriteRenderer>() : null;
        yield return StartCoroutine(FadeBoth(spriteRenderer, coneRenderer, 0f));

        yield return new WaitForSeconds(vanishDelay);

        if (player != null)
        {
            Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            transform.position = player.position + (Vector3)offset;
        }

        spriteRenderer.enabled = true;
        if (coneRenderer != null) coneRenderer.enabled = true;

        yield return StartCoroutine(FadeBoth(spriteRenderer, coneRenderer, 1f));

        animator.SetTrigger("ReappearAttack");
        yield return new WaitForSeconds(0.4f);

        if (player != null)
        {
            Destroy(player.gameObject);
        }

        yield return new WaitForSeconds(0.8f);
        animator.SetTrigger("ResumePatrol");
        currentState = State.WalkPatrol;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != State.Spotted && other.CompareTag("Player"))
        {
            Debug.Log("Player entered detection cone!");
            currentState = State.Spotted;
            animator.SetTrigger("Disappear");
            StartCoroutine(ReappearAndStrike());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            currentState = State.LookPatrol;
            lookTimer = 0f;
            movingForward = !movingForward;
            targetPoint = movingForward ? pointB : pointA;

            if (!patrolsVertically)
                spriteRenderer.flipX = lastMoveDirection.x < 0;
        }
    }
}