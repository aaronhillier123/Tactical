using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		InvokeRepeating ("ChangeText", 1f, 1f);
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeText(){
		Text curText = gameObject.GetComponent<Text> ();
		if (curText.text == "WAITING FOR OTHER PLAYERS...") {
			curText.text = "WAITING FOR OTHER PLAYERS";
		} else {
			curText.text += ".";
		}
	}
}
