using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public GameObject[] first;
	public GameObject[] emptySets;
	public GameObject[] enemies;
	public int level, score, lives;

	Vector2 respawnPos;
	int set;
	AudioSource a;
	void Awake(){

		DontDestroyOnLoad (gameObject);
		score = 0;
		level = 1;
		lives = 3;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha3))
			Application.Quit ();

		if (Application.loadedLevel == 1) {
			GameObject[] e = GameObject.FindGameObjectsWithTag ("Enemy");
			if (e.Length == 0)
				SpawnEnemies();
		}
	}

	void SpawnEnemies(){
		foreach (Transform child in emptySets[set].transform) {
			int eIndex = Random.Range (0, enemies.Length-1);
			Instantiate (enemies[eIndex],child.position,transform.rotation);
		}
	}

	void OnLevelWasLoaded(int lvl){
		if (lvl == 2) {
			lives = 3;
			score = 0;
			level = 1;
		}
	}

	public void SpawnGems(){
		int random = Random.Range (0, first.Length-1);
		Instantiate (first[random], new Vector3 (Random.Range (5, 155), Random.Range (-5, -95), 0), first[random].transform.rotation);
	}

	public void SpawnObjects(){
		set = Random.Range (0, emptySets.Length - 1);

		Instantiate (emptySets [set], transform.position, emptySets[set].transform.rotation);

		foreach (Transform child in emptySets[set].transform) {
			int eIndex = Random.Range (0, enemies.Length-1);
			Instantiate (enemies[eIndex],child.position,transform.rotation);
		}
	}

	public void ResetPositions(){
		GameObject player = GameObject.Find ("Player");
		player.transform.position = new Vector3 (85, -45, 0);
		player.transform.rotation = Quaternion.Euler (0, 0, 0);
		GameObject[] e = GameObject.FindGameObjectsWithTag ("Enemy");

		for (int i = 0; i < e.Length; i++) {
			e[i].transform.position = emptySets[set].transform.GetChild(i).position;
			e[i].GetComponent<Enemies>().isMoving = false;
		}
	}
}
