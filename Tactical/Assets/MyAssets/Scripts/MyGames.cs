using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyGames : MonoBehaviour {

	public static MyGames _instance;
	public List<string> myGames = new List<string>();
	public List<string> myInvites = new List<string> ();
	public GameObject showGames;
	public GameObject showInvites;
	public GameObject content;
	public NetGame currentNet;
	public GameObject currentGamePanel;

	public GameObject SelectGameButton;
	// Use this for initialization
	void Start () {
		_instance = this;
		myGames = MenuScript._instance.currentGames;
		myInvites = MenuScript._instance.currentInvites;
		showGames.GetComponent<Button> ().onClick.AddListener (delegate {
			SetGames (myGames);
		});

		showInvites.GetComponent<Button> ().onClick.AddListener (delegate {
			SetInvites (myInvites);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetGames(List<string> games){
		myGames = games;
		for(int i=0; i<myGames.Count; ++i) {
			GameObject ButtonObject = Instantiate (SelectGameButton);
			ButtonObject.transform.SetParent (content.transform, false);
			ButtonObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (0, 120 - (30 * i), 0);
			Button button = ButtonObject.GetComponent<Button> ();
			ButtonObject.transform.Find ("Text").GetComponent<Text> ().text = "GAME " + i.ToString();
			string myString = myGames [i];
			button.onClick.AddListener( delegate {GetGameFromMenu(myString, true);});
		}
	}

	public void SetInvites(List<string> invites){
		myInvites = invites;
		for(int i=0; i<myInvites.Count; ++i) {
			GameObject ButtonObject = Instantiate (SelectGameButton);
			ButtonObject.transform.SetParent (content.transform, false);
			ButtonObject.GetComponent<RectTransform>().anchoredPosition = new Vector3 (0, 120 - (30 * i), 0);
			Button button = ButtonObject.GetComponent<Button> ();
			ButtonObject.transform.Find ("Text").GetComponent<Text> ().text = "INVITE " + i.ToString();
			string myString = myInvites [i];
			button.onClick.AddListener( delegate {GetGameFromMenu(myString, false);});		}
	}

	public void showGameDetails(){
		JoinGamePanel jgp = currentGamePanel.GetComponent<JoinGamePanel> ();
		if (jgp != null) {
			jgp.id.text = currentNet.id;
			jgp.MemberOne.text = (currentNet.players.Count > 0) ? currentNet.players [0] : "";
			jgp.MemberTwo.text = (currentNet.players.Count > 1) ? currentNet.players [1] : "";
			jgp.MemberThree.text = (currentNet.players.Count > 2) ? currentNet.players [2] : "";
			jgp.MemberFour.text = (currentNet.players.Count > 3) ? currentNet.players [3] : "";
			jgp.inGame = currentNet.inGame;
			jgp.updateButton ();
		}
	}

	public void GetGameFromMenu(string roomId, bool inThisGame){
		Debug.Log ("GETTING DATA");
		MenuScript._instance.initNetGame(roomId, inThisGame);
	}

}
