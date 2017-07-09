using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	//identification
	public int team;
	public int lookingAt = 0;
	//Prefabs needed
	public GameObject TrooperObject;
	public GameObject HealthObject;
	public GameObject ChanceObject;
	public GameObject WaitingScreen;

	//state of player's troops
	public List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;


	public Game game;

	//general player variables
	public static int numberOfTroops = 2;
	public int dogtags = 3;
	public List<ControlPoint> myControlPoints;

	//booleans for player states
	public bool attacking = false;

	// Use this for initialization
	void Start () {
		game = GameObject.FindObjectOfType(typeof(Game)) as Game;
		//PhotonNetwork.OnEventCall += attack;
		//PhotonNetwork.OnEventCall += throwGrenade;
		if (PhotonNetwork.player.ID == Game.playersTurn) {
			GameObject.Find ("NextTurnButton").GetComponent<Button> ().interactable = true;
			GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = false;
			GameObject ws = GameObject.Find ("NotTurnPanel(Clone)");
			if (ws != null) {
				Destroy (ws);
			}
		} else {
			GameObject ws = GameObject.Find ("NotTurnPanel(Clone)");
			if (ws == null) {
				Instantiate (WaitingScreen, GameObject.Find ("Canvas").transform);
			}
			GameObject.Find ("NextTurnButton").GetComponent<Button> ().interactable = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void onStartTurn(){
		foreach (Trooper t in roster) {
			if (t.isInvulnerable) {
				t.makeNotInvulnerable ();
			}
			t.initialPosition = t.gameObject.transform.position;
			t.maxDistance = 50f;
		}
		if (PhotonNetwork.player.ID == Game.playersTurn) {
			GameObject.Find ("NextTurnButton").GetComponent<Button> ().interactable = true;
			GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = false;
			GameObject ws = GameObject.Find ("NotTurnPanel(Clone)");
			GameObject.Find ("CameraPan").GetComponent<CameraPan> ().moveToPlayer (roster [lookingAt]);
			if (ws != null) {
				Destroy (ws);
			}
		} else {
			Instantiate (WaitingScreen, GameObject.Find ("Canvas").transform);
			GameObject.Find ("NextTurnButton").GetComponent<Button> ().interactable = false;
		}
		dogtags += (myControlPoints.Count * 2);
		Hud.updateDogTags (dogtags);
	}

	public void spendDogTags(int amount){
		dogtags -= amount;
		Hud.updateDogTags (dogtags);
	}

	//create a new troop at a certain location
	public void CreateTroopAt(Vector3 location, int troopTeam, int troopId){
		Debug.Log ("Creating Troop");
		GameObject FirstTroopObject = Instantiate (TrooperObject, location, Quaternion.identity, transform) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		if (firstTroop != null) {
			firstTroop.team = troopTeam;
			firstTroop.id = troopId;
			firstTroop.initialPosition = firstTroop.gameObject.transform.position;
			Game.allTroopers.Add (firstTroop);
			roster.Add (firstTroop);
		}
	}

	public void addControlPoint(ControlPoint cp){
		myControlPoints.Add(cp);
	}

	//show all percentages of hits from selected troop to other players' troops
	public void showAllChances(){
		List<Trooper> others = Game.notMyTroopers (this);
		foreach (Trooper t in others) {
			GameObject chanceO = Instantiate (ChanceObject, GameObject.Find ("Canvas").transform);
			chanceO.GetComponent<Chance> ().target = Selected;
			chanceO.GetComponent<Chance> ().id = t.id;
		}
	}

	//remove percentages
	public void removeChances(){
		Chance[] all = GameObject.FindObjectsOfType<Chance> ();
		foreach (Chance c in all) {
			Destroy (c.gameObject);
		}
	}

	//network attack function
	public static void attack(byte id, object content, int senderID){
		if (id == 4) {
			//unpack objects for attacker and target

			float[] contents = (float[])content;
			Trooper myTroop = Game.GetTroop ((int)contents [0]);
			Trooper enemy = Game.GetTroop ((int)contents [1]);
			myTroop.isSniper = false;

			RaycastHit hit;
			Vector3 enemypos = enemy.gameObject.transform.position;
			Vector3 mypos = myTroop.gameObject.transform.position;
			Vector3 enemyhip = new Vector3 (enemypos.x, enemypos.y + 3, enemypos.z);
			Vector3 myhip = new Vector3 (mypos.x, mypos.y + 3, mypos.z);
			Vector3 dir = (enemyhip - myhip);
			if (Physics.Raycast (myhip, dir, out hit, 200f)) {
				Debug.Log ("Collided with " + hit.collider.tag);
				Debug.DrawRay (myhip, dir, Color.white, 3f);
				if (hit.collider.CompareTag ("NaturalCover")) {
					myTroop.rotateTo (enemy.gameObject.transform.position);
					myTroop.animator.SetInteger ("AnimPar", 2);
					myTroop.miss (enemy.gameObject);
				} else {
					//find distance and see if attack is a hit
					float distance = Vector3.Distance (myTroop.gameObject.transform.position, enemy.gameObject.transform.position);
					float random = contents [2];
					if (random - distance > 0) {
						//Debug.Log ("its a hit, distance is " + distance + " and random is " + random);
						myTroop.rotateTo (enemy.gameObject.transform.position);
						myTroop.animator.SetInteger ("AnimPar", 2);
						myTroop.shoot (enemy.gameObject);
			
					} else {
						//Debug.Log ("its a miss,  distance is " + distance + " and random is " + random);
						myTroop.rotateTo (enemy.gameObject.transform.position);
						myTroop.animator.SetInteger ("AnimPar", 2);
						myTroop.miss (enemy.gameObject);
					}
				}
					myTroop.Invoke ("stop", 1f);

					myTroop.unselect ();
					myTroop.freeze ();
					myTroop.noAttackMode ();
				}
			}
		}

	public static void throwGrenade(byte id, object content, int senderID){
		if (id == 6) {
			float[] conList = (float[])content;
			int selectedID = (int)conList [0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game.GetTroop (selectedID);
			myTroop.animator.SetInteger ("AnimPar", 3);
			myTroop.throwGrenade (newPos);
			myTroop.unselect ();
		}
	}
		
	//select a specific trooper
	public void selectTrooper(Trooper a){
		a.select();
	}
}
