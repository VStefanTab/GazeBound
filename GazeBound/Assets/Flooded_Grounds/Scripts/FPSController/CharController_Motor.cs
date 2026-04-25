using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour {

	public float speed = 10.0f;
	public float sensitivity = 30.0f;
	public float WaterHeight = 15.5f;
	CharacterController character;
	public GameObject cam;
	float moveFB, moveLR;
	float rotX, rotY;
	public bool webGLRightClickRotation = true;
	float gravity = -9.8f;

	// --- Distance Music ---
	[System.Serializable]
	public class DistanceZone {
		public float maxDistance;
		public AudioClip music;
		[Range(0f, 1f)] public float volume = 1f;
	}

	public Transform musicTarget;      // drag your target object here
	public DistanceZone[] zones;       // set up zones in Inspector
	private AudioSource _audio;
	private int _currentZone = -1;
	// ----------------------

	void Start(){
		character = GetComponent<CharacterController>();
		_audio = GetComponent<AudioSource>();          // add AudioSource to FpsController

		if (Application.isEditor) {
			webGLRightClickRotation = false;
			sensitivity = sensitivity * 1.5f;
		}
	}

	void CheckForWaterHeight(){
		if (transform.position.y < WaterHeight) {
			gravity = 0f;			
		} else {
			gravity = -9.8f;
		}
	}

	void CheckDistanceMusic(){
		if (musicTarget == null || zones.Length == 0) return;

		float dist = Vector3.Distance(transform.position, musicTarget.position);
		int newZone = -1;

		for (int i = 0; i < zones.Length; i++) {
			if (dist <= zones[i].maxDistance) {
				newZone = i;
				break;
			}
		}

		if (newZone != _currentZone) {
			_currentZone = newZone;

			if (newZone == -1) {
				_audio.Stop();
				return;
			}

			_audio.clip   = zones[newZone].music;
			_audio.volume = zones[newZone].volume;
			_audio.loop   = true;
			_audio.Play();
		}
	}
	void Update(){
		moveFB = Input.GetAxis ("Horizontal") * speed;
		moveLR = Input.GetAxis ("Vertical") * speed;

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		CheckForWaterHeight();
		CheckDistanceMusic();          // <-- added here

		Vector3 movement = new Vector3 (moveFB, gravity, moveLR);

		if (webGLRightClickRotation) {
			if (Input.GetKey (KeyCode.Mouse0)) {
				CameraRotation (cam, rotX, rotY);
			}
		} else if (!webGLRightClickRotation) {
			CameraRotation (cam, rotX, rotY);
		}

		movement = transform.rotation * movement;
		character.Move (movement * Time.deltaTime);
	}

	void CameraRotation(GameObject cam, float rotX, float rotY){		
		transform.Rotate (0, rotX * Time.deltaTime, 0);
		cam.transform.Rotate (-rotY * Time.deltaTime, 0, 0);
	}
}