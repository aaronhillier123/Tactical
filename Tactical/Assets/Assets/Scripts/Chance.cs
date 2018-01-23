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
	private RaycastHit hit;
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
			myTroopScript = Game._instance.GetTroop (id);
			myTroop = myTroopScript.gameObject;
			troopPos = Camera.main.WorldToScreenPoint (myTroop.transform.position);
			transform.position = new Vector2(troopPos.x, troopPos.y+50);
			Vector3 enemypos = target.gameObject.transform.position;
			Vector3 mypos = myTroopScript.gameObject.transform.position;
			Vector3 enemyhip = new Vector3 (enemypos.x, enemypos.y + 3, enemypos.z);
			Vector3 myhip = new Vector3 (mypos.x, mypos.y + 3, mypos.z);
			Vector3 dir = (enemyhip - myhip);
			float per;
			if (Physics.Raycast (myhip, dir, out hit, 200f)) {
				if (hit.collider.CompareTag ("NaturalCover")) {
					per = 0f;
				} else {
					float dis = Vector3.Distance (target.gameObject.transform.position, myTroop.transform.position);
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
				}
		string chance = per.ToString() + "%";
		gameObject.GetComponent<Text> ().text = chance;
		gameObject.GetComponent<Text> ().color = Color.white;
			}
		}
		catch{
		}
	}
}
