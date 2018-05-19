using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marathon : Ability{



	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			myTroop.setMaxDistance (myTroop.getMaxDistance () + 25);
			myTroop.RemoveWalkLimit ();
			myTroop.ShowWalkLimit ();
			myTroop.clearActiveAbility ();
		} else if (phase == 1) {
			//myTroop.removeAbility (id);
		}
		++phase;
	}

	public override void removeControl (){
		phase = 0;
		hasControl = false;
	}
		

	public override void sell(){
		MessageScript._instance.setText (name+ " Sold!");
		myTroop.myPlayer.addDogTags (price);
		myTroop.setMaxDistance(myTroop.getMaxDistance() - 25);
		myTroop.removeAbility (id);
		myTroop.clearActiveAbility ();
		phase = 0;
		hasControl = false;
	}

}
