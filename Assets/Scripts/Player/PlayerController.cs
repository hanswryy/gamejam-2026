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
    
    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    
    private bool isDashing = false;
    private bool canDash = true;
    
    private Collider[] playerColliders;
    private Rigidbody playerRigidbody;
    
    [Header("Dash Visual Effects")]
    public float dashAlpha = 0.3f; // How transparent the player becomes during dash
    
    private Renderer[] playerRenderers;
    private Material[] originalMaterials;
    private Material[] dashMaterials;

    public void OnMove(InputAction.CallbackContext context){
        if (!isDashing) // Only accept move input when not dashing
            moveInput = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context){
        if (!isDashing) // Only accept mouse input when not dashing
            mouseLookInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context){
        if(context.performed && canDash && !isDashing){
            StartCoroutine(PerformDash());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerColliders = GetComponentsInChildren<Collider>();
        playerRigidbody = GetComponent<Rigidbody>();
        
        // Setup renderers for dash transparency
        playerRenderers = GetComponentsInChildren<Renderer>();
        SetupDashMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing) // Only allow normal movement when not dashing
            movePlayerAndAim();
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        
        // Disable collision and physics immediately
        SetCollisionEnabled(false);
        SetPlayerTransparency(true);
        
        bool wasKinematic = false;
        if (playerRigidbody != null)
        {
            wasKinematic = playerRigidbody.isKinematic;
            playerRigidbody.isKinematic = true;
        }
        
        Vector2 dashInput = moveInput;
        Vector3 dashDirection;

        if (dashInput == Vector2.zero)
        {
            // If no input, dash in the direction the player is facing
            dashDirection = transform.forward;
        }
        else
        {
            // Transform dash input relative to camera (same as normal movement)
            Vector3 movement = new Vector3(dashInput.x, 0f, dashInput.y);
            
            // Get camera's forward and right directions, but keep them on the horizontal plane
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            // Transform the movement relative to camera orientation
            dashDirection = (cameraRight * movement.x + cameraForward * movement.z).normalized;
        }
        
        Vector3 startPosition = transform.position;
        Vector3 dashTarget = startPosition + dashDirection * dashDistance;

        // Remove raycast check - we want to phase through everything during dash

        // Smooth dash movement over time
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, dashTarget, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = dashTarget; // Ensure we reach the exact target
        
        // Re-enable collision and restore physics
        SetCollisionEnabled(true);
        SetPlayerTransparency(false);
        
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = wasKinematic;
        }
        
        isDashing = false;
        
        // Start cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private void SetCollisionEnabled(bool enabled)
    {
        foreach (Collider col in playerColliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }
    
    private void SetupDashMaterials()
    {
        if (playerRenderers == null || playerRenderers.Length == 0)
            return;
            
        originalMaterials = new Material[playerRenderers.Length];
        dashMaterials = new Material[playerRenderers.Length];
        
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null && playerRenderers[i].material != null)
            {
                originalMaterials[i] = playerRenderers[i].material;
                
                // Create a copy of the material for dash effect
                dashMaterials[i] = new Material(originalMaterials[i]);
                
                // Set up the material for transparency
                if (dashMaterials[i].HasProperty("_Mode"))
                {
                    dashMaterials[i].SetFloat("_Mode", 3); // Set to Transparent mode
                }
                if (dashMaterials[i].HasProperty("_SrcBlend"))
                {
                    dashMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                }
                if (dashMaterials[i].HasProperty("_DstBlend"))
                {
                    dashMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                if (dashMaterials[i].HasProperty("_ZWrite"))
                {
                    dashMaterials[i].SetInt("_ZWrite", 0);
                }
                
                dashMaterials[i].DisableKeyword("_ALPHATEST_ON");
                dashMaterials[i].DisableKeyword("_ALPHABLEND_ON");
                dashMaterials[i].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                dashMaterials[i].renderQueue = 3000;
                
                // Set the alpha
                if (dashMaterials[i].HasProperty("_Color"))
                {
                    Color color = dashMaterials[i].color;
                    color.a = dashAlpha;
                    dashMaterials[i].color = color;
                }
            }
        }
    }
    
    private void SetPlayerTransparency(bool transparent)
    {
        if (playerRenderers == null || originalMaterials == null || dashMaterials == null)
            return;
            
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                if (transparent && dashMaterials[i] != null)
                {
                    playerRenderers[i].material = dashMaterials[i];
                }
                else if (!transparent && originalMaterials[i] != null)
                {
                    playerRenderers[i].material = originalMaterials[i];
                }
            }
        }
    }

    public void movePlayerAndAim(){
        // Block all movement and aiming during dash
        if (isDashing)
            return;
            
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
        
        // Get camera's forward and right directions, but keep them on the horizontal plane
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Transform the movement relative to camera orientation
        Vector3 relativeMovement = cameraRight * movement.x + cameraForward * movement.z;

        // Check for collision before moving
        Vector3 proposedMovement = relativeMovement * Time.deltaTime * speed;
        if (!WillCollideWithWalls(proposedMovement))
        {
            transform.Translate(proposedMovement, Space.World);
        }

        Vector3 playerForward = transform.forward;
        Vector3 playerRight = transform.right;
        
        // Project the relative movement onto the player's local axes
        float forwardMovement = Vector3.Dot(relativeMovement.normalized, playerForward);
        float rightMovement = Vector3.Dot(relativeMovement.normalized, playerRight);
        
        // Set animator parameters based on movement relative to player's facing direction
        animator.SetFloat("Horizontal", rightMovement);
        animator.SetFloat("Vertical", forwardMovement);
    }
    
    private bool WillCollideWithWalls(Vector3 movement)
    {
        float checkDistance = movement.magnitude + 0.1f;
        Vector3 direction = movement.normalized;
        
        // Cast from multiple points around the player
        Vector3[] checkPoints = {
            transform.position,                              // Center
            transform.position + Vector3.right * 0.4f,     // Right
            transform.position + Vector3.left * 0.4f,      // Left
            transform.position + Vector3.forward * 0.4f,   // Forward
            transform.position + Vector3.back * 0.4f       // Back
        };
        
        foreach (Vector3 point in checkPoints)
        {
            if (Physics.Raycast(point, direction, checkDistance, LayerMask.GetMask("Wall")))
            {
                return true; // Will collide
            }
        }
        
        return false;
    }
}
