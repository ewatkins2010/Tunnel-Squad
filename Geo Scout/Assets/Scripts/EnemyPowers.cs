using UnityEngine;
using System.Collections;

public class EnemyPowers : MonoBehaviour {
	public bool isMole, isIce;
	public GameObject iceShot;
	Enemies stats;
	Animator a;
	// Use this for initialization
	void Start () {
		a = GetComponent<Animator> ();
		stats = GetComponent<Enemies> ();
		if (isIce)
			StartCoroutine ("ShootIce");
		if (isMole)
			StartCoroutine ("ActivatePower");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator ShootIce(){
		a.SetTrigger ("Attack");
		yield return new WaitForSeconds (5f);
	}

	IEnumerator ActivatePower(){
		int rand = Random.Range (3, 7);
		yield return new WaitForSeconds ((float)rand);
		StartCoroutine ("SpeedBoost");
	}

	IEnumerator SpeedBoost(){
		a.SetBool ("Attack", true);
		stats.speed = 30;
		yield return new WaitForSeconds (5f);
		a.SetBool ("Attack", false);
		stats.speed = 15;
		StartCoroutine ("SpeedBoost");
	}
}
