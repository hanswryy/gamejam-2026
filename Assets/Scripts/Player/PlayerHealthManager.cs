using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("Damage & Knockback")]
    public float invulnerabilityDuration = 1.5f;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    
    [Header("Visual Feedback")]
    public float flashDuration = 0.1f;
    
    private bool isInvulnerable = false;
    private bool isKnockedBack = false;
    private Vector3 knockbackVelocity;
    private Renderer playerRenderer;
    private Color originalColor;
    private PlayerController playerController;

    private KillsAndTimer killsAndTimer;
    
    void Start()
    {
        currentHealth = maxHealth;
        playerRenderer = GetComponent<Renderer>();
        playerController = GetComponent<PlayerController>();
        killsAndTimer = FindObjectOfType<KillsAndTimer>();

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;
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
    
    public void TakeDamage(int damage, Vector3 damageSource)
    {
        if (isInvulnerable) return;
        
        currentHealth -= damage;
        
        Vector3 knockbackDirection = (transform.position - damageSource).normalized;
        knockbackDirection.y = 0f;
        knockbackVelocity = knockbackDirection * knockbackForce;
        
        StartCoroutine(InvulnerabilityFrames());
        StartCoroutine(KnockbackEffect());
        StartCoroutine(DamageFlash());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator KnockbackEffect()
    {
        isKnockedBack = true;
        
        // Disable player movement temporarily
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        yield return new WaitForSeconds(knockbackDuration);
        
        // Bring back player control
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        isKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }
    
    private IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        
        // Flicker effect biar bling-bling
        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            if (playerRenderer != null)
            {
                playerRenderer.enabled = !playerRenderer.enabled;
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        
        if (playerRenderer != null)
        {
            playerRenderer.enabled = true;
        }
        
        isInvulnerable = false;
    }
    
    private IEnumerator DamageFlash()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            playerRenderer.material.color = originalColor;
        }
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // Add death effects here (particles, sound, etc.)
        
        // Disable player components
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
            playerCollider.enabled = false;
            
        if (playerController != null)
            playerController.enabled = false;
        
        // You can add death animation here or destroy the enemy
        // For now, just disable the GameObject after a short delay
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Optional: Play death animation or effects
        yield return new WaitForSeconds(1f);

        killsAndTimer.playerLose = true;
        
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

    private int damageTaken;

    void OnTriggerEnter(Collider other)
    {
        // Example: Take damage when colliding with an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, other.transform.position);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, transform.position - transform.forward);
        }
    }
}