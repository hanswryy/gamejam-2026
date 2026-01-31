using System.Collections;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    public int currentHealth;
    
    [Header("Damage & Knockback")]
    public float invulnerabilityDuration = 0.2f;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;
    
    [Header("Visual Feedback")]
    public float flashDuration = 0.1f;
    
    private bool isInvulnerable = false;
    private bool isKnockedBack = false;
    private Vector3 knockbackVelocity;
    private Renderer enemyRenderer;
    private Color originalColor;
    private MonoBehaviour enemyController; // Generic reference to enemy movement script
    
    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        
        // Try to find common enemy movement components
        enemyController = GetComponent<MonoBehaviour>(); // Will need to be more specific based on your enemy scripts
        
        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;
    }
    
    void Update()
    {
        // Apply knockback movement
        if (isKnockedBack)
        {
            transform.Translate(knockbackVelocity * Time.deltaTime, Space.World);
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
        }
    }
    
    public void TakeDamage(float damage, Vector3 damageSource)
    {
        if (isInvulnerable) return;
        
        currentHealth -= Mathf.RoundToInt(damage);
        
        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0f; // Keep knockback horizontal
        knockbackVelocity = knockbackDirection * knockbackForce;
        
        StartCoroutine(InvulnerabilityFrames());
        StartCoroutine(KnockbackEffect());
        StartCoroutine(DamageFlash());
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator KnockbackEffect()
    {
        isKnockedBack = true;
        
        // Disable enemy movement temporarily (adapt this based on your enemy movement scripts)
        if (enemyController != null)
        {
            // You might need to adjust this based on your specific enemy controller
            // For example: enemyController.enabled = false;
        }
        
        yield return new WaitForSeconds(knockbackDuration);
        
        // Re-enable enemy movement
        if (enemyController != null)
        {
            // enemyController.enabled = true;
        }
        
        isKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }
    
    private IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        
        // Flicker effect during invulnerability
        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.enabled = !enemyRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = true;
        }
        
        isInvulnerable = false;
    }
    
    private IEnumerator DamageFlash()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            enemyRenderer.material.color = originalColor;
        }
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // Add death effects here (particles, sound, etc.)
        
        // Disable enemy components
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
            enemyCollider.enabled = false;
            
        if (enemyController != null)
            enemyController.enabled = false;
        
        // You can add death animation here or destroy the enemy
        // For now, just disable the GameObject after a short delay
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Optional: Play death animation or effects
        yield return new WaitForSeconds(1f);
        
        // Destroy the enemy
        Destroy(gameObject);
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
