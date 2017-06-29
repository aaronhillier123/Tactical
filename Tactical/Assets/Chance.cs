using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Chance : MonoBehaviour {

	public int id;
	private GameObject myTroop;
	private Trooper myTroopScript;
	private Vector3 troopPos;
	private bool found = false;
	public Vector3 target;
	private GameObject canv;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

		if (id != null && target!=null) {
			showOnHead ();
			} else {
			Debug.Log ("No id yet");
		}
	}

	void showOnHead(){
		myTroopScript = Game.GetTroop (id);
		myTroop = myTroopScript.gameObject;
		troopPos = Camera.main.WorldToScreenPoint (myTroop.transform.position);
		transform.position = new Vector2(troopPos.x, troopPos.y+50);
		float dis = Vector3.Distance (target, myTroop.transform.position);
		float per = (float)Math.Round((100f - dis), 2);
		if (per < 0) {
			per = 0;
		}
		string chance = per.ToString() + "%";
		gameObject.GetComponent<Text> ().text = chance;
		gameObject.GetComponent<Text> ().color = Color.white;
	}
}
