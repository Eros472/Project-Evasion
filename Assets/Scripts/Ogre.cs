using UnityEngine;

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

    private enum OgreState
    {
        Idle,
        Alert,
        Throw,
        Melee,
        Cooldown
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case OgreState.Idle:
                LookForPlayer();
                break;

            case OgreState.Alert:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    if (PlayerInRange(meleeRange))
                        TransitionTo(OgreState.Melee);
                    else
                        TransitionTo(OgreState.Throw);
                }
                break;

            case OgreState.Throw:
                // ThrowBoulder() called by animation event
                FacePlayer();
                break;

            case OgreState.Melee:
                if (player != null)
                {
                    FacePlayer();
                    // Chase the player continuously
                    transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
                }
                break;

            case OgreState.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                    TransitionTo(OgreState.Idle);
                break;
        }
    }

    private void LookForPlayer()
    {
        if (currentState != OgreState.Idle) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask);
        if (hit != null)
        {
            player = hit.transform;
            Debug.Log("Player detected! Entering ALERT state.");
            TransitionTo(OgreState.Alert, 0.5f);
        }
    }

    private bool PlayerInRange(float range)
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= range;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void TransitionTo(OgreState newState, float delay = 0f)
    {
        currentState = newState;
        stateTimer = delay;

        switch (newState)
        {
            case OgreState.Alert:
                animator.Play("Ogre_Alert");
                break;
            case OgreState.Throw:
                animator.SetTrigger("ThrowTrigger");
                break;
            case OgreState.Melee:
                animator.SetTrigger("MeleeTrigger");
                break;
        }
    }

    // This gets called via animation event at the right frame
    public void ThrowBoulder()
    {
        Debug.Log("THROW BOULDER CALLED!");

        if (player == null) return;

        Vector2 dir = (player.position - throwPoint.position).normalized;
        GameObject boulder = Instantiate(boulderPrefab, throwPoint.position, Quaternion.identity);

        // Ensure it has Rigidbody2D
        Rigidbody2D rb = boulder.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * 5f; // Match Goblin's arrow speed
        }

        // Optional flip or rotate
        SpriteRenderer sr = boulder.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = dir.x < 0;
        }

        Debug.Log("Boulder thrown!");
        TransitionTo(OgreState.Cooldown, cooldownTime);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
