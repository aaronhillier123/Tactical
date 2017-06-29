using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

	public int team;
	public GameObject TrooperObject;
	public GameObject HealthObject;
	public List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;
	public Game game;
	public static int numberOfTroops = 2;
	public bool attacking = false;
	//public Game game;







	// Use this for initialization
	void Start () {
		game = GameObject.FindObjectOfType(typeof(Game)) as Game;
		PhotonNetwork.OnEventCall += attack;
	}
	
	// Update is called once per frame
	void Update () {
	}



	public void CreateTroopAt(Vector3 location, int troopTeam, int troopId){
		Debug.Log ("Creating Troop");
		GameObject FirstTroopObject = Instantiate (TrooperObject, location, Quaternion.identity, transform) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		if (firstTroop != null) {
			firstTroop.team = troopTeam;
			firstTroop.id = troopId;

			Game.allTroopers.Add (firstTroop);
			Debug.Log ("Game size is now " + Game.allTroopers.Count);
			roster.Add (firstTroop);
		}
	}

	public void attack(byte id, object content, int senderID){
		if(id == 4){
			Debug.Log ("attack recieved by all");
			float[] contents = (float[])content;
			Trooper myTroop = Game.GetTroop ((int)contents [0]);
			Trooper enemy = Game.GetTroop((int)contents[1]);
			Debug.Log (myTroop.id + " is shooting " + enemy.id);
			myTroop.rotateTo (enemy.gameObject.transform.position);
			myTroop.animator.SetInteger ("AnimPar", 2);
			enemy.health -= 20;
			myTroop.Invoke ("stop", 1f);
			myTroop.unselect ();
			myTroop.freeze ();
		}
	}
		

	public void selectTrooper(Trooper a){
		a.select();
	}
}
