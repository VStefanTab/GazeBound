using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frightness : MonoBehaviour
{
    [Header("Frightness Settings")]
    public float frightness = 0f;
    public float maxFrightness = 100f;

    [Header("Detection Settings")]
    public NPCController monster;
    public float detectionAngle = 30f;
    public float detectionRange = 20f;
    public float maxFrightDistance = 5f;
    public Camera playerCamera;

    [Header("Game Over")]
    public AudioClip gameOverSound;
    public GameObject gameOverImage;   // drag your UI Image GameObject here in the Inspector

    private float tickTimer = 0f;
    private float tickInterval = 0.5f;
    private bool gameEnded = false;

    void Start()
    {
        frightness = 0f;
        playerCamera = Camera.main;
        if (gameOverImage != null) gameOverImage.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;   // stop ticking once the game is over

        tickTimer += Time.deltaTime;
        float distance = monster != null ? Vector3.Distance(transform.position, monster.transform.position) : detectionRange;
        float dynamicInterval = Mathf.Lerp(0.05f, 0.5f, distance / detectionRange);

        if (tickTimer >= dynamicInterval)
        {
            tickTimer = 0f;
            if (IsLookingAtMonster())
            {
                float distanceRatio = 1f - Mathf.Clamp01(distance / detectionRange);
                float amount = Mathf.Lerp(5f, 100f, Mathf.Pow(distanceRatio, 2f));
                frightness += amount;
            }
            else
            {
                frightness -= Random.Range(2f, 5f);
            }
            frightness = Mathf.Clamp(frightness, 0f, maxFrightness);

            if (frightness >= maxFrightness)
            {
                StartCoroutine(GameOver());
            }
        }
    }

    bool IsLookingAtMonster()
    {
        if (monster == null) return false;
        Vector3 directionToMonster = monster.transform.position - playerCamera.transform.position;
        float distance = directionToMonster.magnitude;
        if (distance > detectionRange) return false;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToMonster);
        return angle < detectionAngle;
    }

    IEnumerator GameOver()
    {
        gameEnded = true;
        if (gameOverImage != null) gameOverImage.SetActive(true);
        if (gameOverSound != null) AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position, 0.3f);


        yield return new WaitForSecondsRealtime(2f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}