using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Chance : MonoBehaviour {

	public int id;
	public Trooper shooter;
	public Trooper target;
	public Text myText;
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		showOnHead ();
	}

	void showOnHead(){
		//Randomize hit based on distance and troop range
		float percentOfHit = Game._instance.getChanceOfHit(shooter, target);
		Vector3 enemypos = target.transform.position + new Vector3(0f, 3f, 0f);
		Vector3 mypos = shooter.transform.position + new Vector3 (0f, 3f, 0f);
		Vector2 chancePos = Camera.main.WorldToScreenPoint (enemypos);
		Color colorkey;
		if (percentOfHit == 0) {
			colorkey = Color.red;
		} else {
			colorkey = Color.green;
		}
		transform.position = chancePos + new Vector2 (0, 50f);
		myText.text = Mathf.Round(percentOfHit).ToString () + "%";
		myText.color = colorkey;
	}
}
