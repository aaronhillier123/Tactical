using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour {

	public int team;
	public List<Material> materials = new List<Material> ();
	public List<Transform> spawnPoints = new List<Transform> ();
	public Quaternion FacingOut = new Quaternion ();
	// Use this for initialization
	void Start () {
		
		FacingOut = gameObject.transform.rotation * Quaternion.Euler (0f, 45f, 0f);

		switch (team) {
		case 1:
			GetComponent<MeshRenderer> ().material = materials [1];
			break;
		case 2:
			GetComponent<MeshRenderer> ().material = materials [2];
			break;
		case 3:
			GetComponent<MeshRenderer> ().material = materials [3];
			break;
		case 4:
			GetComponent<MeshRenderer> ().material = materials [4];
			break;
		default:
			GetComponent<MeshRenderer> ().material = materials [0];
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
