using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 5f;
    public GameObject jumpscareImage;
    public GameObject blackScreen;
    public AudioClip jumpscareSound;                  // <-- was AudioSource
    [Range(0f, 1f)] public float jumpscareVolume = 1f; // bonus: volume control
    public float scareDuration = 1.5f;
    private AudioSource _audio;
    private bool _triggered = false;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_triggered) return;   // just stop processing once triggered
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist <= triggerDistance)
        {
            StartCoroutine(PlayJumpscare());
        }
    }

    IEnumerator PlayJumpscare()
    {
        _triggered = true;
        jumpscareImage.SetActive(true);
        _audio.PlayOneShot(jumpscareSound, jumpscareVolume);
        yield return new WaitForSeconds(scareDuration);
        jumpscareImage.SetActive(false);
        blackScreen.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);   // let player see the black screen
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}