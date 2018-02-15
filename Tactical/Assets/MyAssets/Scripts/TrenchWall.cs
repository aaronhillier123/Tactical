using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchWall : MonoBehaviour {

	float timeFromColl;
	float timeOfColl;
	Trooper collidingTroop;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (timeOfColl != 0) {
			timeFromColl = Time.time - timeOfColl;
			if (timeFromColl > 0.25f && collidingTroop != null) {
				collidingTroop.StopAllCoroutines ();
				collidingTroop.stop ();
				collidingTroop = null;
				timeOfColl = 0;
			}
		}
	}

	void OnCollisionEnter(Collision coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			collidingTroop = myTroop;
			timeOfColl = Time.time;
		}
	}

	void OnCollisionExit(Collision coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			collidingTroop = null;
			timeOfColl = 0;
		}
	}

}
