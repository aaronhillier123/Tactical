using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marathon : Ability{

	void start(){
		InvokeRepeating ("showIn", 1f, 1f);
	}

	private bool deleted = false;
	public void showIn(){
		Debug.Log ("distance is " + Vector3.Distance (myTroop.currentPosition, myTroop.getInitPosition ()) + " and max is " + myTroop.maxMoveDistance);
	}
	public override void inspect(){
		if (Vector3.Distance (myTroop.currentPosition, myTroop.getInitPosition ()) > myTroop.maxMoveDistance && deleted==false) {
			deleted = true;
			terminate ();
		}
	}
	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			myTroop.setMoveDistance (myTroop.getMoveDistance() + 25);
			myTroop.RemoveWalkLimit ();
			myTroop.ShowWalkLimit ();
			myTroop.clearActiveAbility ();
		} else if (phase == 1) {
			//myTroop.removeAbility (id);
		}
		++phase;
	}

	public override void passiveExecute (RaycastHit hit){
	}

	public override void removeControl (){
		phase = 0;
		hasControl = false;
	}
		

	public override void sell(){
		MessageScript._instance.setText (name+ " Sold!");
		myTroop.myPlayer.addDogTags (price);
		myTroop.setMoveDistance(myTroop.getMoveDistance() - 25);
		myTroop.removeAbility (id);
		myTroop.clearActiveAbility ();
		phase = 0;
		hasControl = false;
	}

}
