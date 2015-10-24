using UnityEngine;
using System.Collections;

public class Enemies : MonoBehaviour {
	public bool canUp, canLeft, canRight, canDown, sucked;
	public int powerIndex, reward;


    public LayerMask enemyMask;
    public float speed = 2;
    Rigidbody2D myBody;
    Transform myTrans;
    float myWidth;


    // Use this for initialization
    void Start () {

        myTrans = this.transform;
        myBody = this.GetComponent<Rigidbody2D>();
        myWidth = this.GetComponent<BoxCollider2D>().bounds.extents.x;

    }
	
	// Update is called once per frame
	void Update () {

        //draws horizontal line
        Vector2 lineCastPos1 = myTrans.position - myTrans.right * myWidth;
        Debug.DrawLine(lineCastPos1, lineCastPos1 - myTrans.right.toVector2() * .1f);
        bool isBlockedH = Physics2D.Linecast(lineCastPos1, lineCastPos1 - myTrans.right.toVector2() * .1f, enemyMask);

        //draws vertical line
        Vector2 lineCastPos2 = myTrans.position - myTrans.up * myWidth;
        Debug.DrawLine(lineCastPos2, lineCastPos2 - myTrans.up.toVector2() * .1f);
        bool isBlockedU = Physics2D.Linecast(lineCastPos2, lineCastPos2 - myTrans.up.toVector2() * .1f, enemyMask);


        //if blocked horizontally then rotate 180 degrees
       // if (isBlockedH)
       // {
        //    Vector3 currRot = myTrans.eulerAngles;
        //   currRot.y += 180;
        //  myTrans.eulerAngles = currRot;

       // }


       // if (isBlockedU)
       // {
        //    Vector3 currRot = myTrans.eulerAngles;
        //    currRot.x += 180;
        //    myTrans.eulerAngles = currRot;
       // }



        //moving forward
        Vector2 myVel = myBody.velocity;
        myVel.x = -myTrans.right.x * speed;
        myVel.y = -myTrans.up.y * speed;
        myBody.velocity = myVel;
    }


}
