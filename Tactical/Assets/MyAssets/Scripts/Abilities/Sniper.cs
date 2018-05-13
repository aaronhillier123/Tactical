using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Ability
{
	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			myTroop.setRange (myTroop.getRange () + 100);
		} else if (phase == 1) {
			myTroop.removeAbility (id);
		}
	}

	public override void removeControl (){
		myTroop.setRange(myTroop.getRange() - 100);
		phase = 0;
		hasControl = false;
	}

	public override void giveControl(){
		myTroop.activeAbility = gameObject;
		hasControl = true;
	}
}
