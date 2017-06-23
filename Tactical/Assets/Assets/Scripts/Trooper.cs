using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trooper : MonoBehaviour {

	public Player myPlayer;

	private Animator animator;
	private int animInt;
	public int id;
	public int team;
	public Material BlueTroopSelected;
	public Material BlueTroop;
	public Material RedTroop;
	public Material OrangeTroop;
	public Material GreenTroop;

	public bool moving;
	// Use this for initialization
	void Start () {
		myPlayer = GetComponentInParent<Player>();
		animator = gameObject.GetComponentInChildren<Animator> ();
		assignColor ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("g")) {
			throwGrenade ();
		} else if (Input.GetKey ("s")) {
			stab ();
		} else if (Input.GetKey ("space")) {
			shoot ();
		} else{
			if (moving != true) {
				stop ();
			}
		}
	}

	void rotateTo (float a){
		float rotationPerMinute;
		Quaternion myRotation = gameObject.transform.rotation;
		float moveSpeed = .01f;
		gameObject.transform.rotation = Quaternion.Lerp(myRotation, Quaternion.Euler(0, a, 0), moveSpeed * Time.time);
	}

	void stop(){
		moving = false;
		animator.SetInteger ("AnimPar", 0);
	}

	void move(){
		animator.SetInteger ("AnimPar", 1);
		gameObject.transform.Translate ( 0f, 0f, 0.25f);
	}

	public IEnumerator moveToPosition(Vector3 destination, float speed)
	{
		moving = true;
		Vector3 direction = (destination - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		while (Quaternion.Angle (transform.rotation, lookRotation) < 1) {
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * .001f);
		}
		transform.rotation = lookRotation;
		animator.SetInteger ("AnimPar", 1);
		Vector3 currentPos = transform.position;
		while(Vector3.Distance(transform.position, destination) > 1f)
		{
			transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
			yield return null;
		}
		transform.position = destination;
		stop ();
	}

	void shoot(){
	animator.SetInteger ("AnimPar", 2);
	}

	void throwGrenade(){
		animator.SetInteger ("AnimPar", 3);
	}

	void stab(){
		animator.SetInteger ("AnimPar", 4);
	}

	public void select(){
		if (myPlayer.Selected != null) {
			myPlayer.Selected.unselect ();
		}
		myPlayer.Selected = this;
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		mats[0] = BlueTroopSelected;
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

	public void unselect(){
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		mats[0] = BlueTroop;
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

	void assignColor(){
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		switch (team) {
		case 0:
			mats [0] = BlueTroop;
			break;
		case 1:
			mats [0] = RedTroop;
			break;
		case 2:
			mats [0] = GreenTroop;
			break;
		case 3:
			mats [0] = OrangeTroop;
			break;
		default:
			break;
		}
		//Debug.Log ("assigning color");
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

}
