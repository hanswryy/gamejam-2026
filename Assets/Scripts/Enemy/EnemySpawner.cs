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

    float spawnTimer;

    Vector3 playerPosition;
    void Update()
    {
        OrbitAroundPlayer();
        StartSpawnEnemy();
    }

    void OrbitAroundPlayer() {
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
        Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(spawnInterval);
    }
}
