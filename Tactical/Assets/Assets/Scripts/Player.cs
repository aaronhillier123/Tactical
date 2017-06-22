using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public int id;
	public GameObject TrooperObject;
	private List<Trooper> roster = new List<Trooper>();
	private Trooper Selected;
	// Use this for initialization
	void Start () {
		id = 1;
		Vector3 MyLocation = new Vector3 (100f, 0f, 60f);
		CreateTroopAt (MyLocation);
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Input.GetMouseButtonDown (0)) {
			if (Physics.Raycast (ray, out hit, 100)) {
				if (hit.collider.CompareTag ("Player")) {
					selectTrooper (hit.collider.gameObject.GetComponent<Trooper> ());
				} else {
					if (Selected != null) {
						Debug.Log ("select not null");
						Selected.StopAllCoroutines ();
						Selected.StartCoroutine (Selected.moveToPosition (hit.point, 15f));
					}
				}
			}
		}
	}

	void CreateTroopAt(Vector3 location){
		GameObject FirstTroopObject = Instantiate (TrooperObject, transform, true) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		firstTroop.id = id;
		roster.Add (firstTroop);
	}

	void selectTrooper(Trooper a){
		a.select();
		Selected = a;
	}
}
