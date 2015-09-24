using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	public float speed, playerX, playerY, playerZ;
	public float vInput, hInput;
	public Vector3 uPos, dPos, lPos, rPos, currPos, targetPos;
	public Transform respawnPoint;
	public bool hasTarget, isRight,isUp, isDead, isHard;
	public Animation a;
	public AudioSource death;
	GameObject HUD;

	// Use this for initialization
	void Start () {
		a = GetComponent<Animation> ();
		HUD = GameObject.Find ("HUD");
		currPos = transform.position;
		hasTarget = false;
		isRight = false;
		isDead = false;
		isHard = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!a.isPlaying && !isDead) {
			vInput = Input.GetAxis ("Vertical");
			hInput = Input.GetAxis ("Horizontal");
		}
		CheckDirection ();
		CheckTarget ();
		Move ();
	}

	void CheckDirection(){
		if (hInput < 0)
			isRight = false;
		else if (hInput > 0)
			isRight = true;

		if (vInput < 0)
			isUp = false;
		else if (vInput > 0)
			isUp = true;
	}

	void CheckTarget(){
		if (!hasTarget) {
			playerX = transform.position.x;
			playerY = transform.position.y;
			playerZ = transform.position.z;
			
			uPos = new Vector3 (playerX, playerY + 10f, playerZ);
			dPos = new Vector3 (playerX, playerY - 10f, playerZ);
			lPos = new Vector3 (playerX - 10f, playerY, playerZ);
			rPos = new Vector3 (playerX + 10f, playerY, playerZ);
			currPos = transform.position;

			if (hInput < 0){
				targetPos = lPos;
				hasTarget = true;
				return;
			}

			if (hInput > 0){
				targetPos = rPos;
				hasTarget = true;
				return;
			}

			if (vInput < 0){
				targetPos = dPos;
				hasTarget = true;
				return;
			}

			if (vInput > 0){
				targetPos = uPos;
				hasTarget = true;
				return;
			}
		}
	}

	void Move(){
		if (hasTarget) {
			if (targetPos == uPos) {
				if (vInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
				else if (vInput < 0)
					transform.position = Vector3.MoveTowards(transform.position, currPos, speed*Time.deltaTime);
				else if(hInput < 0 || hInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
			}
			else if(targetPos == dPos){
				if (vInput < 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
				else if (vInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, currPos, speed*Time.deltaTime);
				else if(hInput < 0 || hInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
			}
			else if (targetPos == lPos){
				if (hInput < 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
				else if (hInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, currPos, speed*Time.deltaTime);
				else if(vInput < 0 || vInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
			}
			else if (targetPos == rPos){
				if (hInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
				else if (hInput < 0)
					transform.position = Vector3.MoveTowards(transform.position, currPos, speed*Time.deltaTime);
				else if(vInput < 0 || vInput > 0)
					transform.position = Vector3.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);
			}
		}

		if (transform.position == targetPos || transform.position == currPos)
			hasTarget = false;
	}

	void OnCollisionEnter2D (Collision2D col){
		if (col.gameObject.tag == "Barrier") {
			transform.position = currPos;
			targetPos = currPos;
		} 
		else if (col.gameObject.tag == "Enemy") {
			if (!isHard)
				StartCoroutine ("Death");
		}
	}

	IEnumerator Death(){
		isDead = true;
		hasTarget = false;
		vInput = hInput = 0f;
		HUD.GetComponent<HUD> ().numLives--;
		death.Play ();
		yield return new WaitForSeconds (3f);

		if (HUD.GetComponent<HUD> ().numLives == 0) {
			Application.LoadLevel (2);
		}
		else {
			transform.position = respawnPoint.position;
			isDead = false;
		}
	}
}
