using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPiece : MonoBehaviour {

	public int id;
	public int team;
	public bool placed = false;
	public bool done = false;

	public Barrier myBarrier;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			if (myTroop.covering == false) {
				myTroop.takeCover (gameObject.transform.position);
			} else {
				//myTroop.takeCover(id);
			}
		}
	}
}
