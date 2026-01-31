using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsAndTimer : MonoBehaviour
{
    float timer = 0f;
    public float minutes { get; private set; }
    public float seconds { get; private set; }
    public int kills { get; private set; }
    public bool bossDied { get; set; } = false;
    public bool playerLose { get; set; } = false;
    [SerializeField] private GameOverManager gameOverManager;

    void Start()
    {
        timer = 0f;
        kills = 0;
        bossDied = false;
        playerLose = false;
    }

    void Update()
    {
        RunTimer();
        if (bossDied || playerLose) {
            gameOverManager.ShowGameOverScreen();
        }
    }

    void RunTimer() { 
        timer += Time.deltaTime;
        TimeFormatter();
    }

    void TimeFormatter() { 
        minutes = Mathf.FloorToInt(timer / 60);
        seconds = Mathf.FloorToInt(timer % 60);
    }

    public float GetMinutes() {
        return minutes;
    }

    public float GetSeconds()
    {
        return seconds;
    }

    public void AddKill() {
        kills += 1;
    }

    public int GetKills() {
        return kills;
    }

    public void spawnBoss() {
        kills = 99;
    }
}
