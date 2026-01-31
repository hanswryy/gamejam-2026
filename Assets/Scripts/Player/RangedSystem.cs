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

        if (shotTimer <= 0 && canShoot)
        {
            Firing();
            shotTimer = timeBetweenShots;
        }
        else
        {
            shotTimer -= Time.deltaTime;
        }

        //Debug.Log($"Reload Time : {reloadTime}");
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
        if (isHit && Mouse.current.leftButton.isPressed)
        {
            // To Do :
            // Play shooting sound
            // Play shooting animation

            // Instantiate bullet impact effect at hit.point
            GameObject enemy = hit.collider.gameObject;
            Instantiate(hitParticles, enemy.transform.position, Quaternion.identity);

            // Deal damage to the enemy hit

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            ammo--;
        }
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
