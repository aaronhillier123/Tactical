using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trooper : MonoBehaviour {

	//miscellaneous
	//public GameObject hitmisOb;

	public GameObject limit;

	//animation
	private Animator animator;
	//0 - idle
	//1 - run
	//2 - shoot
	//3 - throw grenade
	//4 - stab
	//5 - got shot
	//6 - Die
	//7 got naded
	//8 flagpool
	//10 - jump barrier
	//11 - take cover


	private int animInt;
	private float maxDistance = 50f;
	private float health = 100;
	//Seperate Objects




	//for movement
	public Vector3 initialPosition;
	public float range = 100;

	//identification
	public int id;
	public int team;
	public Player myPlayer;




	//state of availability
	public bool moving = false;
	public bool frozen = false;
	public bool covering = false;

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

	public void stop(){
		moving = false;
		animator.SetInteger ("AnimPar", 0);
	}

	public void reset(){
		maxDistance = 50f;
		initialPosition = transform.position;
		hasGrenade = false;
		isSniper = false;
		isInvulnerable = false;
		canMarathon = false;
		PhotonNetwork.RaiseEvent (9, (object)id, true, GameHandler._instance.AllReceivers());
		unFreeze ();
	}

	public float getHealth(){
		return health;
	}

	public void flagPull(){
		animator.SetInteger ("AnimPar", 8);
	}

	public void takeCover(Vector3 pos){
	}

	public static void makeInvulnerable(byte id, object content, int senderID){
		if (id == 7) {
			Trooper myTroop = Game._instance.GetTroop ((int)content);
			myTroop.isInvulnerable = true;
			GameObject myShield = Instantiate (TroopController._instance.TroopObjects[3], myTroop.gameObject.transform);
			myShield.GetComponent<MeshRenderer> ().material = TroopController._instance.ShieldMats[myTroop.team];
		}
	}

	public static void makeNotInvulnerable(byte id, object content, int senderID){
		if (id == 9) {
			Trooper myTroop = Game._instance.GetTroop ((int)content);
			GameObject TroopOb = myTroop.gameObject;
			myTroop.isInvulnerable = false;
			GameObject myShield = TroopOb.transform.Find ("Shield(Clone)").gameObject;
			if (myShield != null) {
				Destroy (myShield);
			}
		}
	}
		
	public static void move(byte id, object content, int senderID){
		if (id == 2) {
			Debug.Log ("MOOOVVVIIINGNGGGG");
			float[] conList = (float[])content;
			int selectedID = (int)conList[0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game._instance.GetTroop (selectedID);
			myTroop.StopAllCoroutines ();
			myTroop.StartCoroutine (myTroop.moveToPosition (newPos, 10f)); 
		}
	}
		
	public void RaiseMovement(Vector3 point){
		HudController._instance.AttackMode (false);
		Vector3 floor = toFloor (point);
		float[] contents = new float[5];
		contents [0] = (float)myPlayer.Selected.id;
		if (Vector3.Distance (point, myPlayer.Selected.initialPosition) <= myPlayer.Selected.maxDistance) {
			//if the click point is within the troops walking distance
			contents [1] = point.x;
			contents [2] = point.y;
			contents [3] = point.z;
		} else {
			//find farthest point that troop can currently travel
			Vector3 myPoint = ((point - initialPosition).normalized) * maxDistance;
			contents [1] = initialPosition.x + myPoint.x;
			contents [2] = initialPosition.y + myPoint.y;
			contents [3] = initialPosition.z + myPoint.z;
		}
		RaiseEventOptions rf = RaiseEventOptions.Default;
		rf.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent ((byte)2, (object)contents, true, rf);
		HudController._instance.CanAttack (false);	
	}

	public Vector3 toFloor(Vector3 point){
		return new Vector3 (point.x, Game._instance.floor, point.z);
	}

	public IEnumerator moveToPosition(Vector3 destination, float speed)
	{
		moving = true;
		Vector3 direction = (destination - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation (direction);
		animator.SetInteger ("AnimPar", 1);
		while(Vector3.Distance(transform.position, destination) > 1f)
		{
			transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
			yield return null;
		}
		transform.position = destination;
		stop ();
		select ();
	}

	public void resetDistance(){
		float newDis = maxDistance - (Vector3.Distance(initialPosition, transform.position));
		initialPosition = transform.position;
		maxDistance = newDis;
		}
		
	public void shoot(Trooper enemy, int hit){
		StopAllCoroutines ();
		animator.SetInteger ("AnimPar", 2);
		StartCoroutine (ShootCoroutine (enemy, hit));
		Invoke ("stop", 1f);
	}

	IEnumerator ShootCoroutine(Trooper enemy, int hit){
		yield return new WaitForSeconds (1f);
		Vector3 startpos = transform.position + new Vector3 (0, 5, 0);
		Vector3 enemyPos = enemy.transform.position + new Vector3 (0, 5, 0);
		GameObject mybullet = Instantiate (TroopController._instance.TroopObjects[0], startpos, Quaternion.identity);
		while (Vector3.Distance (mybullet.transform.position, enemyPos) > 1f) {
			mybullet.transform.position = Vector3.MoveTowards (mybullet.transform.position, enemyPos, 60 * Time.deltaTime);
			yield return null;
		}
		switch (hit) {
		case 0:
			
				break;
		case 1:
			enemy.gotShot ();

				break;
			default:
				break;
		}
		HudController._instance.HitOrMiss (mybullet.transform.position, hit);
		Destroy (mybullet);
		enemy.Invoke ("stop", 1f);
	}
		
	public void gotShot(){
		StopAllCoroutines ();
		animator.SetInteger ("AnimPar", 5);
		decreaseHealth (50f);
	}

	public IEnumerator die(){
		unselect ();
		animator.SetInteger ("AnimPar", 6);
		yield return new WaitForSeconds (2f);
		myPlayer.roster.Remove (this);
		Game._instance.allTroopers.Remove (this);
		GameObject newDogTag = Instantiate (TroopController._instance.TroopObjects[4], transform.position + new Vector3 (0, 3f, 0), Quaternion.identity);
		Game._instance.allDogTags.Add(newDogTag.GetComponent<DogTag>());
		HudController._instance.removeHealthBar (id);
		Destroy (gameObject);
	}
		
	public void giveGrenade(){
		hasGrenade = true;
	}

	public void giveSniper(){
		isSniper = true;
		range += 100;
	}

	public void giveInvulnerability(){
		isInvulnerable = true;
		RaiseEventOptions rf = RaiseEventOptions.Default;
		rf.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent (7, (object)id, true, rf);
	}

	public void giveMarathon(){
		canMarathon = true;
		maxDistance += 50;
		resetDistance ();
		select ();
	}

	public void giveAbility(int ability){
		switch(ability){
		case 0:
			giveGrenade();
			break;
		case 1:
			giveSniper();
			break;
		case 2:
			giveInvulnerability();
			break;
		case 3:
			giveMarathon();
			break;
		default:
			break;
		}
	}

	public void throwGrenade(Vector3 target){
		StopAllCoroutines ();
		animator.SetInteger ("AnimPar", 3);
		StartCoroutine (throwCoroutine (target));
		Invoke ("stop", 1f);
	}

	public IEnumerator throwCoroutine(Vector3 position){
		hasGrenade = false;
		rotateTo (position);
		yield return new WaitForSeconds (1f);
		Vector3 groundPosition = toFloor (position);
		//animator.SetInteger ("AnimPar", 3);
		Vector3 p = gameObject.transform.position + new Vector3(0f, 3f ,0f);
		GameObject myGrenade = Instantiate (TroopController._instance.TroopObjects[1], p, Quaternion.identity); 
		Vector3 mid = (midPoint (p, groundPosition)) + new Vector3(0f, 5f, 0f);
		Vector3 fq = (midPoint (p, mid)) + new Vector3 (0f, 3f, 0f);
		Vector3 lq = (midPoint (mid, groundPosition)) + new Vector3(0f, 3f, 0f);
		while (Vector3.Distance (myGrenade.transform.position, fq) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, fq, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, mid) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, mid, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, lq) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, lq, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (myGrenade.transform.position, position) > 1f) {
			myGrenade.transform.position = Vector3.MoveTowards (myGrenade.transform.position, position, 30 * Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds (.5f);
		hurtNearByEnemies (myGrenade.transform.position, 20f, 20f);
		GameObject ex = GameObject.Instantiate (TroopController._instance.TroopObjects[2], myGrenade.transform.position, Quaternion.identity);
		Destroy (myGrenade);
		yield return new WaitForSeconds (1f);
		Destroy (ex);
		stop ();
	}

	public void hurtNearByEnemies(Vector3 point, float distance, float damage){
		Player myPlayer = GameHandler._instance.getPlayer (team);
		List<Trooper> others = Game._instance.notMyTroopers (myPlayer);
		foreach(Trooper t in others){
			if(Vector3.Distance(point, t.gameObject.transform.position) < distance){
				HudController._instance.showHealthBar (t.id);
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
		return (start + finish) / 2;
	}

	public void decreaseHealth(float dec){
			health -= dec;
			if (health <= 0) {
				StartCoroutine (die ());
			} else {
				Invoke ("stop", 1f);
			}
	}
		
	public void select(){
		if (myPlayer.Selected != null) {
			myPlayer.Selected.unselect ();
		}
		myPlayer.Selected = this;
		if (this.frozen == false) {
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.SelectedMats[team];
			HudController._instance.CanAttack (true);
		}
		ShowWalkLimit ();
		HudController._instance.RefreshStore ();
		}

	public void rotateTo(Vector3 point){
		Vector3 direction = (point - gameObject.transform.position).normalized;
		gameObject.transform.rotation = Quaternion.LookRotation (direction);
	}
		
	public void ShowWalkLimit(){
		GameObject limiter = Instantiate (TroopController._instance.TroopObjects[5], initialPosition, Quaternion.identity, transform);
		limit = limiter;
		limit.GetComponent<Projector> ().orthographicSize = maxDistance * 2;
		limit.transform.SetParent (null);
		limit.transform.rotation = Quaternion.Euler (90f, 0, 0);
	}

	public void RemoveWalkLimit(){
		if(limit!=null){
			Destroy (limit.gameObject);
		}
	}

	public void goBack(){
		Vector3 direction = (initialPosition - gameObject.transform.position).normalized;
		gameObject.transform.Translate (direction * 2f);
	}
		
	public void unselect(){
		if (myPlayer.Selected = this) {
			myPlayer.Selected = null;
		}

		RemoveWalkLimit ();
		HudController._instance.AttackMode (false);	
		HudController._instance.CanAttack (false);
		if (this.frozen == false) {
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.TroopMats[team];
		}
	}

	public void freeze(){
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.FrozenMats[team];
		frozen = true;
	}

	public void unFreeze(){
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.TroopMats[team];
		frozen = false;
	}

	void assignColor(){
		transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.TroopMats[team];
	}

	public void DidSomething(){
		unselect ();
		resetDistance ();
	}
}
