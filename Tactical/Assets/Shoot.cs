using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Ability {

	private GameObject attackPanelObject;

	public override void giveControl ()
	{
		hasControl = true;
	}

	public override void removeControl ()
	{
		hasControl = false;
		Destroy (attackPanelObject);
		HudController._instance.showGameHud ();
		phase = 0;
	}

	public override void execute (Vector3 target)
	{
		if (phase == 0) {
			if (PhotonNetwork.player.ID == myTroop.myPlayer.team) {
				List<Trooper> shootable = generateShootList ();
				HudController._instance.removeGameHud ();
				attackPanelObject = Instantiate (models [0], GameObject.Find ("Canvas").transform);
				AttackPanel attackPanel = attackPanelObject.GetComponent<AttackPanel> ();
				attackPanel.myTroop = myTroop;
				attackPanel.inRange = shootable;
				attackPanel.parentAbility = this;
				attackPanel.initializeButtons ();
				attackPanel.nextTroop ();
			}
			++phase;
		} else if (phase == 1) {
			if (PhotonNetwork.player.ID == myTroop.myPlayer.team) {
				float randomHit = Random.Range (0, 100f);
				int hit = 0;
				if (randomHit <= Game._instance.getChanceOfHit (myTroop, myTroop.target)) {
					hit = 1;
				} else {
					hit = 2;
				}
				int[] contentArray = new int[3];
				contentArray[0] = myTroop.id;
				contentArray[1] = myTroop.target.id;
				contentArray[2] = hit;
				object content = (object)contentArray;
				PhotonNetwork.RaiseEvent ((byte)4, content, true, new RaiseEventOptions () {
					Receivers = ReceiverGroup.All,
					ForwardToWebhook = true
				});
			}
			myTroop.removeAbility (id);
		}
	}

	public static void takeTheShot(byte id, object content, int senderId){
		if (id == 4) {
			int[] contentArray = (int[])content;
			Trooper myTroop = Game._instance.GetTroop (contentArray [0]);
			Trooper target = Game._instance.GetTroop (contentArray [1]);
			int hit = contentArray [2];
			myTroop.shoot (target, hit);
		}
	}

	public List<Trooper> generateShootList(){
		List<Trooper> others = Game._instance.notMyTroopers (myTroop.myPlayer);
		List<Trooper> inRange = new List<Trooper> ();
		foreach (Trooper t in others) {
			if (Vector3.Distance (t.currentPosition, myTroop.currentPosition) <= myTroop.getRange ()) {
				inRange.Add (t);
			}
		}
		return inRange;
	}
}
