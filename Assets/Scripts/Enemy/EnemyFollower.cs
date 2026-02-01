using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed;
    public int damage;
    GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        ProcessDirection();
    }

    void FixedUpdate()
    {
        ProcessRotation();
    }

    void ProcessDirection()
    {
        if (player == null) return;
        
        // Create target position but keep current Y position
        Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    void ProcessRotation()
    {
        if (player == null) return;
        Vector3 targetPosition = player.transform.position - transform.position;
        targetPosition.y = 0; // Cap Y-axis rotation
        Quaternion rotation = Quaternion.LookRotation(targetPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
    }
}
