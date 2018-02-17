using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxHole : MonoBehaviour {


	public Trooper myTroop;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider coll){
		Trooper troop = coll.gameObject.GetComponent<Trooper> ();
		if (troop != null) {
			myTroop = troop;
			myTroop.inFoxHole = true;
		}
	}

	void OnTriggerExit(Collider coll){
		Trooper troop = coll.gameObject.GetComponent<Trooper> ();
		if (troop != null) {
			myTroop = null;
			troop.inFoxHole = false;
		}
	}
}
