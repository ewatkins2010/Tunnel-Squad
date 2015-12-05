using UnityEngine;
using System.Collections;

public class PlaySong : MonoBehaviour {
	public GameObject player;
	bool isPaused;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		isPaused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (player.GetComponent<CharacterMovement> ().isDead || player.GetComponent<CircleCollider2D>().enabled == false) {
			GetComponent<AudioSource> ().Stop ();
			GetComponent<AudioSource> ().Play ();
			GetComponent<AudioSource> ().Pause ();
			isPaused = true;
		}
		else {
			if (isPaused){
				GetComponent<AudioSource>().Play ();
				isPaused = false;
			}
		}
	}
	
}
