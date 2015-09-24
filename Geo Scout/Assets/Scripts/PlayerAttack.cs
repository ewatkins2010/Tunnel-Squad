using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	public GameObject[] powers;
	public int currPower;
	public bool statsUp;
	CharacterMovement pMove;
	GameObject HUD;
	// Use this for initialization
	void Start () {
		currPower = 0;
		pMove = GetComponent<CharacterMovement> ();
		HUD = GameObject.Find ("HUD");
		statsUp = false;
	}
	
	// Update is called once per frame
	void Update () {
		Attack ();
	}

	void Attack(){
		if (Input.GetKeyDown (KeyCode.Z)) {
			switch(currPower){
			case 0:
				if (pMove.isRight)
					Instantiate (powers[0], new Vector3(transform.position.x + 5, transform.position.y, 0), transform.rotation);
				else
					Instantiate (powers[0], new Vector3(transform.position.x - 5, transform.position.y, 0), transform.rotation);
					break;
			case 1:
				GetComponent<SpriteRenderer>().color = Color.cyan;
				if (pMove.isRight)
					Instantiate (powers[1], new Vector3(transform.position.x + 5, transform.position.y, 0), transform.rotation);
				else
					Instantiate (powers[1], new Vector3(transform.position.x - 5, transform.position.y, 0), transform.rotation);
				StartCoroutine ("PowerTime");
				break;
			case 2:
				if (!statsUp){
					statsUp = true;
					pMove.isHard = true;
					GetComponent<SpriteRenderer>().color = Color.gray;
					StartCoroutine ("PowerTime");
				}
				break;
			case 3:
				if (!statsUp){
					statsUp = true;
					pMove.speed = 25;
					GetComponent<SpriteRenderer>().color = Color.magenta;
					StartCoroutine ("PowerTime");
				}
				break;
			default:
				break;
			}
		}
	}

	IEnumerator PowerTime(){
		yield return new WaitForSeconds (7f);
		statsUp = false;
		pMove.isHard = false;
		pMove.speed = 15;
		currPower = 0;
		HUD.GetComponent<HUD> ().SwapPowers (0);
		GetComponent<SpriteRenderer>().color = Color.green;
	}
}
