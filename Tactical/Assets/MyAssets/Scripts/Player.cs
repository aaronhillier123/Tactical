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
	private int dogtags;
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
		HudController._instance.updateDogTags (dogtags);
	}
	public void setDogTags(int d){
		dogtags = d;
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
		HudController._instance.updateDogTags (dogtags);
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

	public static void airStrike(byte id, object content, int senderID){
		if (id == 12) {
			Debug.Log ("In airstrike network function");
			float[] cA = (float[])content;
			Vector3 point = new Vector3 (cA [1], cA [2], cA [3]);
			Trooper t = Game._instance.GetTroop( (int)cA [0]);
			t.CallAirstrike(point);
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
