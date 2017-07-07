using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogTag : MonoBehaviour {

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
			myTroop.myPlayer.dogtags++;
			Hud.updateDogTags (myTroop.myPlayer.dogtags);
			Game.allDogTags.Remove (this);
			Destroy (gameObject);
		}
	}
}
