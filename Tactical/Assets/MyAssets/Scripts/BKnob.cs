using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKnob : MonoBehaviour {

	public bool selected = false;
	public GameObject myBarrier;
	public float distance;
	// Use this for initialization
	void Start () {
		distance = Vector3.Distance (gameObject.transform.position, gameObject.transform.parent.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (selected == true) {


			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 200)) {
				RotateKnob (hit.point);

			}
		}
	}

	public void RotateKnob(Vector3 point){

		Vector3 dir = (point - myBarrier.transform.position).normalized;
		Vector3 p = myBarrier.transform.position + (distance * dir);
		gameObject.transform.position = new Vector3 (p.x, myBarrier.transform.position.y, p.z);
		Quaternion mine = gameObject.transform.rotation;
		Quaternion other = Quaternion.LookRotation (dir, Vector3.up);
		Vector3 newEuler = new Vector3 (0, other.eulerAngles.y, 90);
		Vector3 parentEuler = new Vector3 (0, other.eulerAngles.y, 0);
		gameObject.transform.eulerAngles = newEuler;
		gameObject.transform.parent.eulerAngles = parentEuler;

	}

	void onMouseDown(){
		selected = true;
	}

	void onMouseUp(){
		selected = false;
	}
}
