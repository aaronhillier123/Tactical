using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogTagJson{
	Vector3 position;
}

public class DogTag : MonoBehaviour {

	public bool updated = false;
	public int id;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Rotate (0f, 1f, 0f);
	}

	void OnTriggerEnter(Collider coll){
		Debug.Log ("trigger endered");
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			Debug.Log ("trigger troop is " + myTroop.id);
			myTroop.myPlayer.addDogTags(1);
			Hud.updateDogTags (myTroop.myPlayer.getDogTags());
			Game._instance.allDogTags.Remove (this);
			Destroy (gameObject);
		}
	}
}
