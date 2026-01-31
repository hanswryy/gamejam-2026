using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedSystem : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1f)] float timeBetweenShots = 0.5f;
    [SerializeField] int ammoAmount = 10;
    [SerializeField] float reloadTimer = 2f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float damage = 15f; // Damage per shot
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject hitParticles;

    float reloadTime;
    float shotTimer;
    RaycastHit hit;
    bool isHit;
    bool canShoot = true;
    int ammo;

    void Start()
    {
        shotTimer = timeBetweenShots;
        reloadTime = reloadTimer;
        ammo = ammoAmount;
    }

    void Update()
    {
        if (ammo <= 0) {
            canShoot = false;
            Reloading();
        }

        // Countdown the shot timer
        if (shotTimer > 0) {
            shotTimer -= Time.deltaTime;
        }

        // Check for mouse input and fire if ready
        if (Mouse.current.leftButton.isPressed && canShoot && shotTimer <= 0) {
            Firing();
            shotTimer = timeBetweenShots; // Reset timer after firing
        }
    }

    void FixedUpdate()
    {
        DrawRaycast();
    }

    void DrawRaycast() {
        isHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, enemyLayer);

        if (isHit && Mouse.current.leftButton.isPressed) return;

        if (isHit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxDistance, Color.blue);
            // Debug.Log("Did not Hit");
        }
    }

    void Firing() {
        if (isHit)
        {
            // To Do :
            // Play shooting sound
            // Play shooting animation

            // Instantiate bullet impact effect at hit.point
            GameObject enemy = hit.collider.gameObject;
            Instantiate(hitParticles, enemy.transform.position, Quaternion.identity);

            // Deal damage to the enemy hit
            DealDamageToEnemy(hit.collider);

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
        }
        
        // Always consume ammo and show feedback when firing
        ammo--;
        Debug.Log("Bang!");
        Debug.Log($"Ammo left: {ammo}");
    }
    
    void DealDamageToEnemy(Collider enemy) {
        var enemyHealthManager = enemy.GetComponent<EnemyHealthManager>();
        if (enemyHealthManager != null) {
            enemyHealthManager.TakeDamage(damage, transform.position);
            Debug.Log($"Shot {enemy.name} for {damage} damage");
        }
        
        // Add visual feedback here if needed (muzzle flash, screen shake, etc.)
    }

    void Reloading() { 
        if (reloadTime <= 0)
        {
            // Debug.Log("Reloading...");
            canShoot = true;
            reloadTime = reloadTimer;
            ammo = ammoAmount;
        }
        else { 
            reloadTime -= Time.deltaTime;
        }
    }

    public int GetAmmo() {
        return ammo;
    }

    public bool CanFire() {
        return canShoot;
    }
}
