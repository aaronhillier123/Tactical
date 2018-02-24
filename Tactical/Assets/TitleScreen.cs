using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TitleScreen : MonoBehaviour {

	public Text quote;
	public GameObject MenuPanel;
	public List<string> quotes = new List<string>(13){
		"\"I do not know with what weapons World War III will be fought, but I do know that World War IV will be fought with rocks.\"",
		"\"Goddamn it! You'll never get a Purple Heart hiding in a foxhole. Follow me!\"",
		"\"Casualties many; percentage of dead not known; combat efficiency: we are winning.\"",
		"\"Pain is temporary. It may last a minute, or an hour, or a day, or a year, but eventually it will subside and something else will take its place. If I quit, however, it lasts forever.\"",
		"\"One man with courage makes a majority.\"",
		"\"If you kill enough of them, they stop fighting. \"",
		"\"Don’t fire unless fired upon, but if they mean to have a war, let it begin here \"",
		"\"It’s not the size of the dog in the fight, it’s the size of the fight in the dog. \"",
		"\"Incoming fire has the right of way\"",
		"\"Never share a foxhole with anyone braver than yourself\"",
		"\"Never draw fire. It irritates everyone around you.\"",
		"\"Try to look unimportant! They may be low on ammo.\"",
		"\"They are in front of us, behind us, and we are flanked on both sides by an enemy that outnumbers us 29:1. They can't get away from us now!\""
	};

	// Use this for initialization
	void Start () {
		if (MenuScript._instance != null) {
			if (MenuScript._instance.running == true) {
				foreach (Button b in GameObject.FindObjectsOfType<Button>()) {
					b.interactable = true;
				}
			}
		}

		int rand = Random.Range (0, 11);
		quote.text = quotes [rand];

		if (GameObject.FindObjectsOfType<MenuScript> ().Length < 1) {
			Instantiate (MenuPanel);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
