using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SwordSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    private int attackCount;
    private float expireTime = 1f;
    private float acceptInputTimer = 0.6f;
    private bool canAcceptInput = true;
    private float timer;
    private bool isComboEnd = false;
    private Animator anim;

    [Header("Attack Settings")]
    [SerializeField] private float[] attackRanges = { 2f, 2f, 2.2f }; // Range for each attack in combo
    [SerializeField] private float[] attackDamage = { 10f, 15f, 25f }; // Damage for each attack in combo
    [SerializeField] private float[] attackDelays = { 0.4f, 0.4f, 0.5f }; // Delay before damage for each attack
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
        SoundManager.PlaySound(SoundType.MELEE);
        timer = 0f;
        
        // Clear hit enemies for this new attack in the combo
        hitEnemies.Clear();
        
        // Start delayed attack coroutine
        StartCoroutine(DelayedAttack());
    }
    
    
    private IEnumerator DelayedAttack() {
        float delay = (attackCount <= attackDelays.Length) ? attackDelays[attackCount - 1] : 0.3f;
        yield return new WaitForSeconds(delay);
        
        // Perform attack on enemies
        PerformAttack();
    }
    
    void PerformAttack() {
        if (attackCount <= 0 || attackCount > attackRanges.Length) return;
        
        // Get current attack stats (arrays are 0-indexed, attackCount is 1-indexed)
        float currentRange = attackRanges[attackCount - 1];
        float currentDamage = attackDamage[attackCount - 1];
        
        // Find all enemies in range
        Collider[] enemiesInRange = Physics.OverlapSphere(attackPoint.position, currentRange, enemyLayers);
        Debug.Log($"Attack {attackCount}: Found {enemiesInRange.Length} enemies in range {currentRange}");
        
        foreach (Collider enemy in enemiesInRange) {
            // Check if enemy has the correct tag
            if (enemy.CompareTag(enemyTag) || enemy.CompareTag("Boss")) {
                if (!hitEnemies.Contains(enemy)) {
                    hitEnemies.Add(enemy);
                    Debug.Log($"Hitting {enemy.name} with attack {attackCount}");
                    DealDamageToEnemy(enemy, currentDamage);
                } else {
                    Debug.Log($"Enemy {enemy.name} already hit in this attack");
                }
            }
        }
    }
    
    void DealDamageToEnemy(Collider enemy, float damage) {
        // Try to get EnemyHealthManager component
        var enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        if (enemyHealthManager != null) {
            enemyHealthManager.TakeDamage(damage, attackPoint.position);
        }
        
        // Add visual feedback here if needed (particles, screen shake, etc.)
    }
    
    public void OnAttackAnimationEvent() {
        PerformAttack();
    }
    
    public void ResetCombo() {
        hitEnemies.Clear();
        attackCount = 0;
        
        // Ensure animator is initialized before using it
        if (anim == null) {
            anim = GetComponent<Animator>();
        }
        
        if (anim != null) {
            anim.SetInteger("AttackCount", attackCount);
        }
        
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