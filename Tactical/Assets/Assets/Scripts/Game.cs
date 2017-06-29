using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	// Use this for initialization
	public static List<Player> GamePlayers = new List<Player>();
	public GameObject PlayerObject;
	public static GameObject Player1;
	public static List<Trooper> allTroopers = new List<Trooper> ();
	public static int playersTurn;


	private float firstTime;
	private float secondTime;
	private float timeDiff;
	private bool mouseDown = false;
	private bool dragOccuring = false;
	private Vector3 previousMousePosition;

	void Start () {
		Player1 = PlayerObject;
		PhotonNetwork.OnEventCall += CreatePlayer;
		playersTurn = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (dragOccuring == true) {

			//adjust camera from drag
			if (previousMousePosition == null) {
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			} else {
				Vector3 camDifference = Camera.main.ScreenToViewportPoint(Input.mousePosition) - previousMousePosition;
				Camera.main.transform.parent.transform.Translate (camDifference.y * 20f, 0f, camDifference.x * -20f);
				previousMousePosition = previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			}
		}
		//first click time
		if (Input.GetMouseButtonDown (0)) {


			previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			firstTime = Time.time;
			mouseDown = true;
		}

		if (mouseDown == true) {
			timeDiff = Time.time - firstTime;
		}


		if (timeDiff > 0f) {
			dragOccuring = true;
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
							myPlayer.selectTrooper (clickedOn);
						} else {
							float[] targets = new float[3];
							targets [0] = myPlayer.Selected.id;
							targets [1] = clickedOn.id;
							targets [2] = Random.Range (0, 100);
							object target = (object)targets;
							PhotonNetwork.RaiseEvent (4, target, true, EventHandler.ops);
						}
					} else if (hit.collider.CompareTag ("Terrain")) {
						if (myPlayer.Selected != null) {


							//if terrain is clicked while player is selected
							RaiseEventOptions ops = RaiseEventOptions.Default;
							ops.Receivers = ReceiverGroup.All;
							///////send to server
							Debug.Log ("Trying to move now");
							float[] contents1 = new float[4];
							contents1 [0] = (float)myPlayer.Selected.id;
							contents1 [1] = hit.point.x;
							contents1 [2] = hit.point.y;
							contents1 [3] = hit.point.z;
							object contents = (object)contents1;
							PhotonNetwork.RaiseEvent ((byte)2, contents, true, ops);
						}
					}
				}
			}
		}
	}



	public static void CreatePlayer(byte id, object content, int senderID){
		if (id == 1) {
			Vector3 pos = new Vector3 (0, 0, senderID * 50);
			GameObject newPlayerObject = Instantiate (Player1, pos, Quaternion.identity) as GameObject;
			Player newPlayer = newPlayerObject.GetComponent<Player> ();
			newPlayer.team = senderID;
			GamePlayers.Add (newPlayer);
			for (int i = 0; i < Player.numberOfTroops; ++i) {
				Debug.Log("creating troop number " + i + " for team " + senderID);
				Vector3 newPos = new Vector3 (pos.x + 5 + (5*i), pos.y, pos.z);
				newPlayer.CreateTroopAt (newPos, senderID, ((senderID-1) * Player.numberOfTroops) + i);
			}
		}
	}

	public void changeTurn(){
		Debug.Log ("Changing turn");
		if (playersTurn < 4) {
			playersTurn++;
		} else {
			playersTurn = 0;
		}
	}

	public static Trooper GetTroop(int id){
		foreach (Trooper t in allTroopers){
			if(t.id == id){
				return t;
			}
		}
		return null;
	}

	public static Player getPlayer(int id){
		foreach (Player p in GamePlayers) {
			if (p.team == id) {
				return p;
			}
		}
		return null;
	}

}
