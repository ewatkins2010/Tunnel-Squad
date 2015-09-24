using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public int numLives, score, power;
	public bool gameOver;
	public GameObject[] lives;
	public GameObject[] powers;
	public Text scoreDisplay;
	// Use this for initialization
	void Start () {
		score = 0;
		numLives = 3;
		SwapPowers (0);
	}
	
	// Update is called once per frame
	void Update () {
		scoreDisplay.text = score + "";
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
