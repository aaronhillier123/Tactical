using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour {

	public int id;
	public int team;
	public GameObject myFlag;
	public Material[] mats = new Material[5];
	// Use this for initialization
	void Start () {

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
			HudController._instance.updateDogTags (myTroop.myPlayer.getDogTags());
			myTroop.flagPull ();
			StartCoroutine (changeFlag(myTroop));
			team = myTroop.team;
		} else {
		}
	}

	public void setTeam(int currentTeam, int initial){
		if (currentTeam != 0) {
			if (initial == 0) {
				Player myPlayer = GameHandler._instance.getPlayer (currentTeam);
				team = currentTeam;
				myFlag.GetComponent<MeshRenderer> ().material = mats [team];
				if (!myPlayer.myControlPoints.Contains (this)) {
					myPlayer.addControlPoint (this);
				}
			} else {
				team = currentTeam;
				myFlag.GetComponent<MeshRenderer> ().material = mats [team];
			}
		}
	}

	public IEnumerator changeFlag(Trooper troop){
		troop.DidSomething ();
		Vector3 og = myFlag.transform.position;
		while(myFlag.transform.position.y > Game._instance.floor){
				myFlag.transform.Translate (0f, -.2f, 0f);
				yield return null;
		}
		myFlag.GetComponent<MeshRenderer> ().material = mats[team];
		while (myFlag.transform.position.y <= og.y) {
			myFlag.transform.Translate (0f, .2f, 0f);
			yield return null;
		}
		myFlag.transform.position = og;
		Debug.Log ("making stop control point");
		troop.stop ();
		if (troop.myPlayer.myControlPoints.Count > (Game._instance.allControlPoints ().Count / 2)) {
			Debug.Log ("MyControlPoints: " + troop.myPlayer.myControlPoints.Count + " , Goal: " + Game._instance.allControlPoints ().Count);
			PhotonNetwork.RaiseEvent ((byte)14, (object)troop.myPlayer.team, true, new RaiseEventOptions () {
				ForwardToWebhook = true,
				CachingOption = EventCaching.AddToRoomCache,
				Receivers = ReceiverGroup.All,
			});
		}
	}

	void OnTriggerEnter(Collider coll){
		Trooper myTroop = coll.gameObject.GetComponent<Trooper> ();
		if (myTroop != null) {
			setTeam (myTroop.id);
		}
	}
}
