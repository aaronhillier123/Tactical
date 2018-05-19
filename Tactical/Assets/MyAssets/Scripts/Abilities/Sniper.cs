using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Ability
{

	public bool shot = false;
	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			myTroop.setRange (myTroop.getRange () + 100);
			myTroop.clearActiveAbility ();
		} else if (phase == 1) {
			//myTroop.removeAbility (id);
		}
	}

	public override void passiveExecute (RaycastHit hit){
	}

	public override void removeControl (){
		phase = 0;
		hasControl = false;
	}
		
	public override void inspect(){
		if (myTroop.shooting && shot == false) {
			shot = true;
			terminate ();
		}
	}

	public override void sell(){
		MessageScript._instance.setText (name+ " Sold!");
		myTroop.myPlayer.addDogTags (price);
		myTroop.setRange (myTroop.getRange () - 100);
		myTroop.removeAbility (id);
		myTroop.clearActiveAbility ();
		phase = 0;
		hasControl = false;
	}
}
