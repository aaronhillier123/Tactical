using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trooper : MonoBehaviour {

	public GameObject hitmisOb;
	public Player myPlayer;
	public float health = 100;
	public GameObject bullet;
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

	public void shoot(GameObject target){
		StartCoroutine (shootThis (target));
	}

	public void miss(GameObject target){
		StartCoroutine (missThis (target));
	}

	IEnumerator shootThis(GameObject target){
		yield return new WaitForSeconds (1f);
		Vector3 p = gameObject.transform.position;
		Vector3 startpos = new Vector3 (p.x, p.y + 5, p.z);
		GameObject mybullet = Instantiate (bullet, startpos, Quaternion.identity);
		Vector3 t = target.transform.position;
		Vector3 finalPos = new Vector3 (t.x, t.y + 5, t.z);
		Vector2 location = Camera.main.WorldToScreenPoint (new Vector3(p.x, p.y+7, p.z));
		Vector2 alocation = new Vector2 (location.x + 50, location.y);
		GameObject misser = Instantiate (hitmisOb, alocation, Quaternion.identity, GameObject.Find("Canvas").transform);
		misser.GetComponent<HitMiss> ().hitmis = "Hit";
		while (Vector3.Distance (mybullet.transform.position, finalPos) > 1f) {
			mybullet.transform.position = Vector3.MoveTowards (mybullet.transform.position, finalPos, 60 * Time.deltaTime);
			yield return null;
		}
		target.GetComponent<Trooper> ().rotateTo (transform.position);
		target.GetComponent<Trooper>().gotShot ();
		Destroy (mybullet);
	}

	public void gotShot(){
		animator.SetInteger ("AnimPar", 5);
		health -= 50;
		if (health > 0) {
			Invoke ("stop", 1f);
		} else {
			animator.SetInteger ("AnimPar", 6);
			//Hud.hideHealthBars ();
			Invoke ("die", 2f);
		}
	}

	public void die(){
		
		myPlayer.roster.Remove (this);
		Game.allTroopers.Remove (this);
		Hud.removeHealthBar (id);
		Destroy (gameObject);
	}


	IEnumerator missThis(GameObject target){
		yield return new WaitForSeconds (1f);
		Vector3 p = gameObject.transform.position;
		float xoff = Random.Range (-4, 4);
		float yoff = Random.Range (-4, 4);
		float zoff = Random.Range (-4, 4);
		Vector3 startpos = new Vector3 (p.x, p.y + 5, p.z);
		GameObject mybullet = Instantiate (bullet, startpos, Quaternion.identity);
		Vector3 t = target.transform.position;
		Vector3 finalPos = new Vector3 (t.x+xoff, t.y+yoff + 5, t.z+zoff);
		Vector2 location = Camera.main.WorldToScreenPoint (new Vector3(p.x, p.y+7, p.z));
		Vector2 alocation = new Vector2 (location.x + 50, location.y);
		GameObject misser = Instantiate (hitmisOb, alocation, Quaternion.identity, GameObject.Find("Canvas").transform);
		misser.GetComponent<HitMiss> ().hitmis = "Missed";
		while (Vector3.Distance (mybullet.transform.position, finalPos) > 1f) {
			mybullet.transform.position = Vector3.MoveTowards (mybullet.transform.position, finalPos, 60 * Time.deltaTime);
			yield return null;
		}

		Destroy (mybullet);
	}

	void throwGrenade(){
		animator.SetInteger ("AnimPar", 3);
	}

	void stab(){
		animator.SetInteger ("AnimPar", 4);
	}

	public float distanceTo(Vector3 target){
		return (Vector3.Distance(gameObject.transform.position, target));
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
