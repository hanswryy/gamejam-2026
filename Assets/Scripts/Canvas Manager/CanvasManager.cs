using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Canvas menuCanvas;
    [SerializeField] Canvas hudCanvas;
    [SerializeField] Button[] menuButtons;

    CanvasGroup menuCanvasGroup;
    CanvasGroup hudCanvasGroup;
    void Start()
    {
        menuCanvasGroup = menuCanvas.GetComponent<CanvasGroup>();
        hudCanvasGroup = hudCanvas.GetComponent<CanvasGroup>();
        hudCanvasGroup.alpha = 0;
        Time.timeScale = 0;
    }

    void Update()
    {
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && menuCanvasGroup.alpha == 0)
        {

            foreach (Button button in menuButtons)
            {
                if (EventSystem.current.currentSelectedGameObject == button.gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
            menuCanvasGroup.alpha = 1;
            hudCanvasGroup.alpha = 0;
            Time.timeScale = 0;
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play Game");
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        menuCanvasGroup.alpha = 0;
        hudCanvasGroup.alpha = 1;
    }

    public void HideHUD() { 
        Debug.Log("Hide HUD");
        if(hudCanvasGroup.alpha == 1) hudCanvasGroup.alpha = 0;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
