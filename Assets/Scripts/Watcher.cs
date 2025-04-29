using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour
{
    public float patrolSpeed = 1.5f;
    public float patrolDistance = 1f; // shorter patrol distance
    public float viewRadius = 3f;
    public float alertDuration = 4f;
    public LayerMask playerMask;

    private Vector3 initialPosition;
    private Vector3 leftPoint;
    private Vector3 rightPoint;
    private Vector3 targetPoint;
    private bool movingRight = true;

    private int alertFrameIndex = 0;
    private float frameSwitchTimer = 0f;


    private Animator animator;

    private bool isAlerted = false;
    private float alertTimer = 0f;
    private bool playerCurrentlyInView = false;

    private float patrolPauseDuration = 1f;
    private float patrolPauseTimer = 0f;
    private bool isPausedAtEdge = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        leftPoint = initialPosition + Vector3.left * patrolDistance;
        rightPoint = initialPosition + Vector3.right * patrolDistance;
        targetPoint = rightPoint;

        animator.SetBool("IsAlerted", false);
    }

    private void Update()
    {
        if (isAlerted)
        {
            frameSwitchTimer += Time.deltaTime;

            if (frameSwitchTimer >= 1.0f && alertFrameIndex < 4)
            {
                frameSwitchTimer = 0f;
                alertFrameIndex++;
                animator.SetInteger("AlertFrame", alertFrameIndex);
            }

            if (!PlayerStillInView())
            {
                isAlerted = false;
                alertTimer = 0f;
                alertFrameIndex = 0;
                frameSwitchTimer = 0f;
                animator.SetBool("IsAlerted", false);
                animator.SetInteger("AlertFrame", 0);
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

                // Flip sprite AFTER pause
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
        }
    }

    private bool DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
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
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}









