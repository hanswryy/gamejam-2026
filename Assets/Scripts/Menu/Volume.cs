using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Volume : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeText;

    void Update()
    {
        float value = Mathf.RoundToInt(volumeSlider.value * 100);
        volumeText.text = string.Format("{0}%", value);
    }
}
