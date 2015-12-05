using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {
	public GameObject[] screens;
	EventSystem events;
	// Use this for initialization
	void Start () {
		events = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
		for (int i = 0; i < screens.Length; i++) {
			if (i != 0)
				screens[i].SetActive (false);
			else
				screens[i].SetActive (true);
		}
	}

	public void ChangeScreens(string name){
		for (int i = 0; i < screens.Length; i++) {
			if (name != screens[i].name)
				screens[i].SetActive (false);
			else 
				screens[i].SetActive (true);
		}

		events.SetSelectedGameObject (GameObject.FindGameObjectWithTag ("Button"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame(){
		Application.LoadLevel (1);
	}
}
