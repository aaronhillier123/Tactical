﻿using System.Collections;
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
		
	void OnTriggerEnter(Collider coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null && GameHandler._instance.getPlayersTurn()>0) {
			if (myTroop.takingCover == true || myTroop.covering == true) {
				myTroop.setPiece (this);
				myTroop.takeCover ();
			} else {
				myTroop.jumpBarrier ();
			}
		}
	}
}
