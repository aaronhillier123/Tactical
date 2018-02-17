using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopEntry{
	
	public TroopEntry(Trooper tt, float zz){
		t = tt;
		entry = zz;
		forcing = false;
	}

	public bool forcing = false;
	public Trooper t;
	public float entry;
}

public class BarrierPiece : MonoBehaviour {

	public int id;
	public int team;
	public bool placed = false;
	public bool done = false;


	public Barrier myBarrier;
	public Trooper thisTroop;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter(Collider coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null && GameHandler._instance.getPlayersTurn()>0) {

			if (myTroop.takingCover == true) {
				Debug.Log ("Taking cover");
				myTroop.setPiece (this);
				Vector3 dir = ( GetComponent<BoxCollider> ().bounds.center - myTroop.GetComponent<CapsuleCollider> ().bounds.center).normalized;
				myTroop.takeCover (dir);
			} else {
				if (myTroop.isMoving ()) {
					myTroop.jumpBarrier ();
				}
			}
		}
	}



}
