using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : Ability {


	public override void removeControl ()
	{
		phase = 0;
		hasControl = false;
	}

	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			GameObject healObject = Instantiate (models [0], myTroop.currentPosition, Quaternion.identity, myTroop.gameObject.transform);
			myTroop.health = myTroop.getMaxHealth ();
			terminate ();
		} 
	}

	public override void sell ()
	{
		MessageScript._instance.setText (name+ " Sold!");
		myTroop.myPlayer.addDogTags (price);
		myTroop.removeAbility (id);
		myTroop.clearActiveAbility ();
		phase = 0;
		hasControl = false;
	}


}
