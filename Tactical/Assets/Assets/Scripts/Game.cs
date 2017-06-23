using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	// Use this for initialization
	public List<Player> GamePlayers = new List<Player>();
	public GameObject Player;
	public int numberOfPlayers = 2;
	public int turnNumber = 1;
	public int playersTurn;


	void Start () {
		for (int i = 0; i < numberOfPlayers; ++i) {
			CreatePlayer (i);
			//Debug.Log ("created player");
		}
		playersTurn = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreatePlayer(int team){
		GameObject newPlayerObject = Instantiate (Player, transform, true) as GameObject;
		Player newPlayer = newPlayerObject.GetComponent<Player> ();
		newPlayer.team = team;
		GamePlayers.Add (newPlayer);
	}

	public void changeTurn(){
		Debug.Log ("Changing turn");
		if (playersTurn < 4) {
			playersTurn++;
		} else {
			playersTurn = 0;
		}
	}
}
