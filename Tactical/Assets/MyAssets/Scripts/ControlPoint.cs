using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	public int id;
	public int team;
	public Material[] FlagMaterials = new Material[5];
	public GameObject myFlag;
	// Use this for initialization
	void Start () {
		team = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setTeam(int troopId){
		Trooper myTroop = Game._instance.GetTroop (troopId);
		if (team != myTroop.team) {
			if (team != 0) {
				GameHandler._instance.getPlayer (team).removeControlPoint (this);
			}
			myTroop.StopAllCoroutines ();
			myTroop.myPlayer.addControlPoint (this);
			myTroop.myPlayer.addDogTags (2);
			Hud.updateDogTags (myTroop.myPlayer.getDogTags());
			myTroop.flagPull ();
			StartCoroutine (changeFlag(myTroop));
			team = myTroop.team;
		} else {
		}
	}

	public IEnumerator changeFlag(Trooper troop){
		
		Vector3 og = myFlag.transform.position;
		while(myFlag.transform.position.y > Game._instance.floor){
				myFlag.transform.Translate (0f, -.2f, 0f);
				yield return null;
		}
		myFlag.GetComponent<MeshRenderer> ().material = FlagMaterials[team];
		while (myFlag.transform.position.y <= og.y) {
			myFlag.transform.Translate (0f, .2f, 0f);
			yield return null;
		}
		myFlag.transform.position = og;
		troop.stop ();
	}

	void OnTriggerEnter(Collider coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			setTeam (myTroop.id);
		}
	}
}
