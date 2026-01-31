using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsAndTimer : MonoBehaviour
{
    float timer = 0f;
    public float minutes { get; private set; }
    public float seconds { get; private set; }
    public int kills { get; private set; }
    void Start()
    {
        timer = 0f;
        kills = 0;
    }

    void Update()
    {
        RunTimer();
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
}
