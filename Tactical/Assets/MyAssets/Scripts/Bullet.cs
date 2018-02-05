using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public int team;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision coll){
		if(coll.gameObject.CompareTag("Player")){
			
			Trooper t = coll.gameObject.GetComponent<Trooper>();
			Debug.Log ("GOT SHOT playerteam is " + t.team + " and bullet team is " + team);
			if(t.team!=team){
				t.StopAllCoroutines ();
				t.gotShot();
				Destroy(gameObject);
			}
		}
	}
}
