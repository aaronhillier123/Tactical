using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameHandler : MonoBehaviour {
	//handles macro game information such as turn stats and rosters


	public static GameHandler _instance;


	public List<Player> GamePlayers = new List<Player>();
	public GameObject PlayerObject;
	private int playersTurn = 0;
	private int turnNumber = 0;
	private bool GamePhase = false;



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
		}
	}


	public void RaiseTurnChange(){
		int lastturn = GameHandler._instance.playersTurn;
		int newturn = lastturn;
		if (lastturn == PhotonNetwork.room.PlayerCount || lastturn==0) {
			newturn = 1;
		} else {
			newturn++;
		}
		int[] turns = new int[]{ lastturn, newturn };
		object turnObject = (object)turns;

		//Raise Turn Change
		PhotonNetwork.RaiseEvent(5, null, true, new RaiseEventOptions(){
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.RemoveFromRoomCache});

		PhotonNetwork.RaiseEvent(5, turnObject, true, new RaiseEventOptions(){
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.AddToRoomCache,
			ForwardToWebhook = true});

		GameHandler._instance.refreshGameStates();
	}

	public void refreshGameStates (){
		object ht = (object)GameHandler._instance.GetGameState ();
		PhotonNetwork.RaiseEvent (9, null, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All, 
			CachingOption = EventCaching.RemoveFromRoomCache
		});
		PhotonNetwork.RaiseEvent (9, ht, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All, 
			CachingOption = EventCaching.AddToRoomCache,
			ForwardToWebhook = true
		});
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

	public void UpdateTroopState(Hashtable ht){
		object troopsObject;
		ht.TryGetValue ("Troops", out troopsObject);
		string troopsString = (string)troopsObject;
		if (troopsString != null) {
			Debug.Log (troopsString);
			string[] ta = troopsString.Split (' ');
			List<string> ts = new List<string>(ta);
			ts.RemoveAt (ts.Count - 1);
			foreach(string s in ts){
				UpdateTroopFromCode (s);
			}
		}
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

	public Hashtable GetGameState(){
		string troopstate = TroopState ();
		string dogstate = dogState ();
		Hashtable ht = new Hashtable ();
		ht.Add ("Troops", troopstate);
		ht.Add ("Dogs", dogstate);
		ht.Add ("Turn", turnNumber);
		return ht;
	}

	public void SendGameState(){
		PhotonNetwork.room.SetCustomProperties(GameHandler._instance.GetGameState(), null, true);
	}

	public void RaiseEndPlacements(){
		PhotonNetwork.RaiseEvent(3, null, true, AllReceivers());
		BarrierHandler._instance.PlaceAllBarriers ();
		HudController._instance.removeStartHud ();
		Game._instance.BeginGame ();
		GameHandler._instance.GamePhase = true;
		if (GameHandler._instance.ReadyForChange ()) {
			RaiseTurnChange ();

			PhotonNetwork.RaiseEvent (10, null, true, new RaiseEventOptions () {
				Receivers = ReceiverGroup.All,
				CachingOption = EventCaching.AddToRoomCache,
				ForwardToWebhook = true
			});
		}
	}

	public static void SetGamePhase(byte id, object content, int senderID){
		if (id == 10) {
			Debug.Log ("Setting game phase");
			if (GameHandler._instance.GamePhase == false) {
				Debug.Log ("Inside game phase");
				GameHandler._instance.GamePhase = true;
				Game._instance.ReBeginGame ();
			}
		}
	}

	public bool ReadyForChange(){
		int PlayersReady = 0;
		foreach (Player p in GameHandler._instance.GamePlayers) {
			if (p.isReady ()) {
				PlayersReady++;
			}
		}
		int gpc = GameHandler._instance.GamePlayers.Count - 1;
		if (PlayersReady == GameHandler._instance.GamePlayers.Count-1) {
			return true;
		}
		return false;
	}

	//start turn for player {content[1]} and end turn for player {content[2]}
	public static void setTurn(byte Newid, object content, int SenderID){
		if (Newid == 5) {
			int[] turns = (int[])content;
			int lastTurn = turns [0];
			int newTurn = turns [1];
			GameHandler._instance.playersTurn = newTurn;
			if (lastTurn == PhotonNetwork.player.ID) {
				Game._instance.EndTurn ();
			}
			if (newTurn == PhotonNetwork.player.ID) {
				Game._instance.StartTurn ();
			}
		}
	}

	public static void SyncGameState(byte id, object content, int SenderID){
		if (id == 9) {
			Hashtable ht = (Hashtable)content;
			PhotonNetwork.room.SetCustomProperties (ht, null, true);
			GameHandler._instance.UpdateTroopState (ht);
			Debug.Log ("RE SYNCING STATE");
			if (PhotonNetwork.player.ID == GameHandler._instance.playersTurn) {
				foreach (Trooper t in Game._instance.myPlayer.roster) {
					t.reset ();
				}
			}
		}
	}

	public static void EndPlacements(byte id, object content, int SenderID){
		if(id == 3){
			GameHandler._instance.getPlayer (SenderID).setReady(true);
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
