using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

	//identification
	public int team;

	//Prefabs needed
	public GameObject TrooperObject;
	public GameObject HealthObject;
	public GameObject ChanceObject;


	//state of player's troops
	public List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;


	public Game game;

	//number of starting trops
	public static int numberOfTroops = 2;


	//booleans for player states
	public bool attacking = false;

	// Use this for initialization
	void Start () {
		game = GameObject.FindObjectOfType(typeof(Game)) as Game;
		//PhotonNetwork.OnEventCall += attack;
		//PhotonNetwork.OnEventCall += throwGrenade;
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
		}
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
		if(id == 4){
			//unpack objects for attacker and target

			float[] contents = (float[])content;
			Trooper myTroop = Game.GetTroop ((int)contents [0]);
			Trooper enemy = Game.GetTroop((int)contents[1]);
			myTroop.isSniper = false;
			//find distance and see if attack is a hit
			float distance = Vector3.Distance(myTroop.gameObject.transform.position, enemy.gameObject.transform.position);
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
			myTroop.Invoke ("stop", 1f);

			myTroop.unselect ();
			myTroop.freeze ();
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
