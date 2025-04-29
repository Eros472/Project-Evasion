using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Watcher : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private Vector3 moveDelta;
    private RaycastHit2D hit;

    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    private Vector3 targetPoint;
    private bool movingToB = true; // NEW: Tracks direction

    [Header("Field of Vision Settings")]
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstructionMask;
    public bool playerDetected = false;

    [Header("Visualization")]
    public bool showFOV = true;
    public LineRenderer fovRenderer;
    public int fovResolution = 20;

    private void Start()
    {
        targetPoint = pointB.position;
        Debug.Log($"Watcher starting at {transform.position}, pointA: {pointA.position}, pointB: {pointB.position}");
        
        if (showFOV && fovRenderer != null)
        {
            fovRenderer.positionCount = fovResolution + 1;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        Patrol();
        DetectPlayer();

        if (showFOV && fovRenderer != null)
        {
            DrawFieldOfView();
        }
    }

    private void OnDestroy()
    {
        Debug.LogWarning("WATCHER DESTROYED!!! Stack trace: " + Environment.StackTrace);
    }

    private bool isWaiting = false;

    private void Patrol()
    {
        if (isWaiting) return;

        targetPoint.z = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPoint, patrolSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, 0);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            StartCoroutine(PauseBeforeTurning());
        }
    }

    private IEnumerator PauseBeforeTurning()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f); // 1 second pause
        movingToB = !movingToB;
        targetPoint = movingToB ? pointB.position : pointA.position;
        isWaiting = false;
    }

    private void DetectPlayer()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, viewRadius, playerMask);

        foreach (Collider2D col in rangeChecks)
        {
            Debug.Log($"Detected object: {col.gameObject.name}");

            Transform target = col.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.right, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    playerDetected = true;
                    KillPlayer(target.gameObject);
                }
                else
                {
                    playerDetected = false;
                }
            }
            else
            {
                playerDetected = false;
            }
        }
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log("Player detected and killed!");
        //Destroy(player); // Replace with proper player death logic
    }

    private void DrawFieldOfView()
    {
        if (fovRenderer == null)
            return;

        if (fovRenderer.positionCount != fovResolution + 1)
        {
            fovRenderer.positionCount = fovResolution + 1;
        }

        float angleStep = viewAngle / fovResolution;
        float startingAngle = -viewAngle / 2;

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

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}

