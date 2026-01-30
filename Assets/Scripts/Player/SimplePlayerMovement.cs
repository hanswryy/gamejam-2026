using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    
    float verticalDirection;
    float horizontalDirection;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        verticalDirection = Input.GetAxis("Vertical");
        horizontalDirection = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ProcessTranslation();
        }
    }

    void ProcessTranslation()
    {

    }
}
