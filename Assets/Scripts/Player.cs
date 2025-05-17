using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Vector3 moveDelta;
    public GameObject bloodEffectPrefab;

    public float walkMultiplier = 1.0f;
    public float runMultiplier = 1.5f;

    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    public HealthBar healthBar;

    private static Player instance;

    public GameObject arrowPrefab;
    public Transform shootPoint;

    public GameObject bowModelObject;
    public GameObject daggerModelObject;

    public WeaponType currentWeapon;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        // Ensure health is correctly set from GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestorePlayerHealth(out int savedCurrent, out int savedMax);

            if (savedMax > 0 && savedCurrent >= 0)
            {
                maxHealth = savedMax;
                currentHealth = savedCurrent;
                Debug.Log($"[Player] Loaded saved health: {currentHealth}/{maxHealth}");
            }
            else
            {
                currentHealth = maxHealth;
                GameManager.Instance.SavePlayerHealth(currentHealth, maxHealth);
                Debug.Log($"[Player] Resetting health to max: {maxHealth}");
            }
        }
        else
        {
            currentHealth = maxHealth; // If no GameManager, set to default
        }

        if (healthBar == null && UIManager.Instance != null)
            healthBar = UIManager.Instance.healthBar;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }




    public void SetHealth(int newHealth)
    {
        currentHealth = newHealth;
        healthBar.SetHealth(currentHealth);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[Player] Scene loaded: " + scene.name);

        int targetMaxHealthForScene;
        int targetCurrentHealthForScene;

        // Determine the correct health values for the loaded scene
        if (scene.name == "Level2" || scene.name == "Forest") // Use your exact scene names
        {
            targetMaxHealthForScene = 300;
            targetCurrentHealthForScene = 300; // Start with full health for the level
        }
        else // Default for MainScene, MainMenu, or any other scene
        {
            targetMaxHealthForScene = 50;
            targetCurrentHealthForScene = 50; // Start with full health for the level
        }

        // --- Apply the new health values to the Player object ---
        this.maxHealth = targetMaxHealthForScene;
        this.currentHealth = targetCurrentHealthForScene;

        Debug.Log($"[Player] Stats for scene '{scene.name}': MaxHealth = {this.maxHealth}, CurrentHealth = {this.currentHealth}");

        // Update the HealthBar UI
        // Re-acquire HealthBar instance in case UI is reloaded/changed with the scene
        if (UIManager.Instance != null)
        {
            healthBar = UIManager.Instance.healthBar;
        }
        // It's also good practice to check healthBar for null again before using
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(this.maxHealth); // Use the Player's updated maxHealth
            healthBar.SetHealth(this.currentHealth); // Use the Player's updated currentHealth
            Debug.Log("[Player] HealthBar updated for new scene values.");
        }
        else
        {
            Debug.LogWarning("[Player] HealthBar reference is null in OnSceneLoaded for scene: " + scene.name);
        }

        // Save the new current and max health to GameManager so it's aware of the change
        // This is important if GameManager's saved values are used for respawns within the same level
        // or if other systems query GameManager for player health.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerHealth(this.currentHealth, this.maxHealth);
            Debug.Log($"[Player] Saved new scene-specific health to GameManager: {this.currentHealth}/{this.maxHealth}");
        }
    }




    void Update()
    {
        if (isDead) return;
        HandleInputAndAnimation();
        HandleCombatInput();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        float speedMultiplier = walkMultiplier;
        if (Input.GetKey(KeyCode.LeftShift)) speedMultiplier = runMultiplier;
        if (Input.GetKey(KeyCode.LeftControl)) speedMultiplier = walkMultiplier * 0.5f;

        transform.Translate(moveDelta * speedMultiplier * Time.fixedDeltaTime);
    }

    private void HandleInputAndAnimation()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveDelta = new Vector3(x, y, 0);

        if (moveDelta.x > 0)
            transform.localScale = Vector3.one;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Speed", moveDelta.sqrMagnitude);
        animator.speed = isRunning ? 1.5f : 1.0f;
    }

    private void HandleCombatInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentWeapon == WeaponType.Bow)
        {
            ShootArrow();
        }
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || shootPoint == null) return;

        float dir = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 shootDirection = new Vector2(dir, 0f);
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(0, 0, angle));

        PlayerArrow arrowScript = arrow.GetComponent<PlayerArrow>();
        if (arrowScript != null)
        {
            arrowScript.direction = shootDirection;
        }

        Debug.Log($"Arrow fired! Angle: {angle}, Direction: {shootDirection}");
    }

    public void EquipWeapon(WeaponType weapon)
    {
        if (daggerModelObject != null) daggerModelObject.SetActive(false);
        if (bowModelObject != null) bowModelObject.SetActive(false);

        currentWeapon = weapon;

        if (weapon == WeaponType.Dagger && daggerModelObject != null)
            daggerModelObject.SetActive(true);
        else if (weapon == WeaponType.Bow && bowModelObject != null)
            bowModelObject.SetActive(true);
        else
            Debug.Log("[Player] No weapon equipped — visuals cleared.");

        Debug.Log("Equipped: " + currentWeapon);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        Debug.Log($"[PLAYER SCRIPT] TakeDamage({amount}) called. Previous Health: {currentHealth}");

        // Reduce health and ensure it doesn't go below 0 or above maxHealth
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Save current health and max health after damage is applied
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerHealth(currentHealth, maxHealth);
        }

        // Instantiate blood effect if assigned
        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        // Ensure healthBar reference is set
        if (healthBar == null && UIManager.Instance != null)
        {
            healthBar = UIManager.Instance.healthBar;
        }

        // Update the health bar UI
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogWarning("[Player] HealthBar is still null during TakeDamage");
        }

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }


    private IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}