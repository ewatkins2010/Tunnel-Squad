using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	public GameObject[] powers;
	public GameObject vacuum;
	public int currPower;
	public bool statsUp, isSucking;
	CharacterMovement pMove;
	GameObject HUD;
	Animator a;
	// Use this for initialization
	void Start () {
		currPower = 0;
		pMove = GetComponent<CharacterMovement> ();
		HUD = GameObject.Find ("HUD");
		statsUp = false;
		a = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		Attack ();
	}

	void Attack(){
		switch(currPower){
		case 0:
			a.SetBool ("PowerDown", false);
			if (Input.GetKeyDown (KeyCode.U) || Input.GetKeyDown (KeyCode.Z)){
				isSucking = true;
				a.SetBool ("Suck", true);
				vacuum.SetActive(true);
			}
			else{
				isSucking = false;
				a.SetBool ("Suck",false);
				vacuum.SetActive (false);
			}
			break;
		case 1:
			a.SetBool ("PowerDown", false);
			isSucking = false;
			a.SetBool ("Suck",false);
			vacuum.SetActive (false);
			if (Input.GetKeyDown (KeyCode.U) || Input.GetKeyDown (KeyCode.Z)) {
				a.SetTrigger ("Blue");
				if (pMove.isUp)
					Instantiate (powers[1],transform.position+ (Vector3.up*5), transform.rotation);
				else if(pMove.isDown)
					Instantiate (powers[1],transform.position+ (Vector3.down*5), transform.rotation);
				else if(pMove.isLeft && !pMove.isDown && !pMove.isUp)
					Instantiate (powers[1],transform.position+ (Vector3.left*5), transform.rotation);
				else if (pMove.isRight && !pMove.isDown && !pMove.isUp)
					Instantiate (powers[1],transform.position+ (Vector3.right*5), transform.rotation);
				StartCoroutine ("PowerTime");
			}
			break;
		case 2:
			isSucking = false;
			a.SetBool ("Suck",false);
			vacuum.SetActive (false);
			if (Input.GetKeyDown (KeyCode.U) || Input.GetKeyDown (KeyCode.Z)){
				if (!statsUp){
					a.SetBool ("PowerDown", false);
					a.SetTrigger ("Yellow");
					statsUp = true;
					pMove.isHard = true;
					StartCoroutine ("PowerTime");
				}
			}
			break;
		case 3:
			isSucking = false;
			a.SetBool ("Suck",false);
			vacuum.SetActive (false);
			if (Input.GetKeyDown (KeyCode.U) || Input.GetKeyDown (KeyCode.Z)){
				if (!statsUp){
					a.SetBool ("PowerDown", false);
					a.SetTrigger ("Purple");
					statsUp = true;
					pMove.speed = 25;
					StartCoroutine ("PowerTime");
				}
			}
			break;
		default:
			break;
		}
	}

	IEnumerator PowerTime(){
		yield return new WaitForSeconds (7f);
		statsUp = false;
		pMove.isHard = false;
		pMove.speed = 20;
		currPower = 0;
		a.SetBool ("PowerDown", true);
		HUD.GetComponent<HUD> ().SwapPowers (0);
	}
}
