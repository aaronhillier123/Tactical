using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

	void OnCollisionEnter(Collision coll){
		explode (coll.contacts[0].point);
	}

	public void explode(Vector3 point){
		GameObject ex = Instantiate (TroopController._instance.TroopObjects [2], point, Quaternion.identity);
		ParticleSystem pc = ex.GetComponent<ParticleSystem> ();
		Vector3 newV = new Vector3 (5f, 5f, 5f);
		ParticleSystem.ShapeModule psm = pc.shape;
		psm.scale = newV;
		Explosion myEx = ex.GetComponent<Explosion> ();
		if (myEx != null) {
			myEx.type = 2;
		}
		Destroy (gameObject);
	}
}
