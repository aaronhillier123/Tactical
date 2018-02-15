using System.Collections;
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
			myTroop.StopAllCoroutines ();
			myTroop.goBack (2f);
			Debug.Log ("Stopping cause of collision enter");
			myTroop.stop ();
		} else {

		}

	}

}
