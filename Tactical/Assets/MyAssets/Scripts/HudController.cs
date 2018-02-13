﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour {

	public static HudController _instance;
	public Player myPlayer;

	public GameObject HealthObject;
	public GameObject ChanceObject;
	public GameObject HitOrMissObject;


	public GameObject GameHudObject;
	public GameObject StartHudObject;
	public GameObject WaitingHudObject;
	public Canvas canvas;

	public Hud GameHud;
	public StartHud StartHud;

	private bool healthbars = false;
	public static string currentItem;


	// Use this for initialization
	void Start () {
		_instance = this;
		GameHud = GameHudObject.GetComponent<Hud> ();
		StartHud = StartHudObject.GetComponent<StartHud> ();
	}

	public void showInfoPanel(int item){
		GameHud.showInfoPanel (item);
	}

	public void removeInfoPanel(){
		GameHud.removeInfoPanel ();
	}

	public void AttackMode(bool attack){
		if(GameHud.GetComponent<Hud>() != null){
			myPlayer.setAttacking (attack);
			GameHud.AttackMode(attack);
		}
	}

	public void CanAttack(bool attack){
		if(GameHud.GetComponent<Hud>() != null){
			GameHud.CanAttack (attack);
		}
	}

	public void Retract(){
		GameHud.Retract ();
	}

	public void StartTurn(){
		HudController._instance.removeWaitingScreen ();
		AttackMode (false);
		GameHud.StartTurn ();
	}

	public void EndTurn(){
		HudController._instance.showWaitingScreen ();
		AttackMode (false);
		GameHud.EndTurn ();
	}

	public void RefreshStore(){
		GameHud.RefreshStore();
	}

	public void removeStartHud(){
		StartHud.transform.SetParent (null);
	}

	public void removeGameHud(){
		GameHud.transform.SetParent (null);
	}

	public void showStartHud(){
		StartHud.transform.SetParent (canvas.transform);
		StartHud.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
	}

	public void showGameHud(){
		GameHud.transform.SetParent (canvas.transform);
		GameHud.transform.localPosition = new Vector3 (0, 0, 0);
	}

	public void showWaitingScreen(){
		WaitingHudObject.transform.SetParent (canvas.transform);
		WaitingHudObject.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);

	}

	public void removeWaitingScreen(){
		WaitingHudObject.transform.SetParent (null);
	}

	public void BeginGame(){
		HudController._instance.removeStartHud ();
		HudController._instance.showGameHud ();
		HudController._instance.showWaitingScreen ();
	}

	// Update is called once per frame
	void Update () {
		if(myPlayer==null){
			try{
				GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
			} catch{
				Debug.Log ("NO PLAYERS");
			}
		}
	}

	//show all health bars
	public void showHealthBars(){
		if (healthbars == false) {
			hideHealthBars ();
			foreach (Trooper t in Game._instance.allTroopers) {
				GameObject myHealth = Instantiate (HealthObject) as GameObject;
				myHealth.GetComponent<HealthBar> ().id = t.id;
			}
			healthbars = true;
		} else {
			hideHealthBars ();
		}
	}

	//show single health bar
	public void showHealthBar(int id){
		removeHealthBar (id);
		GameObject myHealth = Instantiate (HealthObject) as GameObject;
		myHealth.GetComponent<HealthBar>().id = id;
	}

	//hide all health bars
	public void hideHealthBars(){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			Destroy (s.gameObject);
		}
		healthbars = false;
	}

	//hide single health bar
	public void removeHealthBar(int id){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			if (s.GetComponent<HealthBar> ().id == id) {
				Destroy (s.gameObject);
			}
		}
	}

	//show all percentages of hits from selected troop to other players' troops
	public void showAllChances(){
		List<Trooper> others = Game._instance.notMyTroopers (myPlayer);
		foreach (Trooper t in others) {
			GameObject chance = Instantiate (ChanceObject, GameObject.Find ("Canvas").transform);
			chance.GetComponent<Chance> ().target = myPlayer.getSelected ();
			chance.GetComponent<Chance> ().id = t.id;
		}
	}

	//remove percentages
	public void removeChances(){
		Chance[] all = GameObject.FindObjectsOfType<Chance> ();
		foreach (Chance c in all) {
			Destroy (c.gameObject);
		}
	}

	public void HitOrMiss(Vector3 pos, int hit){
		Vector2 UIpos = Camera.main.WorldToScreenPoint (pos);
		GameObject Hit = Instantiate (HitOrMissObject, UIpos, Quaternion.identity) as GameObject;
		Hit.transform.SetParent (canvas.transform, true);
		Hit.GetComponent<HitMiss> ().hitmis = hit;

	}
}