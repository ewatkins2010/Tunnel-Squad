using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public int numLives, score, power;
	public bool gameOver;
	public GameObject[] lives;
	public GameObject[] powers;
	public Text scoreDisplay;
	public Text levelDisplay;
	public GameObject instance;
	public GameObject manager;
	// Use this for initialization
	void Awake(){
		manager = GameObject.FindGameObjectWithTag ("GameManager");
		if (manager == null) {
			manager = Instantiate (instance, new Vector3(0,0,0), transform.rotation) as GameObject;
		}
	}
	void Start () {
		SwapPowers (0);

		manager.GetComponent<GameManager> ().SpawnObjects ();
		score = manager.GetComponent<GameManager> ().score;
		numLives = manager.GetComponent<GameManager> ().lives;
	}
	
	// Update is called once per frame
	void Update () {
		scoreDisplay.text = score + "";
		levelDisplay.text = manager.GetComponent<GameManager> ().level + "";
		CheckLives ();
	}

	public void CheckLives(){
		switch (numLives) {
		case 0:
			lives[0].SetActive(false);
			lives[1].SetActive(false);
			lives[2].SetActive(false);
			break;
		case 1:
			lives[0].SetActive(true);
			lives[1].SetActive(false);
			lives[2].SetActive(false);
			break;
		case 2:
			lives[0].SetActive(true);
			lives[1].SetActive(true);
			lives[2].SetActive(false);
			break;
		case 3:
			lives[0].SetActive(true);
			lives[1].SetActive(true);
			lives[2].SetActive(true);
			break;
		default:
			break;
		}
	}

	public void SwapPowers(int powerIndex){
		for (int i = 0; i < powers.Length; i++) {
			if (i != powerIndex)
				powers[i].SetActive (false);
		}
		powers[powerIndex].SetActive(true);
	}
}
