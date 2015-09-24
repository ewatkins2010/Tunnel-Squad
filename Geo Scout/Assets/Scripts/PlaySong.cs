using UnityEngine;
using System.Collections;

public class PlaySong : MonoBehaviour {
	public bool isPaused;
	public GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		isPaused = true;
		GetComponent<AudioSource> ().Play ();
		GetComponent<AudioSource> ().Pause ();
	}
	
	// Update is called once per frame
	void Update () {
		if (player.GetComponent<CharacterMovement> ().isDead) {
			GetComponent<AudioSource> ().Stop ();
			GetComponent<AudioSource> ().Play ();
			GetComponent<AudioSource> ().Pause ();
		}
		if (player.GetComponent<CharacterMovement>().vInput != 0 || player.GetComponent<CharacterMovement>().hInput != 0) {
			if (isPaused){
				GetComponent<AudioSource>().UnPause ();
				isPaused = false;
			}
		}
		else {
			isPaused = true;
			GetComponent<AudioSource>().Pause ();
		}
	}
	
}
