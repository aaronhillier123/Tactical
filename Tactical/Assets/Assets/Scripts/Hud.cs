using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour {

	public Game game;
	// Use this for initialization
	void Start () {
		GameObject GO = GameObject.FindGameObjectWithTag ("GameController");
		if (GO != null) {
			Debug.Log ("Found Game object");
		}
		game = GO.GetComponent<Game> ();
		if (game != null) {
			Debug.Log ("found game script1");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void nextTurn(){
		//Debug.Log ("CLICKED THIS BUTTOOOON");
		if (game != null) {
			Debug.Log ("found game script1");
			game.changeTurn ();
		}
	}
}
