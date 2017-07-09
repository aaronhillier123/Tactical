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
	public float maxDistance = 50f;
	//Seperate Objects
	public GameObject bullet;
	public GameObject grenade;
	public GameObject explosion;
	public GameObject shield;
	public GameObject dogTag;
	public GameObject moveLimit;

	//for movement
	public Vector3 initialPosition;

	//identification
	public int id;
	public int team;
	public Player myPlayer;

	//Blue Troop Materials
	public Material BlueTroopSelected;
	public Material BlueTroop;
	public Material BlueTroopFrozen;
	public Material BlueTroopShield;
	public Material BlueTroopLimit;

	//Red Troop Materials
	public Material RedTroop;
	public Material RedTroopSelected;
	public Material RedTroopFrozen;
	public Material RedTroopShield;
	public Material RedTroopLimit;

	//Orange Troop Materials
	public Material OrangeTroop;
	public Material OrangeTroopFrozen;
	public Material OrangeTroopSelected;
	public Material OrangeTroopShield;
	public Material OrangeTroopLimit;

	//Green Troop Materials
	public Material GreenTroop;
	public Material GreenTroopFrozen;
	public Material GreenTroopSelected;
	public Material GreenTroopShield;
	public Material GreenTroopLimit;

	//state of availability
	public bool moving = false;
	public bool frozen = false;

	//bools for abilities
	public bool hasGrenade = false;
	public bool isSniper = false;
	public bool isInvulnerable = false;
	public bool canMarathon = false;


	void Start () {
		myPlayer = GetComponentInParent<Player>();
		animator = gameObject.GetComponentInChildren<Animator> ();
		assignColor ();
	}


	// Update is called once per frame
	void Update () {
	}

	public void showMovementLimit(){
	}

	public void stop(){
		moving = false;
		animator.SetInteger ("AnimPar", 0);
	}

	public void flagPull(){
		animator.SetInteger ("AnimPar", 8);
	}

	public static void makeInvulnerable(byte id, object content, int senderID){
		if (id == 7) {
			Trooper myTroop = Game.GetTroop ((int)content);
			myTroop.isInvulnerable = true;
			GameObject shieldO = Instantiate (myTroop.shield, myTroop.gameObject.transform);
			Material[] mats = shieldO.GetComponent<MeshRenderer> ().materials;
			switch (myTroop.team) {
			case 1:
				mats [0] = myTroop.BlueTroopShield;
				break;
			case 2:
				mats [0] = myTroop.RedTroopShield;
				break;
			case 3:
				mats [0] = myTroop.GreenTroopShield;
				break;
			case 4:
				mats [0] = myTroop.OrangeTroopShield;
				break;
			default:
				break;
			}
			shieldO.GetComponent<MeshRenderer> ().materials = mats;
		}
	}

	public void makeNotInvulnerable(){
		isInvulnerable = false;
		GameObject shieldO = gameObject.transform.Find("Shield(Clone)").gameObject;
		if (shieldO != null) {
			Destroy (shieldO);
		}
	}
		
	public static void move(byte id, object content, int senderID){
		if (id == 2) {
			float[] conList = (float[])content;
			int selectedID = (int)conList[0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			int cpID = (int)conList [4];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game.GetTroop (selectedID);
			if (myTroop.moving == false) {
				if (cpID != -1) {
					Vector3 newPoss = new Vector3 (newPos.x, 0, newPos.z);
					myTroop.StopAllCoroutines ();
					myTroop.StartCoroutine (myTroop.moveAndCapture (myTroop.gameObject, newPoss, 10f, cpID));
				} else {
					myTroop.StopAllCoroutines ();
					myTroop.StartCoroutine (myTroop.moveToPosition (myTroop.gameObject, newPos, 10f)); 
				}
			}
		}
	}

	public IEnumerator moveAndCapture(GameObject t, Vector3 destination, float speed, int CPID){
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
		t.GetComponent<Trooper> ().unselect ();
		t.GetComponent<Trooper> ().resetDistance ();
		ControlPoint cp = Game.getConrolPoint (CPID);
		if (cp != null) {
			cp.setTeam (team, id);
		}
		Debug.Log ("Captured!");
		//t.GetComponent<Trooper> ().stop ();
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

	public void resetDistance(){
		float newDis = maxDistance - (Vector3.Distance(initialPosition, transform.position));
		initialPosition = transform.position;
		maxDistance = newDis;
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
		decreaseHealth (50f);
	}

	public IEnumerator die(){
		unselect ();
		Vector3 pos = gameObject.transform.position;
		Vector3 newPos = new Vector3 (pos.x, pos.y + 3f, pos.z);
		animator.SetInteger ("AnimPar", 6);
		yield return new WaitForSeconds (2f);
		myPlayer.roster.Remove (this);
		Game.allTroopers.Remove (this);
		GameObject newDogTag = Instantiate (dogTag, newPos, Quaternion.identity);
		Game.allDogTags.Add(newDogTag.GetComponent<DogTag>());
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
		hurtNearByEnemies (myGrenade.transform.position, 20f, 20f);
		GameObject ex = GameObject.Instantiate (explosion, myGrenade.transform.position, Quaternion.identity);
		Destroy (myGrenade);
		yield return new WaitForSeconds (1f);
		Destroy (ex);
		stop ();
	}

	public void hurtNearByEnemies(Vector3 point, float distance, float damage){
		Player myPlayer = Game.getPlayer (team);
		List<Trooper> others = Game.notMyTroopers (myPlayer);
		foreach(Trooper t in others){
			if(Vector3.Distance(point, t.gameObject.transform.position) < distance){
				Hud.showHealthBar (t.id);
				t.rotateTo (point);
				t.naded ();
				t.decreaseHealth (damage);
			}
		}
	}
		
	public void naded(){
		animator.SetInteger ("AnimPar", 7);
		Invoke ("stop", 1f);
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

	public void decreaseHealth(float dec){
		if (isInvulnerable == false) {
			health -= dec;
			if (health <= 0) {
				StartCoroutine (die ());
			} else {
				Invoke ("stop", 1f);
			}
		}
	}

	public float distanceTo(Vector3 target){
		return (Vector3.Distance(gameObject.transform.position, target));
	}

	public void select(){
		if (myPlayer.Selected != null) {
			myPlayer.Selected.unselect ();
		}
		if (this.frozen == true) {
			resetDistance ();
		}
			myPlayer.Selected = this;
			GameObject limiter = Instantiate (moveLimit, initialPosition, Quaternion.identity);
			limiter.transform.localScale = new Vector3 (2 * maxDistance, 1, 2 * maxDistance);
			Material[] lim = new Material[1];
			Material[] mats = new Material[1];
			switch (team) {
			case 1:
				mats [0] = BlueTroopSelected;
				lim [0] = BlueTroopLimit;
				break;
			case 2:
				mats [0] = RedTroopSelected;
				lim [0] = RedTroopLimit;
				break;
			case 3:
				mats [0] = GreenTroopSelected;
				lim [0] = GreenTroopLimit;
				break;
			case 4:
				mats [0] = OrangeTroopSelected;
				lim [0] = OrangeTroopLimit;
				break;
			default:
				break;
			}
		if (this.frozen == false) {
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
			noAttackMode ();
		}
			limiter.GetComponent<MeshRenderer> ().materials = lim;
			GameObject[] itemButtons = GameObject.FindGameObjectsWithTag ("ItemButton");
			foreach (GameObject g in itemButtons) {
				g.GetComponent<Button> ().interactable = true;
			}

		}

	public void rotateTo(Vector3 point){
		Vector3 direction = (point - gameObject.transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		gameObject.transform.rotation = lookRotation;
	}

	public void goAttackMode(){
		if (frozen == false) {
			GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = false;
			GameObject.Find ("AttackText").GetComponent<Text> ().text = "CHOSE PLAYER";
			GameObject.Find ("AttackText").GetComponent<Text> ().fontSize = 10;
			myPlayer.showAllChances ();
			myPlayer.attacking = true;
		}
	}

	public void noAttackMode(){
		if (frozen == false) {
			GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = true;
		} else {
			GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = false;
		}
		GameObject.Find ("AttackText").GetComponent<Text> ().text = "ATTACK";
		GameObject.Find ("AttackText").GetComponent<Text> ().fontSize = 14;
		myPlayer.removeChances ();
		myPlayer.attacking = false;
	}

	public void goBack(){
		Vector3 direction = (initialPosition - gameObject.transform.position).normalized;
		gameObject.transform.Translate (direction * 2f);
	}


	public void unselect(){
		if (myPlayer.Selected = this) {
			myPlayer.Selected = null;
		}

		GameObject limiter = GameObject.Find ("MoveLimit(Clone)");
		if (limiter != null) {
			Destroy (limiter);
		}

		GameObject[] itemButtons = GameObject.FindGameObjectsWithTag ("ItemButton");
		foreach (GameObject g in itemButtons) {
			g.GetComponent<Button> ().interactable = false;
		}
		noAttackMode ();
		GameObject.Find ("AttackButton").GetComponent<Button> ().interactable = false;

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
		if (this.frozen == false) {
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().materials = mats;
		}
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

	public void unFreeze(){
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
		frozen = false;
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
