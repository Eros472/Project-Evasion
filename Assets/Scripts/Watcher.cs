using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    private Vector3 targetPoint;

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
        if (showFOV && fovRenderer != null)
        {
            fovRenderer.positionCount = fovResolution + 1;
        }
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

    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip sprite if needed
        }
    }

    private void DetectPlayer()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
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
        else
        {
            playerDetected = false;
        }
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log("Player detected and killed!");
        Destroy(player); // Replace with proper player death logic
    }

    private void DrawFieldOfView()
    {
        float angleStep = viewAngle / fovResolution;
        float startingAngle = -viewAngle / 2;

        fovRenderer.SetPosition(0, transform.position);

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
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}

