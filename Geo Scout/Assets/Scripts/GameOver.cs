using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Restart(){
		Application.LoadLevel (0);
	}

	public void Refresh(){
		Application.LoadLevel (1);
	}
}
