using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invulnerable : Ability {

	//models
	//0 Invulnerable block
	GameObject myShield;

	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			if (!myTroop.isInvulnerable) {
				myShield = Instantiate (models [0], myTroop.transform, true);
				myShield.transform.localPosition = new Vector3 (0, 1f, 0);
				myShield.transform.localScale = new Vector3 (2f, 3f, 2f);
				myShield.GetComponent<MeshRenderer> ().material = TroopController._instance.ShieldMats [myTroop.team];
				myTroop.makeInvulnerable ();
			}
		} else if (phase == 1) {

		}
		++phase;
	}
		
	public override void removeControl (){
		phase = 0;
		hasControl = false;
	}

	public override void giveControl(){
		myTroop.activeAbility = gameObject;
		hasControl = true;
	}
}
