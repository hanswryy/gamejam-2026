using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput, mouseLookInput;
    private Vector3 rotationTarget;
    public float speed;
    
    private Animator animator;

    public void OnMove(InputAction.CallbackContext context){
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context){
        mouseLookInput = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayerAndAim();
    }

    public void movePlayerAndAim(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouseLookInput);
        if(Physics.Raycast(ray, out hit)){
            rotationTarget = hit.point;
        }

        PlayerHealthManager playerHealth = GetComponent<PlayerHealthManager>();
        if (playerHealth != null && playerHealth.IsKnockedBack())
            return;

        var lookPos = rotationTarget - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);

        Vector3 aimDirection = new Vector3(rotationTarget.x, 0f, rotationTarget.z);
        if (aimDirection != Vector3.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        // Transform movement input relative to camera
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        // Debug.Log("Move Input: " + moveInput);

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        
        // Get camera's forward and right directions, but keep them on the horizontal plane
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Transform the movement relative to camera orientation
        Vector3 relativeMovement = cameraRight * movement.x + cameraForward * movement.z;

        transform.Translate(relativeMovement * Time.deltaTime * speed, Space.World);
    }
}
