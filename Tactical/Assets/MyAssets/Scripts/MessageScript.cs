using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageScript : MonoBehaviour {

	public static MessageScript _instance;
	public Text text;
	public string previousText = "";
	// Use this for initialization
	void Start () {
		_instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setPreviousText(){
		text.text = previousText;
	}

	public void setText(string message){
		previousText = text.text;
		text.text = message;
	}


}
