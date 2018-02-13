using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Trooper : MonoBehaviour {

	//miscellaneous
	//public GameObject hitmisOb;
	[System.NonSerialized]
	private GameObject limit;

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
	private BarrierPiece myPiece;
	private Vector3 initialPosition;
	private bool updated = false;
	private float range = 100;

	//identification
	public int id;
	public int team;

	[System.NonSerialized]
	public Player myPlayer;

	public Vector3 currentPosition;
	public Vector3 destinationPosition;
	public Quaternion currentRotation;

	//state of availability
	private bool moving = false;

	public bool frozen = false;

	public bool covering = false;

	[System.NonSerialized]
	public bool takingCover = false;

	//bools for abilities
	[System.NonSerialized]
	public bool hasGrenade = false;
	[System.NonSerialized]
	public bool isSniper = false;
	[System.NonSerialized]
	public bool isInvulnerable = false;
	[System.NonSerialized]
	public bool canMarathon = false;


	void Start () {
		myPlayer = GetComponentInParent<Player>();
		animator = gameObject.GetComponentInChildren<Animator> ();
		assignColor (); 
	}

	//Getters Setters
	public void setPiece(BarrierPiece b){
		myPiece = b;
	}
	public BarrierPiece getPiece(){
		return myPiece;
	}
	public void setInPos(Vector3 ip){
		initialPosition = ip;
	}
	public Vector3 getInPos(){
		return initialPosition;
	}
	public void setRange(float r){
		range = r;
	}
	public float getRange(){
		return range;
	}
	public void setMaxDistance(float m){
		maxDistance = m;
	}
	public float getMaxDistance(){
		return maxDistance;
	}
	public void SetUpdated(bool ud){
		updated = ud;
	}
	public bool isUpdated(){
		return updated;
	}

	// Update is called once per frame
	void Update () {
		if (moving == false) {
			currentPosition = transform.position;
		} else {
			currentPosition = destinationPosition;
		}
		currentRotation = transform.rotation;
	}

	public void setAnimation(int anim){
		animator.SetInteger ("AnimPar", anim);
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
		canMarathon = false;
		if (isInvulnerable == true) {
			isInvulnerable = false;
			PhotonNetwork.RaiseEvent (8, (object)id, true, GameHandler._instance.AllReceivers ());
		}
		unFreeze ();
	}

	public float getHealth(){
		return health;
	}

	public void flagPull(){
		animator.SetInteger ("AnimPar", 8);
	}
		
	public void takeCover(){
		if (takingCover == true) {
			takingCover = false;
			this.StopAllCoroutines ();
			this.goBack (1f);
			this.stop ();
			/*
			if (myPiece != null) {
				Vector3 towardBarrier = (myPiece.transform.position - transform.position).normalized;
				Debug.Log ("X" + towardBarrier.x + " y: " + towardBarrier.y + " z: " + towardBarrier.z);
				transform.rotation = Quaternion.Euler (towardBarrier + new Vector3 (0f, 180f, 0f));
			}
			*/
			transform.Rotate(new Vector3(0, 180f, 0));
			animator.SetInteger ("AnimPar", 11);
		}
	}

	IEnumerator ContinueAnimation(float time, int startAnimation, int animation){
		yield return new WaitForSeconds (time);
		if (animator.GetInteger ("AnimPar") == startAnimation) {
			animator.SetInteger ("AnimPar", animation);
		}
	}

	public void jumpBarrier(){
		int currentAnimation = animator.GetInteger ("AnimPar");
		animator.SetInteger ("AnimPar", 10);
		StartCoroutine(ContinueAnimation(0.8f, 10, currentAnimation));
	}
		


	public static void RaiseInvulnerable(byte id, object content, int senderID){
		if (id == 7) {
			Trooper myTroop = Game._instance.GetTroop ((int)content);
			myTroop.MakeInvulnerable ();
		}
	}

	public static void RaiseNotInvulnerable(byte id, object content, int senderID){
		if (id == 8) {
			Trooper myTroop = Game._instance.GetTroop ((int)content);
			myTroop.makeNotInvulnerable ();
		}
	}

	public void MakeInvulnerable(){
		isInvulnerable = true;
		GameObject myShield = Instantiate (TroopController._instance.TroopObjects[3], gameObject.transform);
		myShield.GetComponent<MeshRenderer> ().material = TroopController._instance.ShieldMats[team];
	}

	public void makeNotInvulnerable(){
			isInvulnerable = false;
			GameObject myShield = transform.Find ("Shield(Clone)").gameObject;
			if (myShield != null) {
				Destroy (myShield);
			}
		}
		
	public static void move(byte id, object content, int senderID){
		if (id == 2) {
			float[] conList = (float[])content;
			int selectedID = (int)conList[0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			int cover = (int)conList [4];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game._instance.GetTroop (selectedID);
			if (cover == 1) {
				myTroop.covering = true;
			} else {
				myTroop.covering = false;
			}
			myTroop.StopAllCoroutines ();
			myTroop.StartCoroutine (myTroop.moveToPosition (newPos, 10f)); 
		}
	}
		
	public void RaiseMovement(Vector3 point, int cover){
		HudController._instance.AttackMode (false);
		Vector3 floor = toFloor (point);
		Debug.Log ("Floor level is " + floor.y);
		float[] contents = new float[6];
		contents [0] = (float)myPlayer.getSelected().id;
		if (Vector3.Distance (floor, myPlayer.getSelected().getInPos()) <= myPlayer.getSelected().getMaxDistance()) {
			//if the click point is within the troops walking distance
			contents [1] = floor.x;
			contents [2] = floor.y;
			contents [3] = floor.z;
		} else {
			//find farthest point that troop can currently travel
			Vector3 myfloor = ((floor - initialPosition).normalized) * maxDistance;
			contents [1] = initialPosition.x + myfloor.x;
			contents [2] = initialPosition.y + myfloor.y;
			contents [3] = initialPosition.z + myfloor.z;
		}
		contents [4] = (float)cover;
		PhotonNetwork.RaiseEvent ((byte)2, (object)contents, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true
		});
		HudController._instance.CanAttack (false);	
	}

	public Vector3 toFloor(Vector3 point){
		return new Vector3 (point.x, Game._instance.floor, point.z);
	}

	public IEnumerator moveToPosition(Vector3 destination, float speed)
	{
		moving = true;
		destinationPosition = destination;
		Vector3 direction = (destination - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation (direction);
		animator.SetInteger ("AnimPar", 1);
		myPiece = null;
		while(Vector3.Distance(transform.position, destination) > 1f)
		{
			transform.position = Vector3.MoveTowards (transform.position, destination, speed * Time.deltaTime);
			yield return null;
		}
		transform.position = destination;
		stop ();
		if (myPlayer.getSelected() == this && frozen == false) {
			HudController._instance.CanAttack (true);
		}
	}

	public IEnumerator MoveAfterSeconds(float time){
		yield return new WaitForSeconds(time);
		animator.SetInteger ("AnimPar", 1);
	}


	public void resetDistance(){
		float newDis = maxDistance - (Vector3.Distance(initialPosition, transform.position));
		initialPosition = transform.position;
		maxDistance = newDis;
		}
		
	public void shoot(Trooper enemy, int hit){
		StopAllCoroutines ();
		CameraController._instance.setFollowedObject (gameObject, 0);
		rotateTo (enemy.transform.position);
		animator.SetInteger ("AnimPar", 2);
		StartCoroutine (ShootCoroutine (enemy, hit));
		Invoke ("stop", 1f);
	}

	IEnumerator ShootCoroutine(Trooper enemy, int hit){
		yield return new WaitForSeconds (1f);
		Vector3 startpos = transform.position + new Vector3 (0, 5, 0);
		Vector3 enemyPos = enemy.transform.position + new Vector3 (0, 5, 0);
		GameObject mybullet = Instantiate (TroopController._instance.TroopObjects[0], startpos, Quaternion.identity);
		CameraController._instance.setFollowedObject (mybullet, 0);
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
		case 3:
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
		CameraController._instance.setFollowedObject (gameObject, 1);
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
		PhotonNetwork.RaiseEvent (7, (object)id, true, GameHandler._instance.AllReceivers());
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
		CameraController._instance.setFollowedObject (gameObject, 0);
		yield return new WaitForSeconds (1f);
		Vector3 groundPosition = toFloor (position);
		Vector3 p = gameObject.transform.position + new Vector3(0f, 3f ,0f);
		GameObject myGrenade = Instantiate (TroopController._instance.TroopObjects[1], p, Quaternion.identity); 
		CameraController._instance.setFollowedObject (myGrenade, 0);
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
		Debug.Log ("Stop2");
		stop ();
		if (covering == true) {
			animator.SetInteger ("AnimPar", 11);
		}
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
		HudController._instance.showHealthBar (id);
		health -= dec;
		if (health <= 0) {
			StartCoroutine (die ());
		} else {
			Invoke ("stop", 1f);
		}
	}
		
	public void select(){
		if (myPlayer.getSelected() != null) {
			myPlayer.getSelected().unselect ();
		}
		myPlayer.setSelected (this);
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
		if (team == PhotonNetwork.player.ID) {
			GameObject limiter = Instantiate (TroopController._instance.TroopObjects [5], initialPosition, Quaternion.identity, transform);
			limit = limiter;
			limit.GetComponent<Projector> ().orthographicSize = maxDistance * 2;
			limit.transform.SetParent (null);
			limit.transform.rotation = Quaternion.Euler (90f, 0, 0);
		}
	}

	public void RemoveWalkLimit(){
		if(limit!=null){
			Destroy (limit.gameObject);
		}
	}

	public void goBack(float distance){
		Vector3 direction = (initialPosition - gameObject.transform.position).normalized;
		gameObject.transform.Translate (direction * distance);
	}
		
	public void unselect(){
		if (myPlayer.getSelected() == this) {
			myPlayer.setSelected (null);
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
