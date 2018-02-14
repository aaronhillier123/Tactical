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

	//all troops in player roster

	public List<Trooper> roster = new List<Trooper>();


	private Trooper Selected;
	private Barrier barrierSelected;


	//general player variables
	public static int numberOfTroops = 5;
	private int dogtags = 3;
	public List<ControlPoint> myControlPoints;

	//booleans for player states
	private bool attacking = false;
	public bool ready = false;

	public Trooper getSelected(){
		return Selected;
	}
	public void setSelected(Trooper s){
		Selected = s;
	}
	public Barrier getBarrierSelected(){
		return barrierSelected;
	}
	public void setBarrierSelected(Barrier b){
		barrierSelected = b;
	}
	public int getDogTags(){
		return dogtags;
	}
	public void addDogTags(int d){
		dogtags = dogtags + d;
	}
	public bool isAttacking(){
		return attacking;
	}
	public void setAttacking(bool a){
		attacking = a;
	}
	public bool isReady(){
		return ready;
	}
	public void setReady(bool r){
		ready = r;
	}

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
			firstTroop.setInPos(firstTroop.gameObject.transform.position);
			Game._instance.allTroopers.Add (firstTroop);
			roster.Add (firstTroop);
		}
	}

	public void addControlPoint(ControlPoint cp){
		myControlPoints.Add(cp);
	}

	public void removeControlPoint(ControlPoint cp){
		myControlPoints.Remove (cp);
	}
		
	public void RaiseAttack(Trooper EnemyTroop){
		attacking = false;
		Selected.freeze ();
		HudController._instance.removeChances ();

		float distance = Vector3.Distance (Selected.transform.position, EnemyTroop.transform.position);
		float hit = (Random.Range (0, Selected.getRange()) - distance) > 0 ? 1 : 0;
		Vector3 enemypos = EnemyTroop.transform.position + new Vector3(0f, 3f, 0f);

		//under cover may be blocked by barrier
		if (EnemyTroop.getPiece() != null) {
			Vector3 enemyCenter = EnemyTroop.GetComponent<CapsuleCollider> ().bounds.center;
			Vector3 barrierCenter = EnemyTroop.getPiece().GetComponent<BoxCollider> ().bounds.center;
			float DistanceToEnemy = Vector3.Distance (Selected.transform.position, enemyCenter);
			float DistanceToBarrier = Vector3.Distance (Selected.transform.position, barrierCenter);
			if (DistanceToEnemy > DistanceToBarrier) {
				hit = 2;
			}
		}

		float[] targets = new float[3];
		targets [0] = Selected.id;
		targets [1] = EnemyTroop.id;
		targets [2] = hit;
		Selected.DidSomething ();
		object target = (object)targets;
		PhotonNetwork.RaiseEvent (4, target, true, GameHandler._instance.AllReceivers());
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

	public void CreateDogTagAt(Vector3 pos, int dogId){
		GameObject newDogTag = Instantiate (TroopController._instance.TroopObjects[4], pos, Quaternion.identity);
		DogTag dt = newDogTag.GetComponent<DogTag> ();
		dt.id = dogId;
		Game._instance.allDogTags.Add(newDogTag.GetComponent<DogTag>());
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
