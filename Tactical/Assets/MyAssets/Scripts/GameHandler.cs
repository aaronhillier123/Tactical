using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;

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
			Vector3 pos = new Vector3 (0, 0, senderID * 50);
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
				if (GameHandler._instance.turnNumber == 0) {
					HudController._instance.showStartHud ();
				} else {
					HudController._instance.showGameHud ();
				}
			}
			ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable ();
			ht.Add ("Player" + PhotonNetwork.player.ID.ToString(), true);
		}
	}


	public void RaiseTurnChange(){
		RaiseEventOptions rf = RaiseEventOptions.Default;
		rf.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent(5, null, true, AllReceivers());
	}

	public string TroopState(){
		string gamestate = "";
		foreach (Player p in GamePlayers) {
			foreach (Trooper t in p.roster) {
				gamestate += t.id.ToString () + "/";
				gamestate += t.team.ToString () + "/";
				gamestate += t.getHealth () + "/";
				gamestate += t.currentPosition.x.ToString () + "/";
				gamestate += t.currentPosition.y.ToString () + "/";
				gamestate += t.currentPosition.z.ToString () + "/";
				gamestate += t.currentRotation.eulerAngles.x.ToString () + "/";
				gamestate += t.currentRotation.eulerAngles.y.ToString () + "/";
				gamestate += t.currentRotation.eulerAngles.z.ToString () + "/";
				if (t.getPiece () != null) {
					gamestate += t.getPiece ().id.ToString ();
				} else {
					gamestate += "0";
				}
				gamestate += " ";
			}
		}
		return gamestate;
	}

	public void UpdateTroopState(){
		ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.room.CustomProperties;
		object troopsObject;
		ht.TryGetValue ("Troops", out troopsObject);
		string troopsString = (string)troopsObject;
		if (troopsString != null) {
			string[] ta = troopsString.Split (' ');
			List<string> ts = new List<string>(ta);
			ts.RemoveAt (ts.Count - 1);
			foreach(string s in ts){
				UpdateTroopFromCode (s);
			}
		}
		Debug.Log ("UPDATED ALL TROOPS");
	}

	public void UpdateTroopFromCode(string troopCode){
		string[] tsa = troopCode.Split ('/');
		Trooper t = Game._instance.GetTroop (int.Parse(tsa[0]));
		if (t != null) {
			t.team = int.Parse(tsa [1]);
			float x = float.Parse (tsa [3]);
			float y = float.Parse (tsa [4]);
			float z = float.Parse (tsa [5]);
			t.transform.position = new Vector3 (x, y, z);
			t.decreaseHealth (t.getHealth () - float.Parse (tsa [2]));
			float rx = float.Parse (tsa [6]);
			float ry = float.Parse (tsa [7]);
			float rz = float.Parse (tsa [8]);
			t.transform.eulerAngles = new Vector3 (rx, ry, rz);
			if (int.Parse(tsa [9]) != 0) {
				t.setPiece (BarrierHandler._instance.getPiece (int.Parse(tsa [9])));
			}
		}
	}

	public string dogState(){
		string dogstate = "";
		foreach (DogTag d in Game._instance.allDogTags) {
			dogstate += d.transform.position.x.ToString () + "/";
			dogstate += d.transform.position.x.ToString () + "/";
			dogstate += d.transform.position.x.ToString () + " ";
		}
		return dogstate;
	}

	public void sendGameState(){
		string troopstate = TroopState ();
		string dogstate = dogState ();
		ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable ();
		ht.Add ("Troops", troopstate);
		ht.Add ("Dogs", dogstate);
		ht.Add ("Turn", turnNumber);
		string deb;
		//Debug.Log (troopstate);
		PhotonNetwork.room.SetCustomProperties (ht, null, true);
	}

	public void RaiseEndPlacements(){
		BarrierHandler._instance.PlaceAllBarriers ();
		HudController._instance.removeStartHud ();
		PhotonNetwork.RaiseEvent(3, null, true, AllReceivers());
	}

	//End current turn and start turn for next player
	public static void changeTurn(byte Newid, object content, int SenderID){
		if (Newid == 5) {
			if (GameHandler._instance.playersTurn == PhotonNetwork.player.ID) {
				GameHandler._instance.sendGameState ();
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
				GameHandler._instance.UpdateTroopState ();
				Game._instance.StartTurn ();
			}
		}
	}


	public static void EndPlacements(byte id, object content, int SenderID){

		if(id == 3){
			if (SenderID == PhotonNetwork.player.ID) {
				HudController._instance.removeStartHud ();
				Game._instance.BeginGame ();
				GameHandler._instance.getPlayer (PhotonNetwork.player.ID).setReady (true);
			}
			GameHandler._instance.getPlayer (SenderID).setReady(true);
			int notReady = 0;
			foreach (Player p in GameHandler._instance.GamePlayers) {
				if (p.isReady() == false) {
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
		RaiseEventOptions ops = new RaiseEventOptions () {
			ForwardToWebhook = true,
			CachingOption = EventCaching.AddToRoomCache,
			Receivers = ReceiverGroup.All,
		};
		return ops;
	}

	public RaiseEventOptions OtherReceivers(){
		RaiseEventOptions ops = new RaiseEventOptions () {
			ForwardToWebhook = true,
			CachingOption = EventCaching.AddToRoomCache,
			Receivers = ReceiverGroup.Others,
		};

		return ops;
	}

}
