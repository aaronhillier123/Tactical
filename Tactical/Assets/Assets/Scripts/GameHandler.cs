using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
	//handles macro game information such as turn stats and rosters


	public static GameHandler _instance;


	public List<Player> GamePlayers = new List<Player>();
	public GameObject PlayerObject;
	private int playersTurn = 0;
	private int turnNumber = 0;



	void Start () {
		_instance = this;
	}

	void Update () {
		
	}

	//create and initialize a global player
	public static void CreatePlayer(byte id, object content, int senderID){
		if (id == 1) {
			Vector3 pos = new Vector3 (0, 40, senderID * 50);
			GameObject newPlayerObject = Instantiate (GameHandler._instance.PlayerObject, pos, Quaternion.identity) as GameObject;
			Player newPlayer = newPlayerObject.GetComponent<Player> ();
			newPlayer.team = senderID;
			GameHandler._instance.GamePlayers.Add (newPlayer);
			if (PhotonNetwork.player.ID == newPlayer.team) {
				Game._instance.myPlayer = newPlayer;
				HudController._instance.myPlayer = newPlayer;
			}
			GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
			SpawnArea mySpawn = new SpawnArea ();
			foreach (GameObject g in spawns) {
				if (g.GetComponent<SpawnArea> ().team == newPlayer.team) {
					mySpawn = g.GetComponent<SpawnArea>();
				}
			}

			for (int i = 0; i < Player.numberOfTroops; ++i) {
				Vector3 newPos = mySpawn.spawnPoints [i].position;
				newPlayer.CreateTroopAt (newPos, mySpawn.FacingOut, senderID, ((senderID-1) * Player.numberOfTroops) + i);
			}
			if (PhotonNetwork.player.ID == newPlayer.team) {
				HudController._instance.showStartHud ();
			}
		}
	}


	public void RaiseTurnChange(){
		RaiseEventOptions rf = RaiseEventOptions.Default;
		rf.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent(5, null, true, rf);
	}

	public void RaiseEndPlacements(){
		BarrierHandler._instance.PlaceAllBarriers ();
		HudController._instance.removeStartHud ();
		PhotonNetwork.RaiseEvent(3, null, true, EventHandler.ops);
	}

	//End current turn and start turn for next player
	public static void changeTurn(byte Newid, object content, int SenderID){
		if (Newid == 5) {
			if (GameHandler._instance.playersTurn == PhotonNetwork.player.ID) {
				Game._instance.EndTurn ();
			}

			//change turn number
			if (GameHandler._instance.playersTurn < GameHandler._instance.GamePlayers.Count) {
				GameHandler._instance.playersTurn++;
			} else {
				GameHandler._instance.playersTurn = 1;
				GameHandler._instance.turnNumber++;
			}

			if (GameHandler._instance.playersTurn == PhotonNetwork.player.ID) {
				Game._instance.StartTurn ();
			}
		}
	}


	public static void EndPlacements(byte id, object content, int SenderID){

		if(id == 3){
			if (SenderID == PhotonNetwork.player.ID) {
				HudController._instance.removeStartHud ();
				Game._instance.BeginGame ();
				GameHandler._instance.getPlayer (PhotonNetwork.player.ID).ready = true;
			}
			GameHandler._instance.getPlayer (SenderID).ready = true;
			int notReady = 0;
			foreach (Player p in GameHandler._instance.GamePlayers) {
				if (p.ready == false) {
					notReady++;
				}
			}
			if (notReady == 0 && SenderID == PhotonNetwork.player.ID) {
				GameHandler._instance.RaiseTurnChange ();
			}
		}
	}



	//return player based on ID
	public Player getPlayer(int id){
		foreach (Player p in GamePlayers) {
			if (p.team == id) {
				return p;
			}
		}
		return null;
	}

	public int getTurnNumber(){
		return turnNumber;
	}

	public int getPlayersTurn(){
		return playersTurn;
	}

	public RaiseEventOptions AllReceivers(){
		RaiseEventOptions ops = RaiseEventOptions.Default;
		ops.Receivers = ReceiverGroup.All;
		return ops;
	}

}
