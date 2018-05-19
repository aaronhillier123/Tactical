using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarineAirStrike : Ability {

	GameObject airControlPanel;
	GameObject missile;

	public override void removeControl ()
	{
		Debug.Log("remove control");
		Destroy (airControlPanel.gameObject);
		CameraController._instance.normalView ();
		HudController._instance.showGameHud ();
		phase = 0;
		hasControl = false;
	}
	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			if (PhotonNetwork.player.ID == myTroop.team) {
				CameraController._instance.aerialView ();
				HudController._instance.removeGameHud ();
				airControlPanel = Instantiate (models [0], GameObject.Find ("Canvas").transform);
				airControlPanel.GetComponent<AirStrikePanel> ().myAbility = this;
				airControlPanel.GetComponent<AirStrikePanel> ().setButtons ();
			}
			phase++;
		} else if (phase == 1) {
			Quaternion downward = Quaternion.Euler (90f, 0f, 0f);
			missile = Instantiate (models [1], target + new Vector3 (0, 80f, 0), downward, myTroop.transform);
			missile.GetComponent<Missile> ().myTroop = myTroop;
		}
	}

	public override void passiveExecute (RaycastHit hit){
	}

	public override void inspect(){
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
