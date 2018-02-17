using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

	public GameObject button;
	public GameObject image;
	public Sprite Win;
	public Sprite Lose;
	public GameObject status;
	public GameObject message;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void show(int victory){
		if (victory == 1) {
			image.GetComponent<Image> ().sprite = Win;
			status.GetComponent<Text> ().text = "Victory";
			message.GetComponent<Text> ().text = "Congratulations! Play more games to increase your ranking and earn more tactical stars!";
		}else {
			image.GetComponent<Image> ().sprite = Lose;
			status.GetComponent<Text> ().text = "Defeat";
			message.GetComponent<Text> ().text = "Sorry! Play again to win and earn some tactical stars!";
		}
	}
		
}
