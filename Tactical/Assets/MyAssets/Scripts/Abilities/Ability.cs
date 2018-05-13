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

	public Trooper myTroop;

	public int phase = 0;
	public bool hasControl = false;

	public abstract void giveControl ();
	public abstract void removeControl();
	public abstract void execute (Vector3 target);

}