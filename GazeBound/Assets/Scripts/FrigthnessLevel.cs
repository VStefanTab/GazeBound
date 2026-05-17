using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Remove this if you're not using TextMeshPro

public class FrightnessUI : MonoBehaviour
{
    [Header("References")]
    public Frightness frightnessScript;

    [Header("UI Elements")]
    public Slider frightnessSlider;
    public TextMeshProUGUI frightnessText; // Or use "public Text frightnessText;" for legacy UI

    void Start()
    {
        // Set slider range to match frightness settings
        if (frightnessSlider != null)
        {
            frightnessSlider.minValue = 0f;
            frightnessSlider.maxValue = frightnessScript.maxFrightness;
        }
    }

    void Update()
    {
        if (frightnessSlider != null)
            frightnessSlider.value = frightnessScript.frightness;

        if (frightnessText != null)
            frightnessText.text = "Frightness: " + Mathf.RoundToInt(frightnessScript.frightness) + "%";
    }
}