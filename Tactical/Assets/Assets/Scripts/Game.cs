using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	// Use this for initialization
	public List<Player> GamePlayers = new List<Player>();
	public GameObject Player;
	public int numberOfPlayers = 0;
	public int turnNumber = 1;
	public int playersTurn = 1;
	void Start () {
		CreatePlayer ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreatePlayer(){
		GameObject newPlayerObject = Instantiate (Player, transform, true) as GameObject;
		Player newPlayer = newPlayerObject.GetComponent<Player> ();
		newPlayer.id = numberOfPlayers + 1;
		numberOfPlayers++;
		GamePlayers.Add (newPlayer);
	}
}
