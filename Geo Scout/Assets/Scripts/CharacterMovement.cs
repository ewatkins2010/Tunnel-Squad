using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	public float speed;
	[HideInInspector]public float vInput, hInput;
	public bool isRight,isUp,isLeft,isDown, isDead, isHard;
	public AudioSource death;
	PlayerAttack pAttack;
	GameObject HUD;
	Animator a;
	// Use this for initialization
	void Start () {
		pAttack = GetComponent<PlayerAttack> ();
		a = GetComponent<Animator> ();
		HUD = GameObject.Find ("HUD");
		isLeft = true;
		isRight = isUp = isDown = false;
		isDead = false;
		isHard = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDead && !pAttack.isSucking) {
			vInput = Input.GetAxis ("Vertical");
			hInput = Input.GetAxis ("Horizontal");
		} else {
			vInput = hInput = 0f;
		}
		Move2 ();
	}

	void Move2(){
		Vector3 movement = Vector3.zero;

		if (hInput < 0) {
			a.SetBool ("Walk", true);
			isLeft = true;
			isRight = isUp = isDown = false;
			movement.x -= speed * Time.deltaTime;
			transform.position += movement;
			transform.rotation = Quaternion.Euler (0, 0, 0);
			movement = Vector3.zero;
		} else if (hInput > 0) {
			a.SetBool ("Walk", true);
			isRight = true;
			isLeft = isUp = isDown = false;
			movement.x += speed * Time.deltaTime;
			transform.rotation = Quaternion.Euler (0, 180, 0);
			transform.position += movement;
			movement = Vector3.zero;
		} else if (vInput < 0) {
			a.SetBool ("Walk", true);
			isDown = true;
			isUp = false;
			movement.y -= speed * Time.deltaTime;
			if (isRight)
				transform.rotation = Quaternion.Euler (0, 180, 90);
			else
				transform.rotation = Quaternion.Euler (0, 0, 90);
			transform.position += movement;
			movement = Vector3.zero;
		} else if (vInput > 0) {
			a.SetBool ("Walk", true);
			isUp = true;
			isDown = false;
			movement.y += speed * Time.deltaTime;
			if (isRight)
				transform.rotation = Quaternion.Euler (0, 180, -90);
			else
				transform.rotation = Quaternion.Euler (0, 0, -90);
			transform.position += movement;
			movement = Vector3.zero;
		} else 
			a.SetBool ("Walk", false);
	}

	void OnCollisionEnter2D (Collision2D col){
		if (col.gameObject.tag == "Barrier") {
			vInput = hInput = 0f;
		} 
		else if (col.gameObject.tag == "Enemy") {
			if (!isHard){
				a.SetTrigger ("Death");
				StartCoroutine ("Death");
			}
		}
	}

	IEnumerator Death(){
		isDead = true;
		vInput = hInput = 0f;
		HUD.GetComponent<HUD> ().numLives--;
		GetComponent<CircleCollider2D> ().enabled = false;
		death.Play ();
		yield return new WaitForSeconds (3f);

		if (HUD.GetComponent<HUD> ().numLives == 0) {
			Application.LoadLevel (2);
		}
		else {
			GetComponent<CircleCollider2D>().enabled = true;
			HUD.GetComponent<HUD>().manager.GetComponent<GameManager>().ResetPositions();
			isDead = false;
			a.SetTrigger ("Respawn");
		}
	}
}
