using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	public int id;
	public int team;
	public Material BlueFlag;
	public Material RedFlag;
	public Material GreenFlag;
	public Material OrangeFlag;
	public Material NeutralFlag;

	// Use this for initialization
	void Start () {
		team = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setTeam(int a, int b){
		Trooper myTroop = Game.GetTroop (b);
		if (team != a) {
			myTroop.myPlayer.addControlPoint (this);
			myTroop.myPlayer.dogtags += 2;
			Hud.updateDogTags (myTroop.myPlayer.dogtags);
			myTroop.flagPull ();
			StartCoroutine (changeFlag(a, b));
			team = a;
		} else {
			myTroop.stop ();
		}
	}

	public IEnumerator changeFlag(int team, int troopid){
		
		GameObject myFlag = gameObject.transform.Find ("Flag").gameObject;
		Debug.Log ("CHANGING FLAG");
		Vector3 og = myFlag.transform.position;
		while(myFlag.transform.position.y > 0){
				myFlag.transform.Translate (0f, -.2f, 0f);
				yield return null;
		}
		switch (team) {
		case 1:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.blue;
			break;
		case 2:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.red;
			break;
		case 3:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.green;
			break;
		case 4:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.yellow;
			break;
		default:
			myFlag.GetComponent<MeshRenderer> ().materials [0].color = Color.white;
			break;
		}
		while (myFlag.transform.position.y <= og.y) {
			myFlag.transform.Translate (0f, .2f, 0f);
			yield return null;
		}
		myFlag.transform.position = og;
		Trooper myTroop = Game.GetTroop (troopid);
		myTroop.stop ();
	}
}
