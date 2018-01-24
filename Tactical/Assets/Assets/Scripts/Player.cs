using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	//identification
	public int team;
	public int lookingAt = 0;
	private bool isTurnBool = false;
	//Prefabs needed
	public GameObject TrooperObject;
	public GameObject HealthObject;
	public GameObject ChanceObject;
	public GameObject WaitingScreen;
	public GameObject HudObject;
	public GameObject StartHudOb;


	//state of player's troops
	public List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;
	public Barrier barrierSelected;
	public List<GameObject> barriers = new List<GameObject> ();

	//general player variables
	public static int numberOfTroops = 3;
	public int dogtags = 3;
	public List<ControlPoint> myControlPoints;

	//booleans for player states
	public bool attacking = false;
	public bool ready = false;


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	}

	public void spendDogTags(int amount){
		dogtags -= amount;
		Hud.updateDogTags (dogtags);
	}

	//create a new troop at a certain location
	public void CreateTroopAt(Vector3 location, Quaternion rotation, int troopTeam, int troopId){
		GameObject FirstTroopObject = Instantiate (TrooperObject, location, rotation, transform) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		if (firstTroop != null) {
			firstTroop.team = troopTeam;
			firstTroop.id = troopId;
			firstTroop.initialPosition = firstTroop.gameObject.transform.position;
			Game._instance.allTroopers.Add (firstTroop);
			roster.Add (firstTroop);
		}
	}

	public void addControlPoint(ControlPoint cp){
		myControlPoints.Add(cp);
	}
		
	public void RaiseAttack(Trooper EnemyTroop){
		attacking = false;
		Selected.freeze ();
		HudController._instance.removeChances ();

		float distance = Vector3.Distance (Selected.transform.position, EnemyTroop.transform.position);
		float hit = (Random.Range (0, Selected.range) - distance) > 0 ? 1 : 0;
		Vector3 enemypos = EnemyTroop.transform.position + new Vector3(0f, 3f, 0f);

		float[] targets = new float[3];
		targets [0] = Selected.id;
		targets [1] = EnemyTroop.id;
		targets [2] = hit;
		Selected.DidSomething ();
		object target = (object)targets;
		PhotonNetwork.RaiseEvent (4, target, true, EventHandler.ops);
	}

	//network attack function
	public static void attack(byte id, object content, int senderID){
		if (id == 4) {
			//unpack objects for attacker and target
			float[] contents = (float[])content;
			Trooper myTroop = Game._instance.GetTroop ((int)contents [0]);
			Trooper Enemy = Game._instance.GetTroop ((int)contents [1]);
			int hit = (int)contents [2];
			myTroop.shoot (Enemy, hit);
			}
		}


	public void RaiseGrenade(Vector3 point){

		float[] contentsFloat = new float[] {(float)Selected.id, point.x, point.y, point.z};
		object contents = (object)contentsFloat;
		Selected.DidSomething ();
		PhotonNetwork.RaiseEvent ((byte)6, contents, true, EventHandler.ops);
	}

	//network grenade function
	public static void throwGrenade(byte id, object content, int senderID){
		if (id == 6) {
			float[] conList = (float[])content;
			Trooper myTroop = Game._instance.GetTroop ((int)conList[0]);
			Vector3 newPos = new Vector3 (conList[1], conList[2], conList[3]);
			myTroop.throwGrenade (newPos);
		}
	}

	public void selectTrooper(Trooper a){
		a.select();
	}

	public bool isTurn(){
		return isTurnBool;
	}

	public void setTurn(bool isMyTurn){
		isTurnBool = isMyTurn;
	}

}
