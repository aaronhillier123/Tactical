using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

	public int id;
	public int type;
	public int team;

	public bool placed = false;

	public GameObject Cylinder;
	public GameObject Knob;
	public List<GameObject> pieces;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HidePrelimbs(){
		Debug.Log ("HIDING PRELIMBS");
		Cylinder.GetComponent<MeshRenderer> ().enabled = false;
		Knob.GetComponent<MeshRenderer> ().enabled = false;
		Knob.GetComponent<CapsuleCollider> ().enabled = false;
	}

	public void ShowPrelimbs(){
		Cylinder.GetComponent<MeshRenderer> ().enabled = false;
		Knob.GetComponent<MeshRenderer> ().enabled = false;
	}


}

