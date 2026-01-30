using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RangedSystem : MonoBehaviour
{
    [SerializeField] float maxDistance = 10f;
    [SerializeField] LayerMask enemyLayer;

    RaycastHit hit;
    bool isHit;

    void Update()
    {
        Firing();
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
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxDistance, Color.blue);
            Debug.Log("Did not Hit");
        }
    }

    void Firing() {
        if (isHit && Mouse.current.leftButton.isPressed)
        {
            // To Do :
            // Play shooting sound
            // Play shooting animation
            // Instantiate bullet impact effect at hit.point
            // Deal damage to the enemy hit

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            Debug.Log("Pew Pew");
        }
    }
}
