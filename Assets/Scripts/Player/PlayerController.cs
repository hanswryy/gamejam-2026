using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput, mouseLookInput;
    private Vector3 rotationTarget;
    public float speed;

    public void OnMove(InputAction.CallbackContext context){
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context){
        mouseLookInput = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouseLookInput);
        if(Physics.Raycast(ray, out hit)){
            rotationTarget = hit.point;
        }
        movePlayerAndAim();
    }

    public void movePlayerAndAim(){
        var lookPos = rotationTarget - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);

        Vector3 aimDirection = new Vector3(rotationTarget.x, 0f, rotationTarget.z);
        if (aimDirection != Vector3.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        transform.Translate(movement * Time.deltaTime * speed, Space.World);
    }
}
