using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour {

	public int team;
	public List<Transform> spawnPoints = new List<Transform> ();
	public Quaternion FacingOut = new Quaternion ();
	public ControlPoint myPoint;
	public Material[] mats = new Material[5];
	// Use this for initialization
	void Start () {
		
		FacingOut = gameObject.transform.rotation * Quaternion.Euler (0f, 45f, 0f);
		GetComponent<MeshRenderer> ().material = mats [team];
		myPoint.setTeam (team, 1);
		myPoint.id = team;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
