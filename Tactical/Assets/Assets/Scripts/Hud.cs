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

	public void showInfoPanel(int item){
		Store.removeInfoPanel ();
		Store.ShowInfoPanel(item);
	}

	public void removeInfoPanel(){
		Store.removeInfoPanel ();
	}

	public void StartTurn(){
		AttackMode (false);
		GamePlay.NextTurn.interactable = true;
	}

	public void EndTurn(){
		AttackMode (false);
		GamePlay.NextTurn.interactable = false;
	}

	public void RefreshStore(){
		Store.refresh ();
	}

	public void CanAttack(bool attack){
		if (attack == true) {
			GamePlay.Attack.interactable = true;
		} else {
			GamePlay.Attack.interactable = false;
			GamePlay.Attack.image.color = new Color(255f / 255f, 163f/255f, 36f/255f, 255f/255f);
		}

	}

	public void AttackMode(bool attack){
		if (attack == true) {
			HudController._instance.showAllChances ();
			GamePlay.Attack.image.color = Color.green;
			GamePlay.Attack.interactable = false;
		} else {
			HudController._instance.removeChances ();
			GamePlay.Attack.image.color = new Color(255f / 255f, 163f/255f, 36f/255f, 255f/255f);
		}
	}

	public void Retract(){
		Store.Retract ();
	}

	public static void updateDogTags(int amount){
		if (HudController._instance.myPlayer.isTurn()) {
			GameObject.Find ("DogTagsText").GetComponent<Text> ().text = "x " + amount.ToString ();
		}
	}

	public void nextTroopPan(){
		Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
		myPlayer.lookingAt++;
		if (myPlayer.lookingAt >= myPlayer.roster.Count) {
			myPlayer.lookingAt = 0;
		}
		CameraController._instance.setFollowedObject(myPlayer.roster [myPlayer.lookingAt].gameObject, 1);
	}



}
