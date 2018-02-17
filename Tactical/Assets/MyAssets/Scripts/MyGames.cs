﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGames : MonoBehaviour {

	public static MyGames _instance;
	public List<string> myGames = new List<string>();
	public List<string> myInvites = new List<string> ();

	public MenuScript menu;
	public GameObject SelectGameButton;
	// Use this for initialization
	void Start () {
		_instance = this;
		MenuScript.CloudGetMyGames ();
		MenuScript.CloudGetMyInvites ();
		menu = GameObject.FindObjectOfType<MenuScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetGames(List<string> games){
		myGames = games;
		for(int i=0; i<myGames.Count; ++i) {
			GameObject ButtonObject = Instantiate (SelectGameButton, new Vector2(-100, (200 - (30 *i))), Quaternion.identity);
			ButtonObject.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			Button button = ButtonObject.GetComponent<Button> ();
			ButtonObject.transform.Find ("Text").GetComponent<Text> ().text = myGames [i];
			string myString = myGames [i];
			button.onClick.AddListener( delegate {menu.rejoinGame(myString);});
		}
	}

	public void SetInvites(List<string> invites){
		myInvites = invites;
		for(int i=0; i<myInvites.Count; ++i) {
			GameObject ButtonObject = Instantiate (SelectGameButton, new Vector2(100, (200 - (30 *i))), Quaternion.identity);
			ButtonObject.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			Button button = ButtonObject.GetComponent<Button> ();
			ButtonObject.transform.Find ("Text").GetComponent<Text> ().text = myInvites [i];
			string myString = myInvites [i];
			button.onClick.AddListener( delegate {menu.JoinGame(myString);});
		}
	}

}
