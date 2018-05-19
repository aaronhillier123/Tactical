using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour {

	public string name = "";
	public string description = "";
	public int price = 0;
	public int id = 0;
	public Sprite art;
	public GameObject[] models;
	public bool passive;

	public Trooper myTroop;

	public int phase = 0;
	public bool hasControl = false;
	void Update(){
		inspect ();
	}

	public abstract void inspect ();
	public void giveControl (){
		hasControl = true;
		myTroop.activeAbility = gameObject;
	}
	public abstract void removeControl();
	public abstract void execute (Vector3 target);
	public abstract void sell ();
	public abstract void passiveExecute(RaycastHit hit);
	public void terminate(){
		myTroop.removeAbility (id);
	}
}