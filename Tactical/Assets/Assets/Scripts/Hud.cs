using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

	public Game game;

	//trooper statistics
	public GameObject HealthObject;
	public GameObject ChanceObject;
	public static GameObject ChanceObjectS;
	public static GameObject HealthObjectS;
	private static bool healthbars = false;

	//GUI prefabs
	public static bool retracted = false;
	public GameObject buyPanel;

	//Store Images and Items
	public Sprite grenadeImage;
	public Sprite sniperImage;
	public Sprite marathon;
	public Sprite invulnerableImage;
	public static string currentItem;

	// Use this for initialization
	void Start () {
		HealthObjectS = HealthObject;
		PhotonNetwork.OnEventCall += changeTurn;
		GameObject GO = GameObject.FindGameObjectWithTag ("GameController");
		game = GO.GetComponent<Game> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	//set player to attackmode
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

	//Send change turn function to network
	public void ClickChangeTurn(){
		RaiseEventOptions ops = RaiseEventOptions.Default;
		ops.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent (3, null, true, ops);
	}

	//change turn function
	public void changeTurn(byte id, object content, int senderID){
		if (id == 3) {
			List<Trooper> troops = Game.allTroopers;
			foreach (Trooper t in troops) {
				t.unFreeze ();
				t.unselect ();
			}
			int idd = Game.playersTurn;
			if (idd == PhotonNetwork.room.PlayerCount) {
				Game.playersTurn = 1;
			} else {
				++Game.playersTurn;
			}
			Player myPlayer = Game.getPlayer (Game.playersTurn);
			myPlayer.onStartTurn ();
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

	//show single health bar
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

	//hide single health bar
	public static void removeHealthBar(int id){
		Slider[] healthbarsList = GameObject.FindObjectsOfType<Slider> ();
		foreach (Slider s in healthbarsList) {
			if (s.GetComponent<HealthBar> ().id == id) {
				Destroy (s.gameObject);
			}
		}
	}

	//send store panel to off screen or on screen
	public void retractStore()
	{
		GameObject sp = GameObject.Find ("StorePanel");
		if (retracted == false) {
			sp.transform.Find ("Retract").GetChild (0).GetComponent<Text> ().text = ">";
			sp.transform.Translate (-1 * Screen.width/5, 0f, 0f);
			retracted = true;
		} else {
			sp.transform.Find ("Retract").GetChild (0).GetComponent<Text> ().text = "<";
			sp.transform.Translate (Screen.width/5, 0f, 0f);
			retracted = false;
		}
	}

	//grenade information
	public void buyGrenade(){
		currentItem = "Grenade";
		string item = "Grenade";
		string description = "Throw an explosive device to the point clicked to injure all enemies around the detonation. Cost is 2 Dog Tags";
		//Sprite myImage = grenadeImage;
		showBuyPanel (item, description, grenadeImage);
		}

	public void buySniper(){
		currentItem = "Sniper";
		string item = "Sniper";
		string description = "Hit targets with twice the accuracy and increase your attack range by 100%.";
		//Sprite myImage = sniperImage;
		showBuyPanel(item, description, sniperImage);
	}

	public void buyInvulnerable(){
		currentItem = "Invulnerability";
		string item = "Invulnerability";
		string description = "Makes the selected troop invulnerable to all damage until your next turn";
		showBuyPanel (item, description, invulnerableImage);
	}

	//show grenade information
	public void showBuyPanel(string item, string description, Sprite image){
		float menuwidth = GameObject.Find ("Store").GetComponent<RectTransform> ().rect.width;
		Vector2 pos = new Vector2 ((Screen.width / 2), Screen.width / 3);
		GameObject buyPanelObject = Instantiate(buyPanel, pos, Quaternion.identity, GameObject.Find("Canvas").gameObject.transform);
		buyPanelObject.transform.Find ("SureItem").GetComponent<Text> ().text = item;
		buyPanelObject.transform.Find ("Description").GetComponent<Text> ().text = description;
		buyPanelObject.transform.Find ("Image").GetComponent<Image> ().sprite = image;
	}

	//do not purchase item and exit store
	public void cancelPurchase(){
		GameObject pan = GameObject.Find ("SurePanel(Clone)");
		Destroy (pan);
	}

	public void turnOffActionButtons(){
		GameObject[] itemButtons = GameObject.FindGameObjectsWithTag ("ItemButton");
		foreach (GameObject g in itemButtons) {
			g.GetComponent<Button> ().interactable = false;
		}
	}
	//purchase a specific item
	public void buy(){
		if (currentItem == "Grenade") {
			Player myPlayer = Game.getPlayer (PhotonNetwork.player.ID);
			myPlayer.Selected.hasGrenade = true;
			cancelPurchase ();
		} else if (currentItem == "Sniper") {
			Player myPlayer = Game.getPlayer (PhotonNetwork.player.ID);
			myPlayer.Selected.isSniper = true;
			cancelPurchase ();
		} else if (currentItem == "Invulnerability") {
			Player myp = Game.getPlayer (PhotonNetwork.player.ID);
			int troopid = myp.Selected.id;
			object con = (object)troopid;
			PhotonNetwork.RaiseEvent (7, con, true, EventHandler.ops);
			cancelPurchase ();
		}
	}

}
