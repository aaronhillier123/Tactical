using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	// Static variables for game state
	public static List<Player> GamePlayers = new List<Player>();
	public GameObject PlayerObject;
	public static GameObject Player1;
	public static List<Trooper> allTroopers = new List<Trooper> ();
	public static List<DogTag> allDogTags = new List<DogTag> ();
	public static int playersTurn;

	//for click and drag differential
	private float firstTime;
	private float secondTime;
	private float timeDiff;
	private bool mouseDown = false;
	private bool dragOccuring = false;
	private Vector3 previousMousePosition;

	void Start () {
		Player1 = PlayerObject;
		PhotonNetwork.OnEventCall += CreatePlayer;
		PhotonNetwork.OnEventCall += Player.throwGrenade;
		PhotonNetwork.OnEventCall += Player.attack;
		PhotonNetwork.OnEventCall += Trooper.makeInvulnerable;
		PhotonNetwork.OnEventCall += Trooper.move;
		playersTurn = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (dragOccuring == true) {

			//adjust camera from drag
			//if (previousMousePosition == null) {
			//	previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			//} else {
				Vector3 camDifference = Camera.main.ScreenToViewportPoint(Input.mousePosition) - previousMousePosition;
				Camera.main.transform.parent.transform.Translate (camDifference.y * 30f, 0f, camDifference.x * -30f);
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			//}
		}

		//decide if click or drag happens
		if (Input.GetMouseButtonDown (0)) {
			previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			firstTime = Time.time;
			mouseDown = true;
		}
		if (mouseDown == true) {
			timeDiff = Time.time - firstTime;
		}
		if (timeDiff > 0f) {
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
				dragOccuring = true;
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			mouseDown = false;
			dragOccuring = false;
			if (timeDiff < .2f) {
				OnClick ();
			}
			timeDiff = 0;
		}
	}

	//if click happens
	void OnClick (){
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (-1)) {

		} else { 
			if (playersTurn == PhotonNetwork.player.ID) {
				Player myPlayer = getPlayer (PhotonNetwork.player.ID);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100)) {

					//if player is clicked
					if (hit.collider.CompareTag ("Player")) {
						Trooper clickedOn = hit.collider.gameObject.GetComponent<Trooper> ();
						if (PhotonNetwork.player.ID == clickedOn.team) {
							//if it is an ally trooper
							myPlayer.selectTrooper (clickedOn);
						} else {
							if (myPlayer.Selected != null) {
								//if it is an enemy player
								if (myPlayer.attacking == true) {
									//if current player is attacking

									myPlayer.attacking = false;
									float[] targets = new float[3];
									targets [0] = myPlayer.Selected.id;
									targets [1] = clickedOn.id;
									if (Game.GetTroop (myPlayer.Selected.id).isSniper == false) {
										targets [2] = Random.Range (0, 100);
									} else {
										targets [2] = Random.Range (0, 200);
									}
									object target = (object)targets;
									PhotonNetwork.RaiseEvent (4, target, true, EventHandler.ops);
									myPlayer.removeChances ();
									//myPlayer.Selected.noAttackMode ();
								}
							}else {
								if (myPlayer.Selected != null) {
									//if current player is not attacking
									if (myPlayer.Selected.hasGrenade) {
										//if player is carrying a grenade
										myPlayer.Selected.resetDistance ();
										float[] contents1 = new float[4];
										contents1 [0] = (float)myPlayer.Selected.id;
										contents1 [1] = hit.point.x;
										contents1 [2] = hit.point.y;
										contents1 [3] = hit.point.z;
										object contents = (object)contents1;
										PhotonNetwork.RaiseEvent ((byte)6, contents, true, EventHandler.ops);
									} else {
										Hud.showHealthBar (clickedOn.id);
									}
								}
							}
						}
					} else if (hit.collider.CompareTag ("Terrain") || hit.collider.CompareTag("ControlPoint") || hit.collider.CompareTag("NaturalCover")) {
						//if player clicked on terrain/ground
						if (myPlayer.Selected != null) {
							//if player is selected
							if (myPlayer.Selected.hasGrenade) {
								//if grenade is equipped
								myPlayer.Selected.resetDistance();
								float[] contents1 = new float[4];
								contents1 [0] = (float)myPlayer.Selected.id;
								contents1 [1] = hit.point.x;
								contents1 [2] = hit.point.y;
								contents1 [3] = hit.point.z;
								object contents = (object)contents1;
								PhotonNetwork.RaiseEvent ((byte)6, contents, true, EventHandler.ops);
							} else {
								//regular player movement

								RaiseEventOptions ops = RaiseEventOptions.Default;
								ops.Receivers = ReceiverGroup.All;
								float[] contents1 = new float[5];
								contents1 [0] = (float)myPlayer.Selected.id;
								if (Vector3.Distance (hit.point, myPlayer.Selected.initialPosition) <= myPlayer.Selected.maxDistance) {
									//if the click point is within the troops walking distance
									contents1 [1] = hit.point.x;
									contents1 [2] = hit.point.y;
									contents1 [3] = hit.point.z;
									if (hit.collider.CompareTag ("ControlPoint")) {
										//if click is also a control point
										contents1 [4] = (float) hit.collider.gameObject.transform.parent.gameObject.GetComponent<ControlPoint> ().id;
									} else {
										contents1 [4] = -1f;
									}
								} else {
									//find farthest point that troop can currently travel
									Trooper myTroop = GetTroop(myPlayer.Selected.id);
									Vector3 myPoint = ((hit.point - myTroop.initialPosition).normalized) * myTroop.maxDistance;
									contents1 [1] = myTroop.initialPosition.x + myPoint.x;
									contents1 [2] = myTroop.initialPosition.y + myPoint.y;
									contents1 [3] = myTroop.initialPosition.z + myPoint.z;
									contents1 [4] = -1f;
									}
									object contents = (object)contents1;
									PhotonNetwork.RaiseEvent ((byte)2, contents, true, ops);
									myPlayer.Selected.noAttackMode ();
								}
							}
					}
				}
			}
		}
	}


	//list of all troopers who are NOT a specifc player's
	public static List<Trooper> notMyTroopers(Player p){
		List<Trooper> nmt = new List<Trooper> ();
		foreach(Trooper t in allTroopers){
			if(t.team != p.team){
				nmt.Add(t);
			}
		}
		return nmt;
	}
		
			
			
	//create and initialize a global player
	public static void CreatePlayer(byte id, object content, int senderID){
		if (id == 1) {
			Vector3 pos = new Vector3 (0, 0, senderID * 50);
			GameObject newPlayerObject = Instantiate (Player1, pos, Quaternion.identity) as GameObject;
			Player newPlayer = newPlayerObject.GetComponent<Player> ();
			newPlayer.team = senderID;
			GamePlayers.Add (newPlayer);
			GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
			SpawnArea mySpawn = new SpawnArea ();
			foreach (GameObject g in spawns) {
				if (g.GetComponent<SpawnArea> ().team == newPlayer.team) {
					mySpawn = g.GetComponent<SpawnArea>();
				}
			}

			for (int i = 0; i < Player.numberOfTroops; ++i) {
				Debug.Log("creating troop number " + i + " for team " + senderID);
				Vector3 newPos = mySpawn.spawnPoints [i];
				newPlayer.CreateTroopAt (newPos, senderID, ((senderID-1) * Player.numberOfTroops) + i);
			}

		}
	}

	//End current turn and go to next player
	public void changeTurn(){
		Debug.Log ("Changing turn");
		if (playersTurn < 4) {
			playersTurn++;
		} else {
			playersTurn = 0;
		}
	}

	public static ControlPoint getConrolPoint(int idd){
		ControlPoint[] allObs = GameObject.FindObjectsOfType (typeof(ControlPoint)) as ControlPoint[];
		foreach(ControlPoint g in allObs){
			if(g.id == idd){
				Debug.Log ("reutnring control point " + g.id);
				return g;
			}
		}
		Debug.Log("returning null because size is" + allObs.Length);
		return null;
	}

	//Return trooper based on ID
	public static Trooper GetTroop(int id){
		foreach (Trooper t in allTroopers){
			if(t.id == id){
				return t;
			}
		}
		return null;
	}

	//Return player based on ID
	public static Player getPlayer(int id){
		foreach (Player p in GamePlayers) {
			if (p.team == id) {
				return p;
			}
		}
		return null;
	}

}
