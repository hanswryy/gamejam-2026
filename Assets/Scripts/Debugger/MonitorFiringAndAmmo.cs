using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonitorFiringAndAmmo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI firingStatusText;
    [SerializeField] TextMeshProUGUI ammoCountText;
    GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.LogError("Player object not found!");
            return;
        }

        RangedSystem rangedSystem = player.GetComponent<RangedSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAmmo();
    }

    void SetAmmo() {
        if (player == null) return;
        RangedSystem rangedSystem = player.GetComponent<RangedSystem>();
        if (rangedSystem == null) {
            Debug.LogError("RangedSystem component not found on Player!");
            return;
        }
        ammoCountText.text = "Ammo: " + rangedSystem.GetAmmo().ToString();
        if (rangedSystem.CanFire()) {
            firingStatusText.text = "Firing: READY";
            firingStatusText.color = Color.green;
        } else {
            firingStatusText.text = "Firing: RELOADING";
            firingStatusText.color = Color.red;
        }
    }
}
