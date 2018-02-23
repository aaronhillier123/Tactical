using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinGamePanel : MonoBehaviour {


	public Text id;
	public Text MemberOne;
	public Text MemberTwo;
	public Text MemberThree;
	public Text MemberFour;
	public Button RejoinButton;
	public bool inGame;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void updateButton(){
		RejoinButton.onClick.RemoveAllListeners ();
		if (inGame) {
			RejoinButton.onClick.RemoveAllListeners ();
			RejoinButton.onClick.AddListener (delegate {
				MenuScript._instance.rejoinGame (id.text);
			});
		} else {
			RejoinButton.onClick.AddListener (delegate {
				MenuScript._instance.JoinGame (id.text);
			});
		}
		RejoinButton.interactable = true;
	}
}
