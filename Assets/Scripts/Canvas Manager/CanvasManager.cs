using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Canvas menuCanvas;
    [SerializeField] Button[] menuButtons;

    CanvasGroup menuCanvasGroup;
    void Start()
    {
        menuCanvasGroup = menuCanvas.GetComponent<CanvasGroup>();
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
