using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour {

	public int id;
	private GameObject myTroop;
	private Trooper myTroopScript;
	private Vector3 troopPos;
	private bool found = false;
	private GameObject canv;
	// Use this for initialization
	void Start () {
		//id = 1;
	}

	// Update is called once per frame
	void Update () {
		if (myTroop == null && found == false && canv==null) {
			try{
			myTroop = Game._instance.GetTroop (id).gameObject;
				canv = GameObject.Find("Canvas");
			} catch{
			}
		} else if (myTroop != null && found == false && canv != null) {
			transform.SetParent(canv.transform);
			myTroopScript = myTroop.GetComponent<Trooper> ();
			//InvokeRepeating ("showOnHead", .1f, .1f);
			found = true;
		} else if(myTroop != null && found == true && canv != null) {
			showOnHead();
		} else {
		}
	}

	void showOnHead(){
		troopPos = Camera.main.WorldToScreenPoint (myTroop.transform.position);
		transform.position = new Vector2(troopPos.x, troopPos.y+50);
		Slider slide = GetComponent<Slider> ();
		slide.value = myTroopScript.getHealth ();
	}
}
