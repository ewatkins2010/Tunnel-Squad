using UnityEngine;
using System.Collections;

public class SpawnedObject : MonoBehaviour {
	public Vector3 spawmPos;
	public GameObject nextObject, clearImage;
	public bool nextIsExit, isExit;
	HUD h;
	AudioSource sound;
	// Use this for initialization
	void Start () {
		sound = GameObject.Find ("Gem").GetComponent<AudioSource> ();
		h = GameObject.Find ("HUD").GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			if (nextIsExit){
				StartCoroutine("SpawnExit");
			}
			else {
				if (isExit){
					col.gameObject.GetComponent<CircleCollider2D>().enabled = false;
					StartCoroutine ("GoToNextLevel");
				}
				else {
					if (nextObject != null){
						StartCoroutine ("SpawnNext");
					}
				}
			}
		}
	}

	IEnumerator GoToNextLevel(){
		if (clearImage != null) {
			GameObject hud = GameObject.Find ("HUD");
			GameObject image = Instantiate (clearImage, new Vector3(-84,0,0), hud.transform.rotation) as GameObject;
			image.transform.SetParent (hud.transform, false);
		}
		AudioSource a = GameObject.Find ("RoundClear").GetComponent<AudioSource> ();
		a.Play ();
		GameManager manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager>();
		GameObject[] e = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject temp in e)
			h.score += 1000;
		manager.level++;
		manager.score = h.score;
		manager.lives = h.numLives;
		yield return new WaitForSeconds (3f);
		Application.LoadLevel (Application.loadedLevel);
	}

	IEnumerator SpawnNext(){
		sound.Play ();
		h.score += 100;
		GetComponent<SpriteRenderer> ().enabled = false;
		yield return new WaitForSeconds(1f);
		Instantiate (nextObject, new Vector3(Random.Range (5,155), Random.Range (-5,-95),0),transform.rotation);
		Destroy (gameObject);
	}

	IEnumerator SpawnExit(){
		sound.Play ();
		h.score += 500;
		GetComponent<SpriteRenderer> ().enabled = false;
		yield return new WaitForSeconds(2f);
		Instantiate (nextObject, new Vector3(155,5,0),transform.rotation);
		Destroy (gameObject);
	}
}
