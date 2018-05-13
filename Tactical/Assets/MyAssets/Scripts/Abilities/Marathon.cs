using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marathon : Ability{



	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			myTroop.setMaxDistance (myTroop.getMaxDistance () + 25);
			myTroop.resetDistance ();
			myTroop.select ();
		} else if (phase == 1) {
			myTroop.removeAbility (id);
		}
		++phase;
	}

	public override void removeControl (){
		myTroop.setMaxDistance (myTroop.getMaxDistance () - 25);
		phase = 0;
		hasControl = false;
	}

	public override void giveControl(){
		myTroop.activeAbility = gameObject;
		hasControl = true;
	}
}
