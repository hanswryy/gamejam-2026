using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float projectileLifetime = 2f;
    [SerializeField] float yPlayerOffset = 0;

    float lifetimeTimer;
    GameObject player;
    Vector3 playerPosition;

    void Start()
    {
        lifetimeTimer = projectileLifetime;
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform.position + new Vector3(0, yPlayerOffset, 0);
        transform.rotation = Quaternion.LookRotation(playerPosition - transform.position);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);

        if(lifetimeTimer <= 0) {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } else {
            lifetimeTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Boss Missile Collided with " + other.gameObject.name);
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Terrain")
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            other.gameObject.GetComponent<PlayerHealthManager>()?.TakeDamage(1, transform.position);
            Destroy(gameObject);
        }
    }
}
