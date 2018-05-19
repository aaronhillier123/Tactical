using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	public Trooper myTroop;

	void OnCollisionEnter(Collision coll){
		explode (coll.contacts[0].point);
	}

	public void explode(Vector3 point){
		GameObject ex = Instantiate (TroopController._instance.TroopObjects [2], point, Quaternion.identity, myTroop.transform);
		ParticleSystem pc = ex.GetComponent<ParticleSystem> ();
		Vector3 newV = new Vector3 (5f, 5f, 5f);
		ParticleSystem.ShapeModule psm = pc.shape;
		psm.scale = newV;
		Explosion myEx = ex.GetComponent<Explosion> ();
		if (myEx != null) {
			myEx.type = 2;
		}
		myTroop.removeAbility(myTroop.activeAbility.GetComponent<Ability>().id);
		CameraPan._instance.moveToObject (ex, true);
		Destroy (gameObject);
	}
}
