using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("left")) {
			moveLeft ();
		} else if (Input.GetKey ("down")) {
			moveDown ();
		} else if (Input.GetKey ("up")) {
			moveUp ();
		} else if (Input.GetKey ("right")) {
			moveRight ();
		}
	}

	void moveLeft(){
		gameObject.transform.Translate ( 0f, 0f, -0.25f);
	}

	void moveRight(){
		gameObject.transform.Translate ( 0f, 0f, 0.25f);
	}

	void moveUp(){
		gameObject.transform.Translate ( -0.25f, 0f, 0f);
	}

	void moveDown(){
		gameObject.transform.Translate ( 0.25f, 0f, 0);
	}
}
