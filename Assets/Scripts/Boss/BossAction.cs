using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    [Header("Movement")]
    [SerializeField] float bossSpeed = 5f;

    [Header("Shooting")]
    [SerializeField] float shootingInterval = 2f;
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject projectilePrefab;

    [Header("State Durations")]
    [SerializeField] float idleDuration = 10f;
    [SerializeField] float firstPatternDuration = 10f;
    [SerializeField] float secondPatternDuration = 10f;

    [Header("First Pattern - Orbit & Charge")]
    [SerializeField] float orbitRadius = 5f;
    [SerializeField] float orbitSpeed = 2f;
    // Charge speed is 2x boss speed (handled in code)

    [Header("Second Pattern - Minion Summoning")]
    [SerializeField] GameObject enemySpawner;

    // Private variables
    bool isRunningState;
    float projectileTimer;
    Vector3 bossInitialPosition;
    GameObject player;

    public BossStates currentState { get; private set; }

    /// <summary>
    /// Boss State Machine:
    /// - IDLING: Return to position while shooting (Available: 100%-0% HP)
    /// - FIRSTPATTERN: Orbit player, shoot, then charge (Unlocked: 70% HP)
    /// - SECONDPATTERN: Shoot stationary, summon minions (Unlocked: 35% HP)
    /// - DEAD: Death behavior (0% HP)
    /// </summary>
    public enum BossStates
    {
        IDLING,
        FIRSTPATTERN,
        SECONDPATTERN,
        DEAD
    }

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Initialize state management
        isRunningState = false;
        projectileTimer = shootingInterval;

        // Store initial position
        bossInitialPosition = transform.position;

        // Find player
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");
        }

        // Start with IDLING state
        currentState = BossStates.IDLING;
    }

    void Update()
    {
        // Handle state machine
        switch (currentState)
        {
            case BossStates.IDLING:
                if (!isRunningState)
                {
                    isRunningState = true;
                    StartCoroutine(IdleBehaviour());
                }
                break;

            case BossStates.FIRSTPATTERN:
                if (!isRunningState)
                {
                    isRunningState = true;
                    StartCoroutine(FirstPatternBehaviour());
                }
                break;

            case BossStates.SECONDPATTERN:
                if (!isRunningState)
                {
                    isRunningState = true;
                    StartCoroutine(SecondPatternBehaviour());
                }
                break;

            case BossStates.DEAD:
                if (!isRunningState)
                {
                    isRunningState = true;
                    StartCoroutine(DeadBehaviour());
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Returns list of available states based on current health percentage
    /// </summary>
    List<BossStates> GetAvailableStates()
    {
        List<BossStates> availableStates = new List<BossStates>();
        float healthPercentage = (currentHealth / maxHealth) * 100f;

        if (healthPercentage <= 0)
        {
            // Boss is dead
            availableStates.Add(BossStates.DEAD);
        }
        else
        {
            // IDLE is always available (100% - 1% HP)
            availableStates.Add(BossStates.IDLING);

            // FIRSTPATTERN unlocks at 70% HP
            if (healthPercentage <= 70f)
            {
                availableStates.Add(BossStates.FIRSTPATTERN);
            }

            // SECONDPATTERN unlocks at 35% HP
            if (healthPercentage <= 35f)
            {
                availableStates.Add(BossStates.SECONDPATTERN);
            }
        }

        return availableStates;
    }

    /// <summary>
    /// Randomly selects next state from available states based on health
    /// </summary>
    IEnumerator StateRandomizer()
    {
        yield return new WaitForSeconds(1f); // Brief pause between states

        List<BossStates> availableStates = GetAvailableStates();

        if (availableStates.Count > 0)
        {
            // Pick random state from available pool
            int randomIndex = Random.Range(0, availableStates.Count);
            BossStates newState = availableStates[randomIndex];

            SetBossState(newState);
            Debug.Log($"[Boss] Health: {currentHealth}/{maxHealth} ({(currentHealth / maxHealth) * 100f}%) | Changed to {currentState}");
        }

        isRunningState = false; // ✅ Allow new state coroutine to start
    }

    public void SetBossState(BossStates newBossStates)
    {
        currentState = newBossStates;
    }

    public BossStates GetBossState()
    {
        return currentState;
    }

    // ============================================
    // STATE BEHAVIORS
    // ============================================

    /// <summary>
    /// IDLE STATE: Return to initial position while shooting at player
    /// </summary>
    IEnumerator IdleBehaviour()
    {
        Debug.Log("[Boss] Executing IDLE behavior - Returning to position and shooting");

        float elapsedTime = 0f;

        // FIX: Continuous behavior using while loop
        while (elapsedTime < idleDuration)
        {
            if (player != null)
            {
                // Move back to initial position
                BackToPosition();

                // Shoot at player
                ShootingAtPlayer();
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait one frame
        }

        // Transition to next random state
        StartCoroutine(StateRandomizer());
    }

    /// <summary>
    /// Moves boss back to initial position while looking at player
    /// </summary>
    void BackToPosition()
    {
        // Look at player
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation on horizontal plane

        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // Move toward initial position
        transform.position = Vector3.MoveTowards(
            transform.position,
            bossInitialPosition,
            bossSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Shoots projectiles at player based on shooting interval
    /// </summary>
    void ShootingAtPlayer()
    {
        if (projectileTimer <= 0)
        {
            // Instantiate projectile at shoot point
            if (projectilePrefab != null && shootPoint != null)
            {
                Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            }

            // Reset timer
            projectileTimer = shootingInterval;
        }
        else
        {
            projectileTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// FIRST PATTERN: Orbit around player while shooting, then charge through player
    /// </summary>
    IEnumerator FirstPatternBehaviour()
    {
        Debug.Log("[Boss] Executing FIRST PATTERN - Orbiting and charging");

        if (player == null) yield break;

        // === PHASE 1: ORBIT AROUND PLAYER ===
        float orbitTime = 0f;
        float currentAngle = 0f;

        while (orbitTime < firstPatternDuration)
        {
            if (player == null) yield break;

            // Calculate orbit position using circular motion
            // x = centerX + radius * cos(angle)
            // z = centerZ + radius * sin(angle)
            currentAngle += orbitSpeed * Time.deltaTime; // Clockwise rotation

            float x = player.transform.position.x + orbitRadius * Mathf.Cos(currentAngle);
            float z = player.transform.position.z + orbitRadius * Mathf.Sin(currentAngle);

            Vector3 orbitPosition = new Vector3(x, transform.position.y, z);

            // Move to orbit position
            transform.position = Vector3.MoveTowards(
                transform.position,
                orbitPosition,
                bossSpeed * Time.deltaTime
            );

            // Look at player while orbiting
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }

            // Shoot while orbiting
            ShootingAtPlayer();

            orbitTime += Time.deltaTime;
            yield return null;
        }

        // === PHASE 2: CHARGE THROUGH PLAYER ===
        Debug.Log("[Boss] CHARGING!");

        // Store player position at charge start
        Vector3 chargeTargetPosition = player.transform.position;

        // Calculate direction and extend target point beyond player
        Vector3 chargeDirection = (chargeTargetPosition - transform.position).normalized;
        Vector3 extendedTarget = chargeTargetPosition + (chargeDirection * 10f); // Overshoot

        // Look at charge direction
        transform.rotation = Quaternion.LookRotation(chargeDirection);

        float chargeSpeed = bossSpeed * 2f; // 2x normal speed
        float chargeDuration = 2f; // Charge for 2 seconds
        float chargeTime = 0f;

        while (chargeTime < chargeDuration)
        {
            // Charge forward
            transform.position = Vector3.MoveTowards(
                transform.position,
                extendedTarget,
                chargeSpeed * Time.deltaTime
            );

            chargeTime += Time.deltaTime;
            yield return null;
        }

        // Transition to next random state
        StartCoroutine(StateRandomizer());
    }

    /// <summary>
    /// SECOND PATTERN: Shoot while stationary, then summon minions at player position
    /// </summary>
    IEnumerator SecondPatternBehaviour()
    {
        Debug.Log("[Boss] Executing SECOND PATTERN - Shooting and summoning minions");

        if (player == null) yield break;

        // Stay stationary and shoot
        float shootTime = 0f;
        float summonTime = secondPatternDuration * 0.7f; // Shoot for 70% of duration

        while (shootTime < summonTime)
        {
            if (player != null)
            {
                // Look at player
                Vector3 directionToPlayer = player.transform.position - transform.position;
                directionToPlayer.y = 0;

                if (directionToPlayer != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(directionToPlayer);
                }

                // Shoot at player
                ShootingAtPlayer();
            }

            shootTime += Time.deltaTime;
            yield return null;
        }

        // === SUMMON MINIONS ===
        Debug.Log("[Boss] SUMMONING MINIONS!");

        if (enemySpawner != null && player != null)
        {
            // Spawn the enemy spawner at player's position
            Instantiate(enemySpawner, player.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[Boss] Enemy spawner prefab not assigned!");
        }

        // Wait remainder of duration
        yield return new WaitForSeconds(secondPatternDuration - summonTime);

        // Transition to next random state
        StartCoroutine(StateRandomizer());
    }

    /// <summary>
    /// DEAD STATE: Boss death behavior
    /// </summary>
    IEnumerator DeadBehaviour()
    {
        Debug.Log("[Boss] BOSS DEFEATED!");

        // Add death effects here (particles, sound, animation, etc.)

        yield return new WaitForSeconds(2f);

        // Destroy boss or disable
        Destroy(gameObject);
    }

    // ============================================
    // PUBLIC METHODS (for health damage system)
    // ============================================

    /// <summary>
    /// Call this method to damage the boss
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Boss] Took {damage} damage! Health: {currentHealth}/{maxHealth}");

        // Check if boss died
        if (currentHealth <= 0 && currentState != BossStates.DEAD)
        {
            SetBossState(BossStates.DEAD);
            isRunningState = false; // Allow death coroutine to run
        }
    }

    /// <summary>
    /// Get current health percentage
    /// </summary>
    public float GetHealthPercentage()
    {
        return (currentHealth / maxHealth) * 100f;
    }
}