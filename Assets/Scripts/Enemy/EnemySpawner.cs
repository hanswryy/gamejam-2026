using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject spawnEffect;
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] float inertia = 5f;
    [SerializeField] GameObject bossPrefab;

    float spawnTimer;

    Vector3 playerPosition;
    // Get kills and timer object
    KillsAndTimer kills;
    
    void Start() {
        kills = GameObject.FindWithTag("KillsAndTimer").GetComponent<KillsAndTimer>();
    }

    void Update()
    {
        OrbitAroundPlayer();
        StartSpawnEnemy();

        // Cheat key for spawning boss by set kills to 99
        if (Input.GetKeyDown(KeyCode.B)) {
            kills.spawnBoss();
        }

        if (kills.GetKills() >= 100) {
            Instantiate(bossPrefab, transform.position + new Vector3(0, 0, 10f), Quaternion.identity);
            // Stop spawning enemies
            this.enabled = false;
        }
    }

    void OrbitAroundPlayer() {
        if (GameObject.FindWithTag("Player") == null) return;
        playerPosition = GameObject.FindWithTag("Player").transform.position;
        //transform.RotateAround(playerPosition, Vector3.up, inertia * Time.deltaTime);
        transform.position = playerPosition;
        transform.Rotate(Vector3.up, inertia * Time.deltaTime);
    }
    void StartSpawnEnemy() {
        if (spawnTimer <= 0)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    void SpawnEnemy() {
        int spawnCount = Random.Range(0, spawnPoints.Length - 1);
        for (int i = 0; i < spawnCount; i++)
        {
            StartCoroutine(SpawnAtPoint(spawnPoints[i]));
        }
    }

    IEnumerator SpawnAtPoint(GameObject spawnPoint)
    {
        Instantiate(spawnEffect, spawnPoint.transform.position, Quaternion.identity);
        Instantiate(enemyPrefab, new Vector3(spawnPoint.transform.position.x, 1, spawnPoint.transform.position.z), Quaternion.identity);
        yield return new WaitForSeconds(spawnInterval);
    }
}
