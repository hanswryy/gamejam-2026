using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject canvasManager;

    AssignHealthUI assignHealthUI;
    CanvasManager canvasManagerScript;
    private void Start()
    {
        assignHealthUI = canvasManager.GetComponent<AssignHealthUI>();
        canvasManagerScript = canvasManager.GetComponent<CanvasManager>();
    }
    public void ShowGameOverScreen() {
        this.gameObject.SetActive(true);
        assignHealthUI.ResetHealthUI();
        canvasManagerScript.HideHUD();
        Time.timeScale = 0f;
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
