using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonitorKillsAndTime : MonoBehaviour
{
    [SerializeField] GameObject killsAndTimerManager;
    [SerializeField] TextMeshProUGUI timesTMP;
    [SerializeField] TextMeshProUGUI killsTMP;

    float minutes;
    float seconds;
    float kills;

    void Update()
    {
        ProcessTime();
        ProcessKills();
    }

    void ProcessTime() { 
        minutes = killsAndTimerManager.GetComponent<KillsAndTimer>().GetMinutes();
        seconds = killsAndTimerManager.GetComponent<KillsAndTimer>().GetSeconds();
        SetTime(minutes, seconds);
    }

    void SetTime(float mins, float secs) {
        string formattedTime = string.Format("Times: {0:00}:{1:00}", mins, secs);
        timesTMP.text = formattedTime;
    }

    void ProcessKills() { 
        kills = killsAndTimerManager.GetComponent<KillsAndTimer>().GetKills();
        SetKills();
    }

    void SetKills() { 
        string formattedKills = string.Format("Kills: {0}", kills);
        killsTMP.text = formattedKills;
    }
}
