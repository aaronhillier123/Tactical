﻿using System.Collections;
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
			firstTroop.myPlayer = GameHandler._instance.getPlayer (troopTeam);
			firstTroop.setInPos(firstTroop.gameObject.transform.position);
			firstTroop.assignColor ();
			Game._instance.allTroopers.Add (firstTroop);
			roster.Add (firstTroop);
		}
	}

	public void RaiseTroopAt(Vector3 location, Quaternion eulersQ, int troopTeam, int troopId){
		float[] CA = new float[8];
		CA [0] = location.x;
		CA [1] = location.y;
		CA [2] = location.z;
		Vector3 eulers = eulersQ.eulerAngles;
		CA [3] = eulers.x;
		CA [4] = eulers.y;
		CA [5] = eulers.z;
		CA [6] = (int)troopTeam;
		CA [7] = (int)troopId;
		object content = (object)CA;
		PhotonNetwork.RaiseEvent ((byte)13, content, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true
		});
	}

	public static void NetworkTroopAt(byte id, object content, int senderID){
		if (id == 13) {
			float[] CA = (float[])content;
			Vector3 pos = new Vector3 (CA [0], CA [1], CA [2]);
			Vector3 eul = new Vector3 (CA [3], CA [4], CA [5]);
			int tteam = (int)CA [6];
			int tid = (int)CA [7];
			Quaternion rot = Quaternion.Euler (eul);
			Game._instance.myPlayer.CreateTroopAt (pos, rot, tteam, tid);
			CameraController._instance.setFollowedObject (Game._instance.GetTroop (tid).gameObject, 1);
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
