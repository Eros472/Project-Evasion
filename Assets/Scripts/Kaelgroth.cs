using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Kaelgroth : Enemy // Inheriting from Enemy.cs
{
    public Animator kaelgrothAnimator;      // Kaelgroth's Animator reference
    public SpriteRenderer kaelgrothSprite;  // Kaelgroth's SpriteRenderer reference
    public Transform player;                // Player's Transform reference
    public float detectionRange = 7f;       // << SET THIS (e.g., 7f or 10f)
    public float attackRange = 3f;          // << SET THIS (e.g., 3f, or <1.5f if you want extreme closeness)
    public float moveSpeed = 3f;            // Movement speed of Kaelgroth
    public GameObject fireBeamPrefab;       // FireBeam prefab reference
    public GameObject fireballPrefab;       // Fireball prefab reference
    public Transform firePoint;             // FirePoint (Kaelgroth's mouth)

    // Damage values that can be set in the Inspector
    public int clawDamage = 25;
    public int fireBeamDamage = 30;     // Used by FireProjectile instance
    public int fireballDamage = 20;     // Used by FireProjectile instance
    public int roarDamage = 15;

    // Cooldown variables
    public float attackCooldown = 2.0f;
    private float lastAttackTime = 0f;

    // Kaelgroth-specific states (inherited 'isDead' and 'currentHealth' come from Enemy.cs)
    private bool inRange = false;       // Member field for general state
    private bool playerClose = false;   // Member field for general state
    private bool isTakingDamage = false; // Kaelgroth's specific state for its damage animation

    // NOTE: 'private bool isDead = false;' has been REMOVED from Kaelgroth.cs. 
    // Kaelgroth will use 'this.isDead' inherited from Enemy.cs

    protected override void Start()
    {
        base.Start(); // Call the base Start method from Enemy.cs (initializes currentHealth from maxHealth)

        if (kaelgrothAnimator == null) kaelgrothAnimator = GetComponent<Animator>();
        if (kaelgrothSprite == null) kaelgrothSprite = GetComponent<SpriteRenderer>();

        StartCoroutine(FindPlayerAfterSpawn());
    }

    private IEnumerator FindPlayerAfterSpawn()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("[Kaelgroth FindPlayer] Player with tag 'Player' not found!");
        }
    }

    void Update()
    {
        if (this.isDead) // Check inherited isDead state from Enemy.cs
        {
            if (kaelgrothAnimator != null && kaelgrothAnimator.GetBool("isMoving"))
            {
                kaelgrothAnimator.SetBool("isMoving", false);
            }
            return;
        }

        if (player == null)
        {
            Debug.LogError("[Kaelgroth UPDATE] PLAYER REFERENCE IS NULL! Kaelgroth cannot function.");
            if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Update the class member fields based on current distance
        this.inRange = distanceToPlayer < detectionRange;
        this.playerClose = distanceToPlayer < attackRange;

        // Sprite Flipping
        if (player.position.x > transform.position.x)
        {
            kaelgrothSprite.flipX = false;
            if (firePoint != null && firePoint.localScale.x < 0)
            {
                firePoint.localScale = new Vector3(Mathf.Abs(firePoint.localScale.x), firePoint.localScale.y, firePoint.localScale.z);
            }
        }
        else
        {
            kaelgrothSprite.flipX = true;
            if (firePoint != null && firePoint.localScale.x > 0)
            {
                firePoint.localScale = new Vector3(-Mathf.Abs(firePoint.localScale.x), firePoint.localScale.y, firePoint.localScale.z);
            }
        }

        // State Logic
        if (isTakingDamage)
        {
            if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", false);
        }
        else // Not taking damage, proceed with AI
        {
            // --- THIS IS THE CORRECTED/SIMPLIFIED AI LOGIC BLOCK ---
            Debug.Log($"[Kaelgroth AI Values] Dist: {distanceToPlayer:F2}, DetectionRange: {detectionRange} (inRange={this.inRange}), AttackRange: {attackRange} (playerClose={this.playerClose})");

            if (this.playerClose) // Priority 1: Player is very close (within actual attackRange)
            {
                Debug.Log("[Kaelgroth AI State] Player Close: Choosing Attack.");
                if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", false);
                ChooseAttack();
            }
            else if (this.inRange) // Priority 2: Player is in wider detectionRange (but not strictly playerClose)
            {
                Debug.Log("[Kaelgroth AI State] In Range (not Player Close): Moving.");
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", true);
            }
            else // Priority 3: Player is out of all ranges (not inRange and not playerClose)
            {
                Debug.Log("[Kaelgroth AI State] Out of Range: Idle.");
                if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", false);
            }
            // --- End of Corrected/Simplified AI Logic Block ---
        }
    }

    private void ChooseAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            int attackChoice = Random.Range(0, 4);
            switch (attackChoice)
            {
                case 0: // Claw Attack
                    kaelgrothAnimator.SetTrigger("AttackClaw");
                    // With the new AI logic, 'this.playerClose' should be true when ChooseAttack is called.
                    Debug.Log("[Kaelgroth ChooseAttack] Attempting Claw. PlayerClose = " + this.playerClose);
                    if (this.playerClose) ApplyDamageToPlayer(clawDamage); // Damage will be attempted
                    break;
                case 1: // FireBeam
                    kaelgrothAnimator.SetTrigger("Attack1");
                    // Projectile spawned by Animation Event "SpawnFireBeam"
                    break;
                case 2: // Fireball
                    kaelgrothAnimator.SetTrigger("Attack2");
                    // Projectile spawned by Animation Event "SpawnFireball"
                    break;
                case 3: // Roar Attack
                    kaelgrothAnimator.SetTrigger("Attack3");
                    Debug.Log("[Kaelgroth ChooseAttack] Attempting Roar. PlayerClose = " + this.playerClose);
                    if (this.playerClose) ApplyDamageToPlayer(roarDamage); // Damage will be attempted
                    break;
            }
            lastAttackTime = Time.time;
        }
    }

    public void SpawnFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            Debug.Log("[Kaelgroth] AnimationEvent: SpawnFireball called");
            GameObject projectileGO = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
            FireProjectile projectileScript = projectileGO.GetComponent<FireProjectile>();
            if (projectileScript != null)
            {
                projectileScript.damageAmount = this.fireballDamage;
            }
        }
        else
        {
            Debug.LogWarning("[Kaelgroth] SpawnFireball event: Prefab or FirePoint not set!");
        }
    }

    public void SpawnFireBeam()
    {
        if (fireBeamPrefab != null && firePoint != null)
        {
            Debug.Log("[Kaelgroth] AnimationEvent: SpawnFireBeam called");
            GameObject projectileGO = Instantiate(fireBeamPrefab, firePoint.position, firePoint.rotation);
            FireProjectile projectileScript = projectileGO.GetComponent<FireProjectile>();
            if (projectileScript != null)
            {
                projectileScript.damageAmount = this.fireBeamDamage;
            }
        }
        else
        {
            Debug.LogWarning("[Kaelgroth] SpawnFireBeam event: Prefab or FirePoint not set!");
        }
    }

    private void ApplyDamageToPlayer(int damageAmount)
    {
        if (player != null)
        {
            float currentDistance = Vector2.Distance(transform.position, player.position);
            Debug.Log($"[Kaelgroth ApplyDamagePlayer] Called. Dist: {currentDistance}, Req: <= {attackRange + 0.5f}");
            if (currentDistance <= attackRange + 0.5f) // Final check for direct damage
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    Debug.Log("[Kaelgroth ApplyDamagePlayer] Player script found. Applying " + damageAmount + " direct damage.");
                    playerScript.TakeDamage(damageAmount);
                }
                else { Debug.LogWarning("[Kaelgroth ApplyDamagePlayer] Player script NOT found on player object!"); }
            }
            else { Debug.Log($"[Kaelgroth ApplyDamagePlayer] Player too far for direct damage (Dist: {currentDistance}, Req: {attackRange + 0.5f})."); }
        }
        else { Debug.LogWarning("[Kaelgroth ApplyDamagePlayer] Player reference is null for ApplyDamageToPlayer!"); }
    }

    public override void TakeDamage(int damageAmount)
    {
        if (this.isDead) return;

        this.isTakingDamage = true;
        if (kaelgrothAnimator != null) kaelgrothAnimator.SetTrigger("isDamaged");

        base.TakeDamage(damageAmount); // Base class handles health reduction & calls Kaelgroth.Die() if overridden
    }

    protected override void Die()
    {
        if (this.isDead) return;

        base.Die(); // Sets inherited 'this.isDead = true' and runs Enemy.Die() logic

        // Kaelgroth-specific death sequence
        if (kaelgrothAnimator != null)
        {
            kaelgrothAnimator.SetBool("isMoving", false);
            kaelgrothAnimator.SetTrigger("Die");
        }
        Debug.Log("[Kaelgroth] Kaelgroth's specific Die() method called! Death animation started.");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

    }

    public void OnDeathAnimationComplete() // Called by Animation Event
    {
        Debug.Log("[Kaelgroth] Death animation event triggered. Transitioning to Main Menu.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void EndDamageState() // Called by Animation Event
    {
        isTakingDamage = false;
    }

    public void EndMovement() // Potentially called by Animation Events
    {
        if (kaelgrothAnimator != null) kaelgrothAnimator.SetBool("isMoving", false);
    }
}