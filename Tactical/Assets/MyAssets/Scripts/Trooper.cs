using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Trooper : MonoBehaviour {

	//miscellaneous
	//public GameObject hitmisOb;
	[System.NonSerialized]
	private GameObject limit;
	[System.NonSerialized]
	private GameObject gLimit;
	//animation
	public Animator animator;
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

	private GameObject partSys;
	private int animInt;
	private float maxDistance = 50f;
	private float health = 100;
	private BarrierPiece myPiece;
	private Vector3 initialPosition;
	private bool updated = false;
	private float range = 100;
	private float grenadeMax = 40f;
	//identification
	public int id;
	public int team;


	public Player myPlayer;

	public Vector3 currentPosition;
	public Vector3 destinationPosition;
	public Quaternion currentRotation;
	public List<int> abilities = new List<int> ();

	//state of availability
	private bool moving = false;
	public bool frozen = false;
	public bool covering = false;
	public bool takingCover = false;
	public bool inFoxHole = false;
	public bool hasGrenade = false;
	public bool isSniper = false;
	public bool isInvulnerable = false;
	public bool canMarathon = false;
	public bool hasDoubleShot = false;
	public bool hasAirStrike = false;
	public bool jumping = false;


	void awake () {
		myPlayer = GetComponentInParent<Player>();
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
		if (maxDistance < 2) {
			maxDistance = 2;
		}
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
	public List<int> GetAbilities(){
		return abilities;
	}
	public void AddAbility(int a){
		abilities.Add(a);
	}
	public bool isMoving(){
		return moving;
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
		if (animator != null) {
			animator.SetInteger ("AnimPar", anim);
		} else {
		}
	}

	public void stop(){
		moving = false;
		jumping = false;
		if (covering == false) {
			animator.SetInteger ("AnimPar", 0);
		} 
	}

	public void reset(){
		setMaxDistance(50f);
		initialPosition = transform.position;
		hasGrenade = false;
		isSniper = false;
		canMarathon = false;
		abilities.Clear ();
		if (isInvulnerable == true) {
			PhotonNetwork.RaiseEvent (8, (object)id, true, new RaiseEventOptions(){
				Receivers = ReceiverGroup.All, 
				ForwardToWebhook = true});
		} else {
		}
		unFreeze ();
	}

	public float getHealth(){
		return health;
	}

	public void flagPull(){
		animator.SetInteger ("AnimPar", 8);
	}
		
	public void takeCover(Vector3 dir){
		if (takingCover == true) {
			takingCover = false;
			this.StopAllCoroutines ();
			Vector3 point = transform.position + (dir*5);
			Vector3 newPos = transform.position + (dir*-0.5f);
			transform.position = newPos;
			rotateTo (point);
			stop ();
			transform.Rotate(new Vector3(0, 180f, 0));
			animator.SetInteger ("AnimPar", 11);
		}
	}

	IEnumerator ContinueAnimation(float time, int startAnimation, int animation){
		yield return new WaitForSeconds (time);
		if (animator.GetInteger ("AnimPar") == startAnimation) {
			Debug.Log ("THIS IS WHERE");
			animator.SetInteger ("AnimPar", animation);
		}
		jumping = false;
	}

	public void jumpBarrier(){
		if (jumping == false) {
			jumping = true;
			int currentAnimation = animator.GetInteger ("AnimPar");
			animator.SetTrigger ("Jump");
			transform.position += Vector3.up * 4;
			StartCoroutine (ContinueAnimation (0.8f, 10, currentAnimation));
		}
	}
		
	public void RaiseAttack(Trooper EnemyTroop){
		MessageScript._instance.setPreviousText ();
		myPlayer.setAttacking(false);
		if (hasDoubleShot == false) {
			freeze ();
		} else {
			hasDoubleShot = false;
			abilities.Remove (4);
			HudController._instance.RefreshStore ();
		}
		HudController._instance.removeChances ();
		float deduct = 0;

		//under cover may be blocked by barrier
		if (EnemyTroop.getPiece() != null) {
			Vector3 enemyCenter = EnemyTroop.GetComponent<CapsuleCollider> ().bounds.center;
			Vector3 barrierCenter = EnemyTroop.getPiece().GetComponent<BoxCollider> ().bounds.center;
			float DistanceToEnemy = Vector3.Distance (transform.position, enemyCenter);
			float DistanceToBarrier = Vector3.Distance (transform.position, barrierCenter);
			if (DistanceToEnemy > DistanceToBarrier) {
				deduct += 0.5f;
			}
		}

		if (EnemyTroop.inFoxHole) {
			deduct += 0.5f;
		}
			
		//Randomize hit based on distance and troop range
		float distance = Vector3.Distance (transform.position, EnemyTroop.transform.position);
		distance = distance + (distance * deduct);
		Debug.Log ("Distance is " + distance + " and range is " + getRange ());
		float hit = (Random.Range (0, getRange()) - distance) > 0 ? 1 : 0;
		Vector3 enemypos = EnemyTroop.transform.position + new Vector3(0f, 3f, 0f);
		Vector3 mypos = transform.position + new Vector3 (0f, 3f, 0f);
		if (isSniper) {
			isSniper = false;
			abilities.Remove (1);
			setRange (100f);
		}
		//determine if enemy is behind terrain or cover
		RaycastHit hitcast;
		Vector3 enemyWorldPos = EnemyTroop.transform.TransformPoint (Vector3.up * 3);
		if(Physics.Linecast(mypos, enemypos, out hitcast)){
			if(hitcast.collider.CompareTag("Terrain") || hitcast.collider.CompareTag("NaturalCover")){
				hit = 0;
			}
		}
		float[] targets = new float[3];
		targets [0] = id;
		targets [1] = EnemyTroop.id;
		targets [2] = hit;
		DidSomething ();
		object target = (object)targets;
		PhotonNetwork.RaiseEvent (4, target, true, GameHandler._instance.AllReceivers());
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
		if (isInvulnerable == false) {
			isInvulnerable = true;
			GameObject myShield = Instantiate (TroopController._instance.TroopObjects [3], gameObject.transform);
			myShield.GetComponent<MeshRenderer> ().material = TroopController._instance.ShieldMats [team];
		}
	}

	public void makeNotInvulnerable(){
		if (isInvulnerable == true) {
			isInvulnerable = false;
			GameObject myShield = transform.Find ("Shield(Clone)").gameObject;
			if (myShield != null) {
				Destroy (myShield);
			}
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
				myTroop.takingCover = true;
			} else {
				myTroop.takingCover = false;
			}
			myTroop.StopAllCoroutines ();
			myTroop.StartCoroutine (myTroop.moveToPosition (newPos, 10f)); 
		}
	}
		
	public void RaiseMovement(Vector3 point, int cover, int terrain){
		HudController._instance.AttackMode (false);
		Vector3 floor;
		if (terrain == 0) {
			floor = toFloor (point);
		} else {
			floor = point;
		}
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

	public void RaiseGrenade(Vector3 point){
		Vector3 floor = toFloor (point);
		Vector3 landing = ((floor - currentPosition).normalized) * grenadeMax;
		float[] contentsFloat = new float[4];
		contentsFloat [0] = id;
		if (Vector3.Distance (floor, transform.position) <= grenadeMax) {
			//if the click point is within the troops walking distance
			contentsFloat [1] = floor.x;
			contentsFloat [2] = floor.y;
			contentsFloat [3] = floor.z;
		} else {
			//find farthest point that troop can currently travel
			contentsFloat [1] = initialPosition.x + landing.x;
			contentsFloat [2] = initialPosition.y + landing.y;
			contentsFloat [3] = initialPosition.z + landing.z;
		}
		object contents = (object)contentsFloat;
		DidSomething ();
		PhotonNetwork.RaiseEvent ((byte)6, contents, true, new RaiseEventOptions(){
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true});
	}

	public void RaiseAirstrike(Vector3 point){
		float[] contentArray = new float[4] { id, point.x, point.y, point.z };
		object content = (object)contentArray;
		PhotonNetwork.RaiseEvent ((byte)12, content, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true,
		});
	}

	public void CallAirstrike(Vector3 point){
		hasAirStrike = false;
		abilities.Remove (5);
		DidSomething ();
		stab ();
		Vector3 skyPoint = point + new Vector3 (0f, 100f, 0f);
		GameObject myStrike = Instantiate (TroopController._instance.TroopObjects [7], skyPoint, Quaternion.Euler (new Vector3 (90, 0, 0))); 
		CameraController._instance.setFollowedObject (myStrike, 0);
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
		if (myPiece != null) {
			if (Vector3.Distance (currentPosition, destination) > Vector3.Distance (myPiece.GetComponent<BoxCollider> ().bounds.center, destination)) {
				animator.SetInteger ("AnimPar", 11);
				StartCoroutine (MoveAfterSeconds (0.5f));
			} else {
				animator.SetInteger ("AnimPar", 1);
			}
		} else {
			animator.SetInteger ("AnimPar", 1);
			myPiece = null;
		}
		myPiece = null;
		while(Vector3.Distance(transform.position, destination) > 2f)
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
		setMaxDistance (newDis);
		}
		
	public void shoot(Trooper enemy, int hit){
		StopAllCoroutines ();
		CameraController._instance.setFollowedObject (gameObject, 0);
		rotateTo (enemy.transform.position);
		animator.SetTrigger ("Shoot");
		StartCoroutine (ShootCoroutine (enemy, hit));
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

		if (enemy.myPiece != null) {
			hit = 2;
		}
		HudController._instance.HitOrMiss (mybullet.transform.position, hit);
		Destroy (mybullet);
	}
		
	public void gotShot(){
		StopAllCoroutines ();
		animator.SetTrigger ("Hit");
		decreaseHealth (40f);
	}

	public IEnumerator die(){
		CameraController._instance.setFollowedObject (gameObject, 1);
		unselect ();
		animator.SetTrigger ("Die");
		yield return new WaitForSeconds (2f);
		myPlayer.roster.Remove (this);
		Game._instance.allTroopers.Remove (this);
		myPlayer.CreateDogTagAt(transform.position + new Vector3 (0, 3f, 0), id);
		HudController._instance.removeHealthBar (id);
		GameHandler._instance.CheckForEnd ();
		Destroy (gameObject);
	}

		
	public void giveGrenade(bool give){
		if (give) {
			MessageScript._instance.setText ("Click on location to throw grenade");
			hasGrenade = true;
			ShowGrenadeLimit ();
		} else {
			hasGrenade = false;
			RemoveGLimit ();
		}
	}

	public void giveSniper(bool give){
		if (give) {
			isSniper = true;
			range += 100;
		} else {
			isSniper = false;
			range -= 100;
			if (range < 0) {
				range = 0;
			}
		}
	}

	public void giveInvulnerability(bool give){
		if (give) {
			PhotonNetwork.RaiseEvent (7, (object)id, true, new RaiseEventOptions () {
				Receivers = ReceiverGroup.All,
				ForwardToWebhook = true
			});
		} else {
			PhotonNetwork.RaiseEvent (8, (object)id, true, new RaiseEventOptions () {
				Receivers = ReceiverGroup.All,
				ForwardToWebhook = true
			});
		}
	}

	public void giveMarathon(bool give){
		if (give) {
			canMarathon = true;
			maxDistance += 50;
			resetDistance ();
			select ();
		} else {
			canMarathon = false;
			maxDistance -= 50;
			if (Vector3.Distance (currentPosition, initialPosition) > maxDistance) {
				transform.position = initialPosition;
				myPlayer.spendDogTags (1);
			}
			resetDistance ();
			select ();
		}
	}

	public void giveDoubleShot(bool give){
		if (give) {
			if (frozen == true) {
				frozen = false;
			} else {
				hasDoubleShot = true;
			}
		} else {
			hasDoubleShot = false;
		}
	}

	public void giveAirstrike(bool give){
		if (give) {
			MessageScript._instance.setText ("Click on location to call airstrike");
			hasAirStrike = true;
		} else {
			hasAirStrike = false;
		}
	}

	public void giveMedkit(bool give){
		if (give) {
			health = 100f;
			Instantiate (TroopController._instance.TroopObjects [8], currentPosition, currentRotation, transform);
			abilities.Remove (6);
		} else {

		}
	}

	public void giveNewTroop(bool give){
		if (give) {
			MessageScript._instance.setText ("New Troop Added");
			GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
			SpawnArea mySpawn = new SpawnArea ();
			foreach (GameObject g in spawns) {
				if (g.GetComponent<SpawnArea> ().team == myPlayer.team) {
					mySpawn = g.GetComponent<SpawnArea>();
				}
			}
			if (myPlayer.roster.Count < mySpawn.spawnPoints.Count) {
				Vector3 newPos = mySpawn.spawnPoints [myPlayer.roster.Count].position;
				myPlayer.RaiseTroopAt (newPos, mySpawn.FacingOut, myPlayer.team, Game._instance.allTroopers.Count);
			} else {
			}
			abilities.Remove (7);
		} else {
		}
	}

	public void giveAbility(int ability){
		bool give = abilities.Contains (ability);

		if (give) {
			MessageScript._instance.setText ("Item Sold");
			myPlayer.addDogTags (HudController._instance.GameHud.Store.ItemPrices [ability]);
			abilities.Remove (ability);
		} else {
			myPlayer.spendDogTags (HudController._instance.GameHud.Store.ItemPrices [ability]);
			abilities.Add (ability);
		}
		HudController._instance.RefreshStore ();
		HudController._instance.RefreshStore ();
		switch(ability){
		case 0:
			giveGrenade(!give);
			break;
		case 1:
			giveSniper (!give);
			break;
		case 2:
			giveMarathon (!give);
			break;
		case 3:
			giveInvulnerability (!give);
			break;
		case 4:
			giveDoubleShot (!give);
			break;
		case 5:
			giveAirstrike (!give);
			break;
		case 6:
			giveMedkit (!give);
			break;
		case 7:
			giveNewTroop (!give);
			break;
		default:
			break;
		}
	}

	public void throwGrenade(Vector3 target){
		StopAllCoroutines ();
		RemoveGLimit ();
		animator.SetTrigger ("Throw");
		StartCoroutine (throwCoroutine (target));
		Invoke ("stop", 1f);
	}

	public IEnumerator throwCoroutine(Vector3 position){
		hasGrenade = false;
		abilities.Remove (0);
		rotateTo (position);
		CameraController._instance.setFollowedObject (gameObject, 0);
		yield return new WaitForSeconds (0.5f);
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
		GameObject ex = GameObject.Instantiate (TroopController._instance.TroopObjects[2], myGrenade.transform.position, Quaternion.identity);
		Explosion now = ex.GetComponent<Explosion> ();
		if (now != null) {
			now.type = 1;
		}
		Destroy (myGrenade);
		yield return new WaitForSeconds (1f);
		Destroy (ex);
		stop ();
		if (covering == true) {
			animator.SetInteger ("AnimPar", 11);
		}
	}
		
		
	public void naded(){
		animator.SetTrigger ("Naded");
		Invoke ("stop", 1f);
	}

	public void stab(){
		animator.SetTrigger ("Knife");
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
		MessageScript._instance.setText ("Click on map to move trooper to locatoin");
		myPlayer.setSelected (this);
		if (this.frozen == false) {
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

	public void ShowGrenadeLimit(){
		GameObject limiter = Instantiate (TroopController._instance.TroopObjects [6], currentPosition, Quaternion.identity, transform);
		gLimit = limiter;
		gLimit.GetComponent<Projector> ().orthographicSize = 80;
		gLimit.transform.SetParent (null);
		gLimit.transform.rotation = Quaternion.Euler (90f, 0, 0);
	}

	public void RemoveGLimit(){
		if(gLimit!=null){
			Destroy (gLimit.gameObject);
		}
	}

	public void RemoveWalkLimit(){
		if(limit!=null){
			Destroy (limit.gameObject);
		}
	}

	public void goBack(float distance){
		Vector3 v = GetComponent<Rigidbody> ().velocity;
		Vector3 direction = gameObject.transform.eulerAngles + new Vector3 (0, 180, 0);
		Vector3 newPos = direction * distance;
		gameObject.transform.Translate (newPos.x, 0, newPos.z);
	}
		
	public void unselect(){
		if (myPlayer.getSelected() == this) {
			myPlayer.setSelected (null);
		}

		RemoveWalkLimit ();
		HudController._instance.AttackMode (false);	
		HudController._instance.CanAttack (false);
		if (this.frozen == false) {
		}
	}

	public void freeze(){
		frozen = true;
	}

	public void unFreeze(){
		frozen = false;
	}

	public void assignColor(){
		transform.Find ("TS_Body_01_infantry").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.TroopMats[team];
	}

	public void DidSomething(){
		stop ();
		unselect ();
		resetDistance ();
		select ();
		stop ();
	}

	public IEnumerator stabTroop(Trooper t){
		
		rotateTo (t.currentPosition);
		stab ();
		yield return new WaitForSeconds (0.5f);
		t.decreaseHealth (100f);

	}
		

	void OnCollisionEnter(Collision coll){
		Trooper t = coll.gameObject.GetComponent<Trooper> ();
		if (t != null) {
			if (t.team != team && moving==true) {
				covering = false;
				myPiece = null;
				StopAllCoroutines ();
				unselect ();
				setMaxDistance (2f);
				initialPosition = transform.position;
				StartCoroutine (stabTroop (t));
			}
		}
	}

	void OnParticleCollision(GameObject ex){
		if (ex != partSys) {
			partSys = ex;
			Explosion myEx = ex.GetComponent<Explosion> ();
			if (myEx != null) {
				if (myEx.type == 1) {
					decreaseHealth (60f);
				} else if (myEx.type == 2) {
					decreaseHealth (100f);
				}
			}
			HudController._instance.showHealthBar (id);
			rotateTo (ex.transform.position);
			naded ();
		}
	}
}
