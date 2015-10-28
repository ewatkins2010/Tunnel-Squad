using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ReturnToTitleScreen(){
		Application.LoadLevel (0);
	}

	public void RestartGame(){
		Application.LoadLevel (1);
	}
}
