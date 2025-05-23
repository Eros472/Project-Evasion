﻿using UnityEngine;

public class Ogre : MonoBehaviour
{
    public float detectionRadius = 6f;
    public float meleeRange = 1.5f;
    public float cooldownTime = 1f;
    public GameObject boulderPrefab;
    public Transform throwPoint;
    public LayerMask playerMask;
    public float chaseSpeed = 2.5f;

    private Transform player;
    private Animator animator;
    private OgreState currentState = OgreState.Idle;
    private float stateTimer = 0f;
    private OgreState nextState;

    private enum OgreState
    {
        Idle,
        Alert,
        Throw,
        Chase,
        Melee,
        Cooldown
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName("Ogre_Chase")) Debug.Log("Animator: CHASE animation playing");

        switch (currentState)
        {
            case OgreState.Idle:
                animator.SetBool("isChasing", false);
                Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask);
                if (hit != null)
                {
                    player = hit.transform;
                    float dist = Vector2.Distance(transform.position, player.position);
                    // Engage chase if reasonably close
                    nextState = (dist <= detectionRadius * 0.7f) ? OgreState.Chase : OgreState.Throw;

                    animator.SetTrigger("isAlerted");
                    TransitionTo(OgreState.Alert, 1.5f);
                }
                break;

            case OgreState.Alert:
                stateTimer -= Time.deltaTime;
                if (!PlayerInRange(0f, detectionRadius))
                {
                    TransitionTo(OgreState.Idle);
                    break;
                }
                if (stateTimer <= 0f)
                {
                    TransitionTo(nextState);
                }
                break;

            case OgreState.Chase:
                if (player != null)
                {
                    animator.SetBool("isChasing", true);
                    FacePlayer();

                    float dist = Vector2.Distance(transform.position, player.position);
                    Debug.Log("Chasing player — distance: " + dist);

                    if (dist > 0.5f)
                    {
                        transform.position = Vector2.MoveTowards(
                            transform.position,
                            player.position,
                            chaseSpeed * Time.deltaTime
                        );
                    }

                    if (dist <= 0.5f)
                    {
                        Debug.Log("Player in melee range — attacking.");
                        animator.SetBool("isChasing", false);
                        animator.SetTrigger("meleeAttack");
                        TransitionTo(OgreState.Melee);
                    }
                }
                break;

            case OgreState.Throw:
                FacePlayer();
                break;

            case OgreState.Melee:
                // Attack animation plays, kill handled on collision
                break;

            case OgreState.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                    TransitionTo(OgreState.Idle);
                break;
        }
    }

    private void TransitionTo(OgreState newState, float delay = 0f)
    {
        currentState = newState;
        stateTimer = delay;

        switch (newState)
        {
            case OgreState.Alert:
                Debug.Log("Entering ALERT");
                break;

            case OgreState.Throw:
                Debug.Log("Entering THROW");
                animator.ResetTrigger("meleeAttack");
                animator.ResetTrigger("isAlerted");
                animator.SetBool("isChasing", false);
                animator.SetTrigger("isThrowing");
                break;

            case OgreState.Chase:
                Debug.Log("Entering CHASE");
                animator.SetBool("isChasing", true);
                break;

            case OgreState.Melee:
                Debug.Log("Entering MELEE");
                animator.SetBool("isChasing", false);
                break;

            case OgreState.Cooldown:
                Debug.Log("Entering COOLDOWN");
                break;

            case OgreState.Idle:
                Debug.Log("Returning to IDLE");
                animator.SetBool("isChasing", false);
                break;
        }
    }

    private bool PlayerInRange(float min, float max)
    {
        if (player == null) return false;
        float dist = Vector2.Distance(transform.position, player.position);
        return dist >= min && dist <= max;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // Called by animation event
    public void ThrowBoulder()
    {
        if (player == null) return;

        Vector2 dir = (player.position - throwPoint.position).normalized;
        GameObject boulder = Instantiate(boulderPrefab, throwPoint.position, Quaternion.identity);

        Rigidbody2D rb = boulder.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * 5f;
        }

        SpriteRenderer sr = boulder.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = dir.x < 0;
        }

        Debug.Log("Boulder thrown!");
        TransitionTo(OgreState.Cooldown, cooldownTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentState == OgreState.Melee)
        {
            Debug.Log("Player touched — Ogre deals melee damage.");
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(5); // Set your melee damage value
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
