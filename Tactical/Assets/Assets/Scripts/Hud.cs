using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

	public Game game;
	public GameObject HealthObject;
	public GameObject ChanceObject;
	public static GameObject ChanceObjectS;
	public static GameObject HealthObjectS;
	private static bool healthbars = false;
	// Use this for initialization
	void Start () {
		HealthObjectS = HealthObject;
		PhotonNetwork.OnEventCall += changeTurn;
		GameObject GO = GameObject.FindGameObjectWithTag ("GameController");
		if (GO != null) {
			Debug.Log ("Found Game object");
		}
		game = GO.GetComponent<Game> ();
		if (game != null) {
			Debug.Log ("found game script1");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	public void attackMode(){
		Player myPlayer = Game.getPlayer (PhotonNetwork.player.ID);
		if (myPlayer.attacking == true) {
			myPlayer.removeChances ();
			myPlayer.attacking = false;
		} else {
			myPlayer.showAllChances ();
			myPlayer.attacking = true;
		}
	}

	public void ClickChangeTurn(){
		RaiseEventOptions ops = RaiseEventOptions.Default;
		ops.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent (3, null, true, ops);
	}

	public void changeTurn(byte id, object content, int senderID){
		if (id == 3) {
			Debug.Log ("Changing turns");
			int idd = Game.playersTurn;
			if (idd == PhotonNetwork.room.PlayerCount) {
				Game.playersTurn = 1;
			} else {
				++Game.playersTurn;
			}
		}
	}

	//show all health bars
	public void showHealthBars(){
		if (healthbars == false) {
			hideHealthBars ();
			foreach (Trooper t in Game.allTroopers) {
				GameObject myHealth = Instantiate (HealthObjectS) as GameObject;
				myHealth.GetComponent<HealthBar> ().id = t.id;
			}
			healthbars = true;
		} else {
			hideHealthBars ();
		}
	}

	//single health bar
	public static void showHealthBar(int id){
		hideHealthBars ();
		GameObject myHealth = Instantiate (HealthObjectS) as GameObject;
		myHealth.GetComponent<HealthBar>().id = id;
	}

	//hide all health bars
	public static void hideHealthBars(){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			Destroy (s.gameObject);
		}
		healthbars = false;
	}

	public static void removeHealthBar(int id){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			if (s.GetComponent<HealthBar> ().id == id) {
				Destroy (s.gameObject);
			}
		}
	}
		


}
