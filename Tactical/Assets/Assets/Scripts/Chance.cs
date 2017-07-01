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
	//private bool found = false;
	public Trooper target;
	private GameObject canv;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

		if (target!=null) {
			showOnHead ();
			} else {
			Debug.Log ("No id yet");
		}
	}

	void showOnHead(){
		try{
			myTroopScript = Game.GetTroop (id);
			myTroop = myTroopScript.gameObject;
			troopPos = Camera.main.WorldToScreenPoint (myTroop.transform.position);
			transform.position = new Vector2(troopPos.x, troopPos.y+50);
			float dis = Vector3.Distance (target.gameObject.transform.position, myTroop.transform.position);
			float per;
			if(target.isSniper == false){
				per = (float)Math.Round((100f - dis), 2);
			}
			else{
				per = (float)Math.Round((200f - dis), 2);
				per = per / 2.0f;
			}
			if (per < 0) {
				per = 0;
			}
		string chance = per.ToString() + "%";
		gameObject.GetComponent<Text> ().text = chance;
		gameObject.GetComponent<Text> ().color = Color.white;
		} catch{
		}
	}
}
