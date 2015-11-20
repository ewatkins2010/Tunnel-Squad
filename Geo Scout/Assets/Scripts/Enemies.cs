using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour {
	public int powerIndex, reward;
	public bool isMoving, hasTarget;

    public float speed;

	Vector2 targetPos;
	bool canUp, canLeft, canRight, canDown;
	int rand;
	List <Vector2> targets = new List<Vector2>();

    Rigidbody2D myBody;
    Transform myTrans;
    float myWidth;
	GameObject player;

    


    // Use this for initialization
    void Start () {
		canUp = canDown = canLeft = canRight = false;
		isMoving = false;
		player = GameObject.Find ("Player");

        

    }
	
	// Update is called once per frame
	void Update () {
		Move ();
    }

    public void CheckDirection()
    {
        Vector2 upStart, leftStart, downStart, rightStart;

        upStart = new Vector2(transform.position.x, transform.position.y + 5f);
        leftStart = new Vector2(transform.position.x - 5f, transform.position.y);
        downStart = new Vector2(transform.position.x, transform.position.y - 5f);
        rightStart = new Vector2(transform.position.x + 5f, transform.position.y);

        Debug.DrawRay(upStart, Vector2.up * 10f, Color.red);
        Debug.DrawRay(downStart, Vector2.down * 10f, Color.red);
        Debug.DrawRay(leftStart, Vector2.left * 10f, Color.red);
        Debug.DrawRay(rightStart, Vector2.right * 10f, Color.red);

        RaycastHit2D upHit = Physics2D.Raycast(upStart, Vector2.up, 9f);
        if (upHit.collider != null)
        {
            if (upHit.collider.gameObject.tag == "Dirt" || upHit.collider.gameObject.tag == "Barrier")
            {
                canUp = false;
            }
            else
                canUp = true;
        }
        else
            canUp = true;

        RaycastHit2D downHit = Physics2D.Raycast(downStart, Vector2.down, 9f);
        if (downHit.collider != null)
        {
            if (downHit.collider.gameObject.tag == "Dirt" || downHit.collider.gameObject.tag == "Barrier")
            {
                canDown = false;
            }
            else
                canDown = true;
        }
        else
            canDown = true;

        RaycastHit2D leftHit = Physics2D.Raycast(leftStart, Vector2.left, 9f);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.gameObject.tag == "Dirt" || leftHit.collider.gameObject.tag == "Barrier")
            {
                canLeft = false;
                Vector3 theScale = transform.localScale;


                //turns scale negative on wall impact (to change sprite direction)
                if (!canLeft && theScale.x > 0)
                {
                    theScale.x *= -1;
                    transform.localScale = theScale;
                }
            }
            else
                canLeft = true;

        }
        else
            canLeft = true;

        RaycastHit2D rightHit = Physics2D.Raycast(rightStart, Vector2.right, 9f);
        if (rightHit.collider != null)
        {
            if (rightHit.collider.gameObject.tag == "Dirt" || rightHit.collider.gameObject.tag == "Barrier")
            {
                canRight = false;
                Vector3 theScale = transform.localScale;


                //turns scale negative on wall impact (to change sprite direction)
                if (!canRight && theScale.x < 0)
                {
                    theScale.x *= -1;
                    transform.localScale = theScale;
                }
            }
            else
                canRight = true;
        }
        else
            canRight = true;

        Vector2 upTarget = new Vector2(transform.position.x, transform.position.y + 10f);
        Vector2 downTarget = new Vector2(transform.position.x, transform.position.y - 10f);
        Vector2 leftTarget = new Vector2(transform.position.x - 10f, transform.position.y);
        Vector2 rightTarget = new Vector2(transform.position.x + 10f, transform.position.y);

       
        if (canUp)
            targets.Add(upTarget);
        if (canDown)
            targets.Add(downTarget);
        if (canLeft)
            targets.Add(leftTarget);
        if (canRight)
            targets.Add(rightTarget);
        Debug.Log((int)targets.Count);
        rand = Random.Range(0, (int)targets.Count);

        if (targets.Count > 0)
            targetPos = targets[rand];
        targets.Clear();
        
	}

	void Move(){
		if (!isMoving) {
			CheckDirection ();
			isMoving = true;
		}
		else {
			if(!player.GetComponent<CharacterMovement>().isDead){
				transform.position = Vector2.MoveTowards (transform.position,targetPos,speed*Time.deltaTime);
				if ((Vector2)transform.position == targetPos)
					isMoving = false;
			}
		}
	}
}
