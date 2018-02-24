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
		float distance = Vector3.Distance (shooter.transform.position, target.transform.position);
		float percentOfHit = 100 - ((distance / shooter.getRange ()) * 100);
		Color colorKey = Color.green;

		Vector3 enemypos = target.transform.position + new Vector3(0f, 3f, 0f);
		Vector3 mypos = shooter.transform.position + new Vector3 (0f, 3f, 0f);

		//under cover may be blocked by barrier
		if (target.getPiece() != null) {
			Vector3 enemyCenter = target.GetComponent<CapsuleCollider> ().bounds.center;
			Vector3 barrierCenter = target.getPiece().GetComponent<BoxCollider> ().bounds.center;
			float DistanceToEnemy = Vector3.Distance (transform.position, enemyCenter);
			float DistanceToBarrier = Vector3.Distance (transform.position, barrierCenter);
			if (DistanceToEnemy > DistanceToBarrier) {
				percentOfHit = percentOfHit / 2;
				colorKey = Color.yellow;
			}
		}

		//if target is in foxhole
		if (target.inFoxHole) {
			colorKey = Color.yellow;
			percentOfHit = percentOfHit / 2;
		}

		//determine if enemy is behind terrain or cover
		RaycastHit hitcast;

		if(Physics.Linecast(mypos, enemypos, out hitcast)){
			if(hitcast.collider.CompareTag("Terrain") || hitcast.collider.CompareTag("NaturalCover")){
				percentOfHit = 0;
				colorKey = Color.red;
			}
		}

		if (percentOfHit < 0) {
			percentOfHit = 0;
		}

		Vector2 chancePos = Camera.main.WorldToScreenPoint (enemypos);
		transform.position = chancePos + new Vector2 (0, 50f);
		myText.text = Mathf.Round(percentOfHit).ToString () + "%";
		myText.color = colorKey;
	}
}
