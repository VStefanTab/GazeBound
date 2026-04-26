using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour {

    public Transform player;
    public float triggerDistance = 5f;

    public GameObject jumpscareImage;
    public GameObject blackScreen;
    public AudioClip jumpscareSound;
    public float scareDuration = 1.5f;

    private AudioSource _audio;
    private bool _triggered = false;

    void Start(){
        _audio = GetComponent<AudioSource>();
    }

    void Update(){
        if (_triggered) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= triggerDistance){
            StartCoroutine(PlayJumpscare());
        }
    }

    IEnumerator PlayJumpscare(){
        _triggered = true;

        jumpscareImage.SetActive(true);
        _audio.PlayOneShot(jumpscareSound);

        yield return new WaitForSeconds(scareDuration);

        jumpscareImage.SetActive(false);
        blackScreen.SetActive(true);

        Time.timeScale = 0f;
    }
}