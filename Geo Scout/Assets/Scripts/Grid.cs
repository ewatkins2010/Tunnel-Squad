using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public Vector3 startPos1;
	public GameObject[] dirt;
	public AudioSource intro;
	int dirtIndex;
	// Use this for initialization
	void Start () {
		SpawnDirt ();
		intro.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SpawnDirt(){
		dirtIndex = 0;
		for (int i = 0; i < 23; i++) {
			for (int j = 0; j < 130; j++){
				Instantiate(dirt[dirtIndex], new Vector3(startPos1.x + (j*2), startPos1.y - (i*2), 0), transform.rotation);
			}
		}

		dirtIndex = 1;
		for (int i = 23; i < 46; i++) {
			for (int j = 0; j < 130; j++){
				Instantiate(dirt[dirtIndex], new Vector3(startPos1.x + (j*2), startPos1.y - (i*2), 0), transform.rotation);
			}
		}
		dirtIndex = 2;
		for (int i = 46; i < 70; i++) {
			for (int j = 0; j < 130; j++){
				Instantiate(dirt[dirtIndex], new Vector3(startPos1.x + (j*2), startPos1.y - (i*2), 0), transform.rotation);
			}
		}
	}
}
