using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarineGrenade : Ability{

	//models
	//0 Grenade
	//1 Grenade Limit

	private GameObject currentLimit;

	void awake(){
		name = "Grenade";
		description = "Throw an explosive device that detonates and injures nearby troops!";
	}
		
	public override void execute (Vector3 target)
	{
		if (hasControl) {
			if (phase == 0) {
				if (PhotonNetwork.player.ID == myTroop.team) {
					currentLimit = Instantiate (models [1], transform, false);
					currentLimit.GetComponent<Projector> ().orthographicSize = myTroop.throwMax*2;
					currentLimit.transform.rotation = Quaternion.Euler (90f, 0, 0);
				}
			} else if (phase == 1) {
				myTroop.StopAllCoroutines ();
				myTroop.animator.SetTrigger ("Throw");
				HudController._instance.RefreshStore ();
				bool inRange = (Vector3.Distance (target, myTroop.currentPosition) <= myTroop.throwMax);
				target = (inRange) ? target : myTroop.currentPosition + (((target - myTroop.currentPosition).normalized) *  myTroop.throwMax);
				Debug.Log (target);
				StartCoroutine (throwCoroutine (myTroop, target));
			}
		}
		phase++;
	}

	public override void removeControl (){
		Destroy (currentLimit);
		phase = 0;
		hasControl = false;
	}

	public override void giveControl(){
		myTroop.activeAbility = gameObject;
		hasControl = true;
	}

	public IEnumerator throwCoroutine(Trooper t, Vector3 position){
		t.rotateTo (position);
		CameraPan._instance.moveTo (t.transform.position, 0.5f);
		yield return new WaitForSeconds (0.5f);
		Vector3 p = t.transform.position + new Vector3(0f, 3f ,0f);
		GameObject myGrenade = Instantiate (models[0], p, Quaternion.identity, myTroop.transform);
		Grenade g = myGrenade.GetComponent<Grenade> ();
		if (g != null) {
			g.destination = position;
			g.team = t.team;
		}
		CameraController._instance.setFollowedObject (myGrenade, 0);
		t.stop ();
		if (t.covering == true) {
			t.animator.SetInteger ("AnimPar", 11);
		}
		myTroop.removeAbility (id);
	}
		
}
