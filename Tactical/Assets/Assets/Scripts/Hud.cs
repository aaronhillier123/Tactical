using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {



	//Store All Hud Buttons
	public GameObject StorePanelObject;
	public GameObject GamePlayPanelObject;
	public GameObject ChatPanel;


	public GamePlayPanel GamePlay;
	public StorePanel Store;

	void Start () {
		GamePlay = GamePlayPanelObject.GetComponent<GamePlayPanel> ();
		Store = StorePanelObject.GetComponent<StorePanel> ();
	}



	void Update () {
		
	}

	public void StartTurn(){
		AttackMode (false);
		GamePlay.NextTurn.interactable = true;
	}

	public void EndTurn(){
		AttackMode (true);
		GamePlay.NextTurn.interactable = false;
	}

	public void RefreshStore(){
		Store.refresh ();
	}

	public void AttackMode(bool attack){
		if (attack == true) {
			GamePlay.Attack.interactable = true;
		} else {
			GamePlay.Attack.interactable = false;
		}
	}

	public static void updateDogTags(int amount){
		if (HudController._instance.myPlayer.isTurn()) {
			GameObject.Find ("DogTagsText").GetComponent<Text> ().text = "x " + amount.ToString ();
		}
	}

	public void nextTroopPan(){
		Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
		myPlayer.lookingAt++;
		if (myPlayer.lookingAt == myPlayer.roster.Count) {
			myPlayer.lookingAt = 0;
		}
		GameObject.Find ("CameraPan").GetComponent<CameraPan> ().moveToPlayer (myPlayer.roster [myPlayer.lookingAt]);
	}

}
