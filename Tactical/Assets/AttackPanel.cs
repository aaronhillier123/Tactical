using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackPanel : MonoBehaviour {

	public Button left;
	public Button right;
	public Button attack;
	public Button cancel;
	public List<Trooper> inRange;
	private int troopIndex = 0;
	public Text troopInfo;
	public Trooper myTroop;
	public Ability parentAbility;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void nextTroop(){
		if (inRange.Count == 0) {
			MessageScript._instance.setText ("There are no enemy troops in range!");
			cancelAttack ();
		} else {
			if (troopIndex == (inRange.Count - 1)) {
				troopIndex = 0;
			} else {
				troopIndex++;
			}
			myTroop.target = inRange [troopIndex];
			troopInfo.text = "Possible Targets: " + inRange.Count + "\n";
			troopInfo.text += getTroopString (myTroop.target);
			CameraPan._instance.moveToObject (inRange [troopIndex].gameObject, false);
		}
	}

	public void previousTroop(){
		if (troopIndex != 0) {
			troopIndex--;
		} else {
			troopIndex = inRange.Count - 1;
		}
		myTroop.target = inRange [troopIndex];
		troopInfo.text = "Possible Targets: " + inRange.Count + "\n";
		troopInfo.text += getTroopString (myTroop.target);
		CameraPan._instance.moveToObject (inRange [troopIndex].gameObject, false);
	}

	public string getTroopString(Trooper t){
		string troopString = "";
		troopString += "Troop " + troopIndex + " of " + inRange.Count + "\n";
		troopString += "Team: " + t.team + "\n";
		troopString += "Health: " + t.getHealth () + "/" + t.getMaxHealth () + "\n";
		troopString += "Distance: " + Vector3.Distance (myTroop.transform.position, t.transform.position) + "\n";
		troopString += "Chance of hit: " + Game._instance.getChanceOfHit (myTroop, t) + "\n";
		troopString += "Team Threat " + getMapControl (t) + "%\n";
		return troopString;
	}

	public float getMapControl(Trooper t){
		int activeControlPoints = 0;
		foreach (ControlPoint p in Game._instance.allControlPoints()) {
			if (p.team != 0) {
				activeControlPoints++;
			}
		}
		float controlPointFloat = (float)t.myPlayer.myControlPoints.Count / activeControlPoints;
		float troopFloat = (float)t.myPlayer.roster.Count / (float)Game._instance.allTroopers.Count;
		return (((controlPointFloat + troopFloat) / 2f)*100f);
	}

	public void cancelAttack(){
		parentAbility.removeControl();
		myTroop.removeAbility (0);
		CameraPan._instance.moveToObject (myTroop.gameObject, false);
	}

	public void initializeButtons(){
		right.onClick.AddListener (delegate {
			previousTroop();
		});
		left.onClick.AddListener (delegate {
			nextTroop();
		});
		attack.onClick.AddListener (delegate {
			myTroop.executeAbility(myTroop.target.transform.position);
		});
		cancel.onClick.AddListener (delegate {
			cancelAttack();
		});
	}
}
