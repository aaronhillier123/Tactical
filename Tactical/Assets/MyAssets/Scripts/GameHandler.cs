using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {
	//handles macro game information such as turn stats and rosters


	public static GameHandler _instance;


	public List<Player> GamePlayers = new List<Player>();
	public GameObject PlayerObject;
	private int playersTurn = 0;
	private int turnNumber = 0;
	private bool GamePhase = false;
	public GameObject GameOverPanel;


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
				CameraPan._instnace.moveToObject (newPlayer.roster [0].gameObject);
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
		GameHandler._instance.refreshGameStates();
		//Raise Turn Change

		//SetDogTagsForPlayer
		Hashtable PlayerHt = new Hashtable();
		Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
		int myTags = myPlayer.getDogTags ();
		PlayerHt.Add ("DogTags", myTags);
		Debug.Log ("Sending new properties");
		PhotonNetwork.player.SetCustomProperties (PlayerHt, null, true);

		PhotonNetwork.RaiseEvent(5, null, true, new RaiseEventOptions(){
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.RemoveFromRoomCache});

		PhotonNetwork.RaiseEvent(5, turnObject, true, new RaiseEventOptions(){
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.AddToRoomCache,
			ForwardToWebhook = true});
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
				if(t.getPiece () != null){
					gamestate += t.getPiece ().id.ToString () + "/";
				} else {
						gamestate += "-1/";
				}
				if (t.isInvulnerable) {
					gamestate += "1";
				} else {
					gamestate += "0";
				}
				gamestate += " ";
			}
		}
		return gamestate;
	}

	public string CpState(){
		string cpstate = "";
		foreach (ControlPoint cp in Game._instance.allControlPoints()) {
			cpstate += cp.id + "/";
			cpstate += cp.team + " ";
		}
		return cpstate;
	}

	public void setMyTags(Hashtable ht){
		string myName = "Player" + PhotonNetwork.player.ID.ToString ();
		object myTagsOb;
		int myTags = 3;
		if (ht.TryGetValue (myName, out myTagsOb)) {
			myTags = (int)myTagsOb;
			Debug.Log ("my tag number is " + myTags);
		}

		Debug.Log ("Setting dog tags to " + myTags);

		Game._instance.myPlayer.setDogTags (myTags);
	}

	public void UpdateCpState(Hashtable ht){
		object cpObject;
		ht.TryGetValue ("Cps", out cpObject);
		string cpString = (string)cpObject;
		if(cpString !=null){
			string[] cpa = cpString.Split (' ');
			List<string> cps = new List<string> (cpa);
			cps.RemoveAt (cps.Count - 1);
			foreach (string s in cps) {
				UpdateCpFromCode (s);
			}
		}
	}

	public void UpdateCpFromCode(string cpCode){
		string[] cpa = cpCode.Split ('/');
		ControlPoint cp = Game._instance.getConrolPoint (int.Parse (cpa [0]));
		if (cp != null) {
			cp.setTeam (int.Parse (cpa [1]), 0);
		}
	}

	public void UpdateTroopState(Hashtable ht){
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
				
			//if a troop is not in gameState, remove all traces of it
			List<int> idsToRemove = new List<int>();
			foreach(Trooper tt in Game._instance.allTroopers){
				if (tt.isUpdated () == false) {
					idsToRemove.Add(tt.id);
				} else {
					tt.SetUpdated (false);
				}
			}
			foreach (int idd in idsToRemove) {
				Trooper gone = Game._instance.GetTroop (idd);
				Player gonePlayer = GameHandler._instance.getPlayer (gone.team);
				Game._instance.allTroopers.Remove (gone);
				gonePlayer.roster.Remove (gone);
				Destroy (gone.gameObject);
			}
		}
	}

	public void UpdateDogState(Hashtable ht){
		object dogObject;
		ht.TryGetValue ("Dogs", out dogObject);
		string dogsString = (string)dogObject;
		if (dogsString != null) {
			string[] da = dogsString.Split (' ');
			List<string> ds = new List<string> (da);
			ds.RemoveAt (ds.Count - 1);
			foreach (string s in ds) {
				UpdateDogFromCode (s);
			}

			//if a dogtag is not in gameState, remove all traces of it
			List<int> idsToRemove = new List<int>();
			foreach (DogTag dg in Game._instance.allDogTags) {
				if (dg.updated == false) {
					idsToRemove.Add (dg.id);
				} else {
					dg.updated = false;
				}
			}
			foreach (int idd in idsToRemove) {
				DogTag gone = Game._instance.GetTag (idd);
				Game._instance.allDogTags.Remove (gone);
				Destroy (gone.gameObject);
			}
		}
	}

	public void UpdateTroopFromCode(string troopCode){
		string[] tsa = troopCode.Split ('/');
		Trooper t = Game._instance.GetTroop (int.Parse(tsa[0]));
		//if troop exists, move to gameState position
		if (t != null) {
			t.team = int.Parse (tsa [1]);
			float x = float.Parse (tsa [3]);
			float y = float.Parse (tsa [4]);
			float z = float.Parse (tsa [5]);
			t.transform.position = new Vector3 (x, y, z);
			t.decreaseHealth (t.getHealth () - float.Parse (tsa [2]));
			float rx = float.Parse (tsa [6]);
			float ry = float.Parse (tsa [7]);
			float rz = float.Parse (tsa [8]);
			t.transform.eulerAngles = new Vector3 (rx, ry, rz);
			if (int.Parse (tsa [9]) != -1) {
				t.setPiece (BarrierHandler._instance.getPiece (int.Parse (tsa [9])));
				t.covering = true;
				t.setAnimation (11);
			}
			if (int.Parse (tsa [10]) == 1) {
				t.MakeInvulnerable ();
			} else {
				t.makeNotInvulnerable ();
			}
			t.SetUpdated (true);
		} else {
			//if troop doesnt exists, create a troop at specs
			float x = float.Parse (tsa [3]);
			float y = float.Parse (tsa [4]);
			float z = float.Parse (tsa [5]);
			int newid = int.Parse (tsa [0]);
			int newteam = int.Parse (tsa [1]);
			float rx = float.Parse (tsa [6]);
			float ry = float.Parse (tsa [7]);
			float rz = float.Parse (tsa [8]);
			Game._instance.myPlayer.CreateTroopAt (new Vector3 (x, y, z), Quaternion.Euler (new Vector3 (rx, ry, rz)), newteam, newid);
			Trooper newTroop = Game._instance.GetTroop (newid);
			if (int.Parse (tsa [9]) != -1) {
				newTroop.setPiece (BarrierHandler._instance.getPiece (int.Parse (tsa [9])));
				newTroop.covering = true;
				newTroop.transform.Rotate (new Vector3 (0, 180f, 0));
				Debug.Log ("setting to cover");
				newTroop.setAnimation (11);
			}
			if (int.Parse (tsa [10]) == 1) 
				t.MakeInvulnerable ();
			newTroop.SetUpdated (true);
		}
	}

	public void UpdateDogFromCode(string dogCode){
		string[] tsa = dogCode.Split ('/');
		DogTag dg = Game._instance.GetTag (int.Parse(tsa[0]));
		//if troop exists, move to gameState position
		if (dg != null) {
			float x = float.Parse (tsa [1]);
			float y = float.Parse (tsa [2]);
			float z = float.Parse (tsa [3]);
			dg.transform.position = new Vector3 (x, y, z);
			dg.updated = true;
		} else {
			//if troop doesnt exists, create a troop at specs
			float x = float.Parse (tsa [1]);
			float y = float.Parse (tsa [2]);
			float z = float.Parse (tsa [3]);
			int newid = int.Parse (tsa [0]);

			Game._instance.myPlayer.CreateDogTagAt (new Vector3 (x, y, z), newid);
			Game._instance.GetTag (newid).updated = true;
		}
	}

	public string dogState(){
		string dogstate = "";
		foreach (DogTag d in Game._instance.allDogTags) {
			dogstate += d.id + "/";
			dogstate += d.transform.position.x.ToString () + "/";
			dogstate += d.transform.position.y.ToString () + "/";
			dogstate += d.transform.position.z.ToString () + " ";
		}
		return dogstate;
	}

	public Hashtable GetGameState(){
		string troopstate = TroopState ();
		string dogstate = dogState ();
		string cpState = CpState ();

		int myTags = Game._instance.myPlayer.getDogTags ();
		string myName = "Player" + PhotonNetwork.player.ID.ToString ();
		Hashtable ht = new Hashtable ();
		ht.Add ("Troops", troopstate);
		ht.Add ("Dogs", dogstate);
		ht.Add ("Turn", turnNumber);
		ht.Add ("Cps", cpState);
		ht.Add (myName, myTags);
		return ht;
	}
		

	public void RaiseEndPlacements(){
		PhotonNetwork.RaiseEvent(3, null, true, AllReceivers());
		BarrierHandler._instance.PlaceAllBarriers ();
		HudController._instance.removeStartHud ();
		Game._instance.SendBarriersToNetwork ();
		BarrierHandler._instance.RemoveAllPrelimbs ();
		Debug.Log ("ENDING ALL PLACEMENTS");
		PhotonNetwork.RaiseEvent (11, null, true, new RaiseEventOptions () {
			CachingOption = EventCaching.AddToRoomCache,
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true
		});


		//Game._instance.BeginGame ();
		GameHandler._instance.GamePhase = true;
		if (GameHandler._instance.ReadyForChange ()) {
			RaiseTurnChange ();
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
			Debug.Log ("sending new properties 2");
			PhotonNetwork.room.SetCustomProperties (ht, null, true);
			GameHandler._instance.UpdateTroopState (ht);
			GameHandler._instance.UpdateDogState (ht);
			GameHandler._instance.UpdateCpState (ht);
			GameHandler._instance.setMyTags (ht);
		}
	}

	public void CheckForEnd(){
		int playersLeft = 0;
		int winner = 0;
		foreach (Player p in GameHandler._instance.GamePlayers) {
			if (p.roster.Count > 0) {
				playersLeft++;
				winner = p.team;
			}
		}
		if (playersLeft == 1) {
			PhotonNetwork.RaiseEvent ((byte)14, (object)winner, true, new RaiseEventOptions () {
				Receivers = ReceiverGroup.All,
				ForwardToWebhook = true, 
				CachingOption = EventCaching.AddToRoomCache,
			});
		}
	}

	public void showGameOver(int win){
		GameObject winObject = Instantiate (GameOverPanel, GameObject.Find ("Canvas").transform, false);
		winObject.GetComponent<GameOver> ().show (win);

	}

	public static void EndGame(byte id, object content, int senderID){
		if (id == 14) {
			Game._instance.over = true;
			int winner = (int)content;
			Button[] allButons = GameObject.FindObjectsOfType<Button> ();
			foreach (Button b in allButons) {
				b.interactable = false;
			}
			HudController._instance.removeWaitingScreen ();
			if (PhotonNetwork.player.ID == winner) {
				GameHandler._instance.showGameOver (1);

			} else {
				///you are loser
				GameHandler._instance.showGameOver(0);
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
