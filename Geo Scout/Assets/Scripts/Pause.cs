using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {
	public GameObject pause;
	//public int currLevel;
	bool isPaused;
	// Use this for initialization
	void Start () {
		pause.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P))
			CheckIfPaused ();
	}

	public void CheckIfPaused(){
		if (!isPaused) {
			isPaused = true;
			Time.timeScale = 0;
			pause.SetActive (true);
		}
		else {
			isPaused = false;
			Time.timeScale = 1;
			pause.SetActive (false);
		}
	}

	public void Restart(){
		isPaused = false;
		Time.timeScale = 1;
		Application.LoadLevel (Application.loadedLevel);
	}

	public void QuitGame(){
		Application.Quit ();
	}
}
