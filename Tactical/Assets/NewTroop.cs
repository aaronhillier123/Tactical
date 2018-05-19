using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTroop : Ability {


	public override void removeControl ()
	{
		phase = 0;
		hasControl = false;
	}

	public override void execute (Vector3 target)
	{
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		SpawnArea mySpawn = null;
		foreach (GameObject g in spawns) {
			SpawnArea s = g.GetComponent<SpawnArea> ();
			if (s != null) {
				if (s.team == myTroop.team) {
					mySpawn = s;
				}
			}
		}
		if (myTroop.myPlayer.roster.Count >= mySpawn.spawnPoints.Count) {
			MessageScript._instance.setText ("Maximum Troops allowed are on the map");
			sell ();
			return;
		}
		Vector3 newLocation = mySpawn.spawnPoints [myTroop.myPlayer.roster.Count].transform.position;
		int newTeam = myTroop.team;
		int newId = Game._instance.generateNewId ();
		myTroop.myPlayer.CreateTroopAt (newLocation, mySpawn.FacingOut, newTeam, newId);
		terminate ();
	}

	public override void sell ()
	{
		myTroop.myPlayer.addDogTags (price);
		myTroop.removeAbility (id);
		myTroop.clearActiveAbility ();
		phase = 0;
		hasControl = false;
	}

	void Update () {
		
	}
}
