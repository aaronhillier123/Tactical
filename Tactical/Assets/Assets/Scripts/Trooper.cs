using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trooper : MonoBehaviour {

	public Player myPlayer;
	public float health = 100;

	public Animator animator;
	private int animInt;
	public int id;
	public int team;
	public Material BlueTroopSelected;
	public Material BlueTroop;
	public Material RedTroop;
	public Material OrangeTroop;
	public Material GreenTroop;
	public bool moving;
	public bool frozen = false;
	// Use this for initialization
	void Start () {
		
		myPlayer = GetComponentInParent<Player>();
		animator = gameObject.GetComponentInChildren<Animator> ();
		assignColor ();
		PhotonNetwork.OnEventCall += move;
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
				//stop ();
			}
		}
	}

	public void stop(){
		moving = false;
		animator.SetInteger ("AnimPar", 0);
	}

	public void move(byte id, object content, int senderID){
		if (id == 2) {
			float[] conList = (float[])content;
			int selectedID = (int)conList[0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game.GetTroop (selectedID);
			if (myTroop.moving == false) {
				myTroop.StopAllCoroutines();
				StartCoroutine (moveToPosition (myTroop.gameObject, newPos, 10f)); 
			}
		}
	}

	public IEnumerator moveToPosition(GameObject t, Vector3 destination, float speed)
	{
		t.GetComponent<Trooper>().moving = true;
		Vector3 direction = (destination - t.transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		t.transform.rotation = lookRotation;
		t.GetComponent<Trooper>().animator.SetInteger ("AnimPar", 1);
		Vector3 currentPos = t.transform.position;
		while(Vector3.Distance(t.transform.position, destination) > 1f)
		{
			t.transform.position = Vector3.MoveTowards (t.transform.position, destination, speed * Time.deltaTime);
			yield return null;
		}
		t.transform.position = destination;
		t.GetComponent<Trooper> ().stop ();
	}

	//public IEnumerator rotateAndShoot(GameObject t, Vector3 destination){
	//	Vector3 direction = (destination - t.transform.positionfd

	public void shoot(){
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
			mats [0] = BlueTroopSelected;
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;

	}

	public void rotateTo(Vector3 point){
		Vector3 direction = (point - gameObject.transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		gameObject.transform.rotation = lookRotation;
	}

	public void unselect(){
		if (myPlayer.Selected = this) {
			myPlayer.Selected = null;
		}
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		mats[0] = BlueTroop;
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

	public void freeze(){
		frozen = true;
	}

	void assignColor(){
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		switch (team) {
		case 1:
			mats [0] = BlueTroop;
			break;
		case 2:
			mats [0] = RedTroop;
			break;
		case 3:
			mats [0] = GreenTroop;
			break;
		case 4:
			mats [0] = OrangeTroop;
			break;
		default:
			break;
		}
		//Debug.Log ("assigning color");
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

}
