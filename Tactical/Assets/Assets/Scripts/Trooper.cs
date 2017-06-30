using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trooper : MonoBehaviour {

	//miscellaneous
	public GameObject hitmisOb;
	public float health = 100;

	//animation
	public Animator animator;
	private int animInt;

	//Seperate Objects
	public GameObject bullet;
	public GameObject grenade;


	//identification
	public int id;
	public int team;
	public Player myPlayer;

	//Blue Troop Materials
	public Material BlueTroopSelected;
	public Material BlueTroop;
	public Material BlueTroopFrozen;

	//Red Troop Materials
	public Material RedTroop;
	public Material RedTroopSelected;
	public Material RedTroopFrozen;

	//Orange Troop Materials
	public Material OrangeTroop;
	public Material OrangeTroopFrozen;
	public Material OrangeTroopSelected;

	//Green Troop Materials
	public Material GreenTroop;
	public Material GreenTroopFrozen;
	public Material GreenTroopSelected;

	//state of availability
	public bool moving;
	public bool frozen = false;
	// Use this for initialization

	//bools for abilities
	public bool hasGrenade = false;
	public bool isSniper = false;
	public bool isInvulnerable = false;
	public bool canMarathon = false;


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

	public void throwGrenade(Vector3 target){
		StartCoroutine (throwthis (target));
	}

	public IEnumerator throwthis(Vector3 positio){
		hasGrenade = false;
		rotateTo (positio);
		yield return new WaitForSeconds (1f);
		Vector3 position = new Vector3 (positio.x, 0, positio.z);
		//animator.SetInteger ("AnimPar", 3);
		Vector3 p = gameObject.transform.position;
		Vector3 pos = new Vector3 (p.x, p.y + 3, p.z);
		GameObject myGrenade = Instantiate (grenade, pos, Quaternion.identity); 
		Vector3 mid = midPoint (pos, position);
		Vector3 midup = new Vector3 (mid.x, mid.y + 5, mid.z);
		Vector3 fq = midPoint (pos, mid);
		Vector3 fqup = new Vector3 (fq.x, fq.y + 3, fq.z);
		Vector3 lq = midPoint (mid, position);
		Vector3 lqup = new Vector3 (lq.x, lq.y + 3, lq.z);
		while (Vector3.Distance (myGrenade.transform.position, fqup) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, fqup, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, midup) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, midup, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, lqup) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, lqup, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, position) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, position, 30 * Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds (.5f);
		Destroy (myGrenade);
		Invoke ("stop", .5f);
	}

	public void stab(){
		animator.SetInteger ("AnimPar", 4);
		Invoke ("stop", 1f);
	}

	public Vector3 midPoint(Vector3 start, Vector3 finish){
		Vector3 mp = new Vector3();
		mp.x = (start.x + finish.x) / 2;
		mp.y = (start.y + finish.y) / 2;
		mp.z = (start.z + finish.z) / 2;
		return mp;
	}

	public float distanceTo(Vector3 target){
		return (Vector3.Distance(gameObject.transform.position, target));
	}

	public void select(){
		if (myPlayer.Selected != null) {
			myPlayer.Selected.unselect ();
		}
		if (this.frozen == false) {
			myPlayer.Selected = this;
			Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
			switch(team){
			case 1:
				mats [0] = BlueTroopSelected;
				break;
			case 2:
				mats [0] = RedTroopSelected;
				break;
			case 3:
				mats [0] = GreenTroopSelected;
				break;
			case 4:
				mats [0] = OrangeTroopSelected;
				break;
			default:
				break;
			}
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
			GameObject[] itemButtons = GameObject.FindGameObjectsWithTag ("ItemButton");
			foreach (GameObject g in itemButtons) {
				g.GetComponent<Button> ().interactable = true;
			}
		}
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
		GameObject[] itemButtons = GameObject.FindGameObjectsWithTag ("ItemButton");
		foreach (GameObject g in itemButtons) {
			g.GetComponent<Button> ().interactable = false;
		}

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
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

	public void freeze(){
		Material[] mats = transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials;
		switch (team) {
		case 1:
			mats [0] = BlueTroopFrozen;
			break;
		case 2:
			mats [0] = RedTroopFrozen;
			break;
		case 3:
			mats [0] = GreenTroopFrozen;
			break;
		case 4:
			mats [0] = OrangeTroopFrozen;
			break;
		default:
			break;
		}
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
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
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
	}

}
