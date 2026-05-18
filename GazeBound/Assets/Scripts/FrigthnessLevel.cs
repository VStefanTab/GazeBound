using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FrightnessUI : MonoBehaviour
{
    public GameObject gameOverImage;
    [Header("References")]
    public Frightness frightnessScript;

    [Header("UI Elements")]
    public TextMeshProUGUI frightnessText;

    [Header("Game Over Settings")]
    public string gameOverScene = "GameOver"; // Name of your Game Over scene

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        // Non-linear increase: squaring the ratio makes low levels feel safe,
        // but high levels escalate rapidly
        float ratio = frightnessScript.frightness / frightnessScript.maxFrightness;
        float nonLinearValue = Mathf.Pow(ratio, 2f) * frightnessScript.maxFrightness;

        if (frightnessText != null)
            frightnessText.text = "Frightness: " + Mathf.RoundToInt(nonLinearValue) + "%";

        if (frightnessScript.frightness >= frightnessScript.maxFrightness)
            StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        gameEnded = true;
        frightnessText.text = "You were too scared...";
        yield return new WaitForSeconds(2f);
        gameOverImage.SetActive(true);
    }
}