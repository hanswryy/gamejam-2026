using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignHealthUI : MonoBehaviour
{
    [Header("Health UI Settings")]
    [SerializeField] GameObject healthUI;              // Reference to the first health icon
    [SerializeField] float offsetPerHealth = 10f;      // Pixel offset between each icon
    [SerializeField] int maxHealth = 5;                // Maximum health (number of icons)

    [Header("Sprite References")]
    [SerializeField] Sprite fullHealthSprite;          // Sprite for full health
    [SerializeField] Sprite lostHealthSprite;          // Sprite for lost health (damaged state)

    GameObject player;
    int playerMaxHealth;

    private List<GameObject> healthIcons = new List<GameObject>();

    void Start()
    {
        InitializeHealthUI();
        UpdateHealthUI(playerMaxHealth);
    }

    private void Update()
    {
        if (player) UpdateHealthUI(player.GetComponent<PlayerHealthManager>().currentHealth);
    }

    /// <summary>
    /// Creates all health icons based on maxHealth value
    /// </summary>
    void InitializeHealthUI()
    {
        player = GameObject.FindWithTag("Player");
        playerMaxHealth = player.GetComponent<PlayerHealthManager>().maxHealth;
        maxHealth = playerMaxHealth;
        healthIcons.Clear();

        if (healthUI == null || player == null)
        {
            Debug.LogError("[AssignHealthUI] healthUI is not assigned in the Inspector!");
            return;
        }

        RectTransform originalRect = healthUI.GetComponent<RectTransform>();

        if (originalRect == null)
        {
            Debug.LogError("[AssignHealthUI] healthUI must have a RectTransform component!");
            return;
        }

        healthIcons.Add(healthUI);

        if (fullHealthSprite != null)
        {
            Image originalImage = healthUI.GetComponent<Image>();
            if (originalImage != null)
            {
                originalImage.sprite = fullHealthSprite;
            }
        }

        Transform parentTransform = healthUI.transform.parent;

        Vector2 basePosition = originalRect.anchoredPosition;

        for (int i = 0; i < maxHealth - 1; i++)
        {
            Vector2 newPosition = new Vector2(
                basePosition.x + (offsetPerHealth * (i + 1)),
                basePosition.y
            );

            GameObject newHealthIcon = Instantiate(healthUI, parentTransform);

            RectTransform newRect = newHealthIcon.GetComponent<RectTransform>();

            newRect.anchoredPosition = newPosition;

            if (fullHealthSprite != null)
            {
                Image newImage = newHealthIcon.GetComponent<Image>();
                if (newImage != null)
                {
                    newImage.sprite = fullHealthSprite;
                }
            }

            healthIcons.Add(newHealthIcon);
        }

        Debug.Log($"[AssignHealthUI] Initialized {healthIcons.Count} health icons");
    }

    // ============================================
    // PUBLIC METHODS (For Health Management)
    // ============================================

    /// <summary>
    /// Updates UI to show current health value
    /// Call this when player takes damage or heals
    /// </summary>
    /// <param name="currentHealth">Current health value (1 to maxHealth)</param>
    public void UpdateHealthUI(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        for (int i = 0; i < healthIcons.Count; i++)
        {
            if (healthIcons[i] != null)
            {
                Image iconImage = healthIcons[i].GetComponent<Image>();

                if (iconImage != null)
                {
                    if (i < currentHealth)
                    {
                        iconImage.sprite = fullHealthSprite;
                        iconImage.enabled = true;
                    }
                    else
                    {
                        if (lostHealthSprite != null)
                        {
                            iconImage.sprite = lostHealthSprite;
                        }
                        else
                        {
                            iconImage.enabled = false;
                        }
                    }
                }
            }
        }

        Debug.Log($"[AssignHealthUI] Updated UI to show {currentHealth}/{maxHealth} health");
    }

    /// <summary>
    /// Destroys a specific number of health icons from the right
    /// Use this for permanent health loss
    /// </summary>
    /// <param name="amount">Number of icons to destroy</param>
    public void DestroyHealthIcons(int amount)
    {
        amount = Mathf.Clamp(amount, 0, healthIcons.Count);
        for (int i = 0; i < amount; i++)
        {
            int lastIndex = healthIcons.Count - 1;

            if (lastIndex >= 0 && healthIcons[lastIndex] != null)
            {
                Destroy(healthIcons[lastIndex]);
                healthIcons.RemoveAt(lastIndex);
            }
        }

        Debug.Log($"[AssignHealthUI] Destroyed {amount} health icons. Remaining: {healthIcons.Count}");
    }

    /// <summary>
    /// Resets all health icons to full health state
    /// </summary>
    public void ResetHealthUI()
    {
        UpdateHealthUI(maxHealth);
    }

    /// <summary>
    /// Returns the current number of health icons
    /// </summary>
    public int GetHealthIconCount()
    {
        return healthIcons.Count;
    }

    /// <summary>
    /// Gets reference to a specific health icon by index
    /// </summary>
    /// <param name="index">Index of the icon (0 = leftmost)</param>
    public GameObject GetHealthIcon(int index)
    {
        if (index >= 0 && index < healthIcons.Count)
        {
            return healthIcons[index];
        }

        Debug.LogWarning($"[AssignHealthUI] Invalid index {index}. Valid range: 0-{healthIcons.Count - 1}");
        return null;
    }
}