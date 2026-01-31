using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SwordSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    private int attackCount;
    private float expireTime = 1.3f;
    private float acceptInputTimer = 0.7f;
    private bool canAcceptInput = true;
    private float timer;
    private bool isComboEnd = false;
    private Animator anim;

    [Header("Attack Settings")]
    [SerializeField] private float[] attackRanges = { 2f, 2f, 2.2f }; // Range for each attack in combo
    [SerializeField] private float[] attackDamage = { 10f, 15f, 25f }; // Damage for each attack in combo
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private Transform attackPoint; // Point from which attacks are calculated
    [SerializeField] private LayerMask enemyLayers = 3; // What layers are considered enemies

    [Header("Visual Feedback")]
    [SerializeField] private bool showAttackGizmos = true;
    [SerializeField] private Color gizmoColor = Color.red;

    private List<Collider> hitEnemies = new List<Collider>(); // Track enemies hit in current attack

    public void OnMouseClick(InputAction.CallbackContext context) {
        if (context.performed) {
            OnAttack();
        }
    }

    void Start() {
        anim = GetComponent<Animator>();
        attackCount = anim.GetInteger("AttackCount");
        
        // Set attack point to this transform if not assigned
        if (attackPoint == null) {
            attackPoint = transform;
        }
    }

    void Update() {
        if (attackCount > 0) {
            timer += Time.deltaTime;
            if (timer >= expireTime) {
                attackCount = 0;
                anim.SetInteger("AttackCount", attackCount);
                timer = 0f;
            }
            if (timer >= acceptInputTimer) {
                canAcceptInput = true;
            }
            if (isComboEnd) {
                Debug.Log("Combo ended");
                attackCount = 0;
                anim.SetInteger("AttackCount", attackCount);
                isComboEnd = false;
                canAcceptInput = true;
                hitEnemies.Clear(); // Reset hit enemies for next combo
            }
        }
    }

    void OnAttack() {
        if (canAcceptInput == false) {
            return;
        }

        canAcceptInput = false;

        attackCount++;
        if (attackCount > 3) {
            isComboEnd = true;
        }
        anim.SetInteger("AttackCount", attackCount);
        timer = 0f;
        
        // Perform attack on enemies
        PerformAttack();
    }
    
    /// <summary>
    /// Performs the actual attack, detecting and damaging enemies
    /// </summary>
    void PerformAttack() {
        if (attackCount <= 0 || attackCount > attackRanges.Length) return;
        
        // Get current attack stats (arrays are 0-indexed, attackCount is 1-indexed)
        float currentRange = attackRanges[attackCount - 1];
        float currentDamage = attackDamage[attackCount - 1];
        
        // Find all enemies in range
        Collider[] enemiesInRange = Physics.OverlapSphere(attackPoint.position, currentRange, enemyLayers);
        Debug.Log(enemiesInRange);
        
        foreach (Collider enemy in enemiesInRange) {
            // Check if enemy has the correct tag
            if (enemy.CompareTag(enemyTag)) {
                // Prevent hitting the same enemy multiple times in one combo
                Debug.Log("Hitting enemy");
                if (!hitEnemies.Contains(enemy)) {
                    hitEnemies.Add(enemy);
                    DealDamageToEnemy(enemy, currentDamage);
                }
            }
        }
    }
    
    /// <summary>
    /// Deals damage to an enemy
    /// </summary>
    void DealDamageToEnemy(Collider enemy, float damage) {
        // Try to get health component (adapt this to your enemy health system)
        // var healthComponent = enemy.GetComponent<Health>();
        // if (healthComponent != null) {
        //     healthComponent.TakeDamage(damage);
        // }
        
        // // Alternative: Try different health component names
        // var enemyHealth = enemy.GetComponent<EnemyHealth>();
        // if (enemyHealth != null) {
        //     enemyHealth.TakeDamage(damage);
        // }
        
        // Log for debugging
        Debug.Log($"Hit {enemy.name} for {damage} damage with attack {attackCount}");
        
        // Add visual feedback here if needed (particles, screen shake, etc.)
    }
    
    /// <summary>
    /// Call this method from animation events to trigger attacks at specific frames
    /// </summary>
    public void OnAttackAnimationEvent() {
        PerformAttack();
    }
    
    /// <summary>
    /// Resets the hit enemies list when a new combo starts
    /// </summary>
    public void ResetCombo() {
        hitEnemies.Clear();
        attackCount = 0;
        anim.SetInteger("AttackCount", attackCount);
        timer = 0f;
        canAcceptInput = true;
        isComboEnd = false;
    }
    
    void OnDrawGizmosSelected() {
        if (!showAttackGizmos || attackPoint == null) return;
        
        // Set gizmo color
        Gizmos.color = gizmoColor;
        
        // Draw attack ranges for each combo attack
        for (int i = 0; i < attackRanges.Length; i++) {
            // Adjust transparency for each attack
            Color currentColor = gizmoColor;
            currentColor.a = 0.2f + (0.3f * i / attackRanges.Length);
            Gizmos.color = currentColor;
            
            // Draw sphere representing attack range
            Gizmos.DrawSphere(attackPoint.position, attackRanges[i]);
            
            // Draw wireframe for better visibility
            Gizmos.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            Gizmos.DrawWireSphere(attackPoint.position, attackRanges[i]);
        }
        
        // Draw a small cube at attack point
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(attackPoint.position, Vector3.one * 0.1f);
    }
}