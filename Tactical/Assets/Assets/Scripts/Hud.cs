using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

	public Game game;
	public GameObject HealthObject;
	private bool healthbars = false;
	// Use this for initialization
	void Start () {
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
			myPlayer.attacking = false;
		} else {
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

	public void showHealthBars(){
		if (healthbars == false) {
			hideHealthBars ();
			foreach (Trooper t in Game.allTroopers) {
				GameObject myHealth = Instantiate (HealthObject) as GameObject;
				myHealth.GetComponent<HealthBar> ().id = t.id;
			}
			healthbars = true;
		} else {
			hideHealthBars ();
		}
	}

	public void hideHealthBars(){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			Destroy (s.gameObject);
		}
		healthbars = false;
	}


}
