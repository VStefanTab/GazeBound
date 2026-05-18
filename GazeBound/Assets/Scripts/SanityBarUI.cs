using System.Collections;
using UnityEngine;

// Attach this script to the "Sanity" GameObject (the red child of SanityBar).
// It runs the fear/sanity logic itself and resizes its own RectTransform width.
public class SanityBarUI : MonoBehaviour
{
    [Header("Frightness Settings")]
    public float frightness = 0f;
    public float maxFrightness = 100f;

    [Header("Detection Settings")]
    public NPCController monster;
    public float detectionAngle = 30f;
    public float detectionRange = 20f;
    public Camera playerCamera;
    [Tooltip("Used as the source position for distance & line-of-sight. If empty, falls back to playerCamera.")]
    public Transform player;

    [Header("Bar Visual")]
    [Tooltip("Width of the Sanity fill (in pixels) when frightness is at max. Set this to the width of the SanityBar background.")]
    public float maxWidth = 200f;

    [Header("Game Over")]
    public AudioClip gameOverSound;
    [Range(0f, 1f)] public float gameOverVolume = 0.3f;
    public GameObject gameOverImage;

    [Header("Audio")]
    [Tooltip("If true, all game audio is muted at Start. It is briefly unmuted only while the jumpscare/game-over sound plays.")]
    public bool muteAllExceptJumpscare = true;

    private RectTransform _rect;
    private float tickTimer = 0f;
    private bool gameEnded = false;

    void Start()
    {
        _rect = GetComponent<RectTransform>();

        if (playerCamera == null) playerCamera = Camera.main;
        if (player == null && playerCamera != null) player = playerCamera.transform;

        frightness = 0f;
        if (gameOverImage != null) gameOverImage.SetActive(false);

        if (muteAllExceptJumpscare) AudioListener.volume = 0f;

        UpdateWidth();
    }

    void Update()
    {
        if (gameEnded) return;

        tickTimer += Time.deltaTime;

        Vector3 playerPos = player != null ? player.position : Vector3.zero;
        float distance = monster != null ? Vector3.Distance(playerPos, monster.transform.position) : detectionRange;
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

            UpdateWidth();

            if (frightness >= maxFrightness)
            {
                StartCoroutine(GameOver());
            }
        }
    }

    bool IsLookingAtMonster()
    {
        if (monster == null || playerCamera == null) return false;

        Vector3 directionToMonster = monster.transform.position - playerCamera.transform.position;
        float distance = directionToMonster.magnitude;
        if (distance > detectionRange) return false;

        float angle = Vector3.Angle(playerCamera.transform.forward, directionToMonster);
        return angle < detectionAngle;
    }

    void UpdateWidth()
    {
        if (_rect == null) return;
        float ratio = maxFrightness > 0f ? frightness / maxFrightness : 0f;
        Vector2 size = _rect.sizeDelta;
        size.x = maxWidth * ratio;
        _rect.sizeDelta = size;
    }

    IEnumerator GameOver()
    {
        gameEnded = true;
        if (gameOverImage != null) gameOverImage.SetActive(true);

        // Unmute just before playing the jumpscare sound
        if (muteAllExceptJumpscare) AudioListener.volume = 1f;

        if (gameOverSound != null && playerCamera != null)
            AudioSource.PlayClipAtPoint(gameOverSound, playerCamera.transform.position, gameOverVolume);

        yield return new WaitForSecondsRealtime(2f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}