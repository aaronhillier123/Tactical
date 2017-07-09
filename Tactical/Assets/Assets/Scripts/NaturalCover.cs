﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalCover : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			Debug.Log ("Collision");
			myTroop.StopAllCoroutines ();
			myTroop.goBack ();
			myTroop.stop ();
		} else {
			Debug.Log ("NO COLLISION");
		}

	}

}