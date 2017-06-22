using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trooper : MonoBehaviour {

	private Animator animator;
	private int animInt;
	// Use this for initialization
	void Start () {

		animator = gameObject.GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("left")) {
			rotateTo (180.0f);
			move ();
		} else if (Input.GetKey ("down")) {
			rotateTo (90f);
			move ();
		} else if (Input.GetKey ("up")) {
			rotateTo (270f);
			move ();
		} else if (Input.GetKey ("right")) {
			rotateTo (0f);
			move ();
		} else if (Input.GetKey ("space")) {
			shoot ();
		} else {
			stop();
		}
	}

	void rotateTo (float a){
		float rotationPerMinute;
		Quaternion myRotation = gameObject.transform.rotation;
		float moveSpeed = .01f;
		gameObject.transform.rotation = Quaternion.Lerp(myRotation, Quaternion.Euler(0, a, 0), moveSpeed * Time.time);
	}

	void stop(){
		animator.SetInteger ("AnimPar", 0);
	}

	void move(){
		animator.SetInteger ("AnimPar", 1);
		gameObject.transform.Translate ( 0f, 0f, 0.25f);
	}

		void shoot(){
		animator.SetInteger ("AnimPar", 2);
	}
}
