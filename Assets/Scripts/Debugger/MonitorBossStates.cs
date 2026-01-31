using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonitorBossStates : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bossStatesText;
    [SerializeField] GameObject boss;
    BossAction bossAction;
    // Start is called before the first frame update
    void Start()
    {
        bossAction = boss.GetComponent<BossAction>();
    }

    // Update is called once per frame
    void Update()
    {
        string states = bossAction.GetBossState().ToString();
        bossStatesText.text = states;
    }
}
