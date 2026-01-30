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

    }

    private void Update()
    {
        player = GameObject.FindWithTag("Player");
        ProcessDirection();
    }

    void FixedUpdate()
    {
        ProcessRotation();
    }

    void ProcessDirection()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void ProcessRotation()
    {
        Vector3 targetPosition = player.transform.position - transform.position;
        targetPosition.y = 0; // Cap Y-axis rotation
        Quaternion rotation = Quaternion.LookRotation(targetPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
    }
}
