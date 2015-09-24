using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public int projectileIndex;
	public GameObject player;
	PlayerAttack pAttack;
	CharacterMovement pMove;
	GameObject HUD;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 1f);

		player = GameObject.FindGameObjectWithTag ("Player");
		pAttack = player.GetComponent<PlayerAttack> ();
		pMove = player.GetComponent<CharacterMovement> ();
		HUD = GameObject.Find ("HUD");

		if (projectileIndex == 0)
			transform.SetParent (GameObject.FindGameObjectWithTag ("Player").transform);
		else {
			if (pMove.isRight)
				GetComponent<Rigidbody2D> ().AddForce (Vector2.right * 4000f);
			else
				GetComponent<Rigidbody2D> ().AddForce (Vector2.right * -4000f);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D c){
		if (c.gameObject.tag == "Enemy") {
			if (projectileIndex == 0) {
				pAttack.currPower = c.gameObject.GetComponent<Enemies>().powerIndex;
				HUD.GetComponent<HUD>().score+=c.GetComponent<Enemies>().reward;
				HUD.GetComponent<HUD>().SwapPowers (pAttack.currPower);
				Destroy (c.gameObject);
			}
			else{
				HUD.GetComponent<HUD>().score += c.GetComponent<Enemies>().reward;
				Destroy (c.gameObject);
			}
		}
		if (c.gameObject.tag == "Dirt" && projectileIndex == 1)
			Destroy (gameObject);
	}
}
