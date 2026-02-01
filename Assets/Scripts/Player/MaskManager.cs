using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskManager : MonoBehaviour
{
    [Header("Combat Systems")]
    [SerializeField] private SwordSystem swordSystem;
    [SerializeField] private RangedSystem rangedSystem;

    [Header("State Management")]
    [SerializeField] private bool isGunState = false;
    private Animator animator;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference switchWeaponAction;

    [Header("Weapon GameObjects")]
    [SerializeField] private GameObject hitArea;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject gunObject;
    
    [Header("Audio/Visual Feedback")]
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private float switchCooldown = 0.3f;
    [SerializeField] GameObject canvasManager;
    private AssignHealthUI assignHealthUI;
    private float lastSwitchTime;
    private int weaponIndex = 0; // 0 = sword, 1 = gun

    [Header("Ammo Management")]
    private int savedAmmoCount = -1; // -1 means not initialized

    private void Awake()
    {
        // Get animator component
        animator = GetComponent<Animator>();

        // Get system components if not assigned
        if (swordSystem == null)
            swordSystem = GetComponent<SwordSystem>();
        if (rangedSystem == null)
            rangedSystem = GetComponent<RangedSystem>();

        assignHealthUI = canvasManager.GetComponent<AssignHealthUI>();
    }

    private void OnEnable()
    {
        if (switchWeaponAction != null)
        {
            switchWeaponAction.action.performed += OnSwitchWeapon;
            switchWeaponAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (switchWeaponAction != null)
        {
            switchWeaponAction.action.performed -= OnSwitchWeapon;
            switchWeaponAction.action.Disable();
        }
    }

    void Start()
    {
        // Initialize to sword state
        SetCombatState(false);

        // Validate components
        if (swordSystem == null)
            Debug.LogError("SwordSystem component not found on " + gameObject.name);
        if (rangedSystem == null)
            Debug.LogError("RangedSystem component not found on " + gameObject.name);
        if (animator == null)
            Debug.LogError("Animator component not found on " + gameObject.name);

        // Validate gameobject references
        if (hitArea == null)
            Debug.LogWarning("HitArea GameObject not assigned to " + gameObject.name);
        if (swordObject == null)
            Debug.LogWarning("Sword GameObject not assigned to " + gameObject.name);
    }

    void Update()
    {
        // Fallback input handling if InputAction is not set up
        if (switchWeaponAction == null && Input.GetKeyDown(KeyCode.Q))
        {
            UpdateWeaponUI(isGunState);
            if (CanSwitchWeapon())
            {
                SwitchWeapon();
            }
        }
    }

    void UpdateWeaponUI(bool isGun)
    {
        if (isGun)
        {
            Debug.Log("Updating weapon UI to Gun");
            assignHealthUI.UpdateWeaponUsage(1);
        }
        else if (!isGun)
        {
            Debug.Log("Updating weapon UI to Sword");
            assignHealthUI.UpdateWeaponUsage(0);
        }
    }

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.performed && CanSwitchWeapon())
        {
            SwitchWeapon();
        }
    }

    private bool CanSwitchWeapon()
    {
        // Check cooldown
        if (Time.time - lastSwitchTime < switchCooldown)
            return false;

        // Prevent switching during sword combo attacks
        if (!isGunState && swordSystem != null)
        {
            // Check if currently in an attack animation or combo
            if (animator != null)
            {
                int attackCount = animator.GetInteger("AttackCount");
                if (attackCount > 0)
                {
                    Debug.Log("Cannot switch weapons during sword combo");
                    return false;
                }

                // Check if currently in attack animation state
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                if (currentState.IsName("swd_combo1") || currentState.IsName("swd_combo2") || currentState.IsName("swd_combo3"))
                {
                    if (currentState.normalizedTime < 0.8f) // Allow switching near end of animation
                    {
                        Debug.Log("Cannot switch weapons during attack animation");
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void SwitchWeapon()
    {
        isGunState = !isGunState;
        SetCombatState(isGunState);
        lastSwitchTime = Time.time;

        // Play switch sound if available
        if (switchSound != null && GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().PlayOneShot(switchSound);
        }

        if (isGunState)
        {
            assignHealthUI.UpdateWeaponUsage(1);
        }
        else
        {
            assignHealthUI.UpdateWeaponUsage(0);
        }
        ;

        Debug.Log($"Switched to {(isGunState ? "Ranged" : "Sword")} mode");
    }

    private void SetCombatState(bool gunState)
    {
        // Save current ammo before switching away from gun state
        if (isGunState && !gunState && rangedSystem != null)
        {
            savedAmmoCount = rangedSystem.GetAmmo();
            Debug.Log($"Saved ammo count: {savedAmmoCount}");
        }

        // Reset sword combo and animator when switching away from sword state
        if (!isGunState && gunState && swordSystem != null)
        {
            swordSystem.ResetCombo();

            // Force reset animator parameters
            if (animator != null)
            {
                animator.SetInteger("AttackCount", 0);
                animator.ResetTrigger("Attack"); // Reset any attack triggers that might exist
            }

            Debug.Log("Reset sword combo when switching to gun");
        }

        isGunState = gunState;

        // Update animator parameter
        if (animator != null)
        {
            animator.SetBool("isGunState", isGunState);
        }

        // Enable/disable appropriate systems
        if (swordSystem != null)
            swordSystem.enabled = !isGunState;

        if (rangedSystem != null)
            rangedSystem.enabled = isGunState;

        // Show/hide sword-related gameobjects
        if (hitArea != null)
            hitArea.SetActive(!isGunState);

        if (swordObject != null)
            swordObject.SetActive(!isGunState);
            
        // Show/hide gun-related gameobjects
        if (gunObject != null)
            gunObject.SetActive(isGunState);
            
        // Restore saved ammo when switching to gun state
        if (isGunState && savedAmmoCount >= 0 && rangedSystem != null)
        {
            StartCoroutine(RestoreAmmoAfterFrame());
        }

        // Reset any ongoing attacks/combos when switching to sword (keep existing logic)
        if (!isGunState && swordSystem != null)
        {
            swordSystem.ResetCombo();
        }
    }

    private IEnumerator RestoreAmmoAfterFrame()
    {
        // Wait one frame to ensure RangedSystem is fully enabled
        yield return null;

        if (rangedSystem != null && savedAmmoCount >= 0)
        {
            // Access the private ammo field through reflection or add a public setter
            var rangedSystemType = rangedSystem.GetType();
            var ammoField = rangedSystemType.GetField("ammo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (ammoField != null)
            {
                ammoField.SetValue(rangedSystem, savedAmmoCount);
                Debug.Log($"Restored ammo count: {savedAmmoCount}");
            }
            else
            {
                Debug.LogWarning("Could not find ammo field in RangedSystem to restore ammo count");
            }
        }
    }

    // Public getters for other scripts to check current state
    public bool IsInGunMode() => isGunState;
    public bool IsInSwordMode() => !isGunState;
    public SwordSystem GetSwordSystem() => swordSystem;
    public RangedSystem GetRangedSystem() => rangedSystem;

    // Force switch to specific mode (useful for cutscenes, pickups, etc.)
    public void ForceGunMode()
    {
        if (!isGunState)
            ForceSwitch(true);
    }

    public void ForceSwordMode()
    {
        if (isGunState)
            ForceSwitch(false);
    }

    // Emergency force switch that bypasses all restrictions
    private void ForceSwitch(bool toGunState)
    {
        // Force reset everything before switching
        if (swordSystem != null)
        {
            swordSystem.ResetCombo();
        }

        if (animator != null)
        {
            animator.SetInteger("AttackCount", 0);
            animator.ResetTrigger("Attack");
        }

        isGunState = toGunState;
        SetCombatState(isGunState);
        lastSwitchTime = Time.time;

        Debug.Log($"Force switched to {(isGunState ? "Ranged" : "Sword")} mode");
    }

    // Get current weapon name for UI display
    public string GetCurrentWeaponName()
    {
        return isGunState ? "Gun" : "Sword";
    }

    // Ammo management methods
    public int GetCurrentAmmo()
    {
        if (isGunState && rangedSystem != null)
            return rangedSystem.GetAmmo();
        return savedAmmoCount;
    }

    public int GetSavedAmmo()
    {
        return savedAmmoCount;
    }

    public void ResetSavedAmmo()
    {
        savedAmmoCount = -1;
    }
}
