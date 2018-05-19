using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Trooper : MonoBehaviour {

	//models
	public GameObject MoveOutline;
	public GameObject Bullet;

	//miscellaneous
	//public GameObject hitmisOb;
	protected GameObject limit;

	//animation
	public Animator animator;
	public SkinnedMeshRenderer body;
	public SkinnedMeshRenderer Head;
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

	protected int animInt;

	public float maxMoveDistance = 50;
	public float maxHealth = 100;
	private float moveDistance = 50;
	private float health = 100;

	public float shootRange = 100;
	public float throwMax = 40;

	public List<GameObject> raceAbilities = new List<GameObject> ();
	public List<GameObject> currentAbilities = new List<GameObject> ();
	public GameObject activeAbility;

	protected BarrierPiece myPiece;
	protected Vector3 initialPosition;
	protected bool updated = false;
	GameObject partSys;

	//identification
	public int id;
	public int team;

	//abilities
	public Player myPlayer;
	public Vector3 currentPosition;
	public Vector3 destinationPosition;
	public Quaternion currentRotation;
	public Trooper target;

	//state of availability
	protected bool moving = false;
	public bool frozen = false;
	public bool covering = false;
	public bool takingCover = false;
	public bool inFoxHole = false;
	public bool jumping = false;
	public bool isInvulnerable = false;
	public bool shooting = false;

	void awake () {
		myPlayer = GetComponentInParent<Player>();
		body.material.shader = TroopController._instance.outlined;
		assignColor (); 
	}

	void Update(){
		if (moving == false) {
			currentPosition = transform.position;
		} else {
			currentPosition = destinationPosition;
		}
		currentRotation = transform.rotation;
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
		shootRange = r;
	}
	public float getRange(){
		return shootRange;
	}
	public Vector3 getInitPosition(){
		return initialPosition;
	}
	public float getGrenadeMax(){
		return throwMax;
	}
	public void setMoveDistance(float m){
		moveDistance = m;
		if (moveDistance < 2) {
			moveDistance = 2;
		}
	}
	public float getMoveDistance(){
		return moveDistance;
	}
	public void SetUpdated(bool ud){
		updated = ud;
	}
	public bool isUpdated(){
		return updated;
	}
	public List<GameObject> GetAbilities(){
		return currentAbilities;
	}
	private void AddAbility(GameObject a){
		currentAbilities.Add(a);
	}
	public bool isMoving(){
		return moving;
	}
	public float getHealth(){
		return health;
	}
	public void setHealth(float h){
		health = h;
	}
	public float getMaxHealth(){
		return maxHealth;
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
		setMoveDistance(maxMoveDistance);
		initialPosition = transform.position;
		currentAbilities.Clear ();
		giveAbility (0);
		if (indexOfCurrentAbility(5) != -1) {
			
		} else {
		}
		unFreeze ();
		setOutlineColor (3);
	}
		
	public void flagPull(){
		animator.SetTrigger("Flag");
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
			if (myPlayer.getSelected () == this) {
				myPlayer.selectTrooper (this);
			}
		}
	}

	public void jumpBarrier(){
		if (jumping == false) {
			jumping = true;
			int currentAnimation = animator.GetInteger ("AnimPar");
			animator.SetTrigger ("Jump");
			transform.position += Vector3.up * 4;
		}
	}
		
	public static void move(byte id, object content, int senderID){
		if (id == 2) {
			float[] conList = (float[])content;
			int cover = (int)conList [4];
			Vector3 newPos = new Vector3 (conList[1], conList[2], conList[3]);
			Trooper myTroop = Game._instance.GetTroop ((int)conList[0]);
			myTroop.takingCover = ((int)conList [4] == 1) ? true : false;
			myTroop.StopAllCoroutines ();
			myTroop.StartCoroutine (myTroop.moveToPosition (newPos, 10f)); 
		}
	}

	public void RaiseMovement(Vector3 point, int cover, int terrain){
		HudController._instance.AttackMode (false);
		float[] contents = new float[6];
		bool inRange = (Vector3.Distance (point, initialPosition) <= moveDistance);
		point = (inRange) ? point : initialPosition + (((point - initialPosition).normalized) * moveDistance);
		contents [0] = (float)id;
		contents [1] = point.x;
		contents [2] = point.y;
		contents [3] = point.z;
		contents [4] = (float)cover;
		HudController._instance.CanAttack (false);	
		Debug.Log ("raising movement");
		PhotonNetwork.RaiseEvent ((byte)2, (object)contents, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All,
			ForwardToWebhook = true
		});
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
		float newDis = moveDistance - (Vector3.Distance(initialPosition, transform.position));
		initialPosition = transform.position;
		setMoveDistance (newDis);
		}
		
	public void shoot(Trooper enemy, int hit){
		StopAllCoroutines ();
		CameraPan._instance.moveToObject (gameObject, false);
		rotateTo (enemy.transform.position);
		shooting = true;
		StartCoroutine (ShootCoroutine (enemy, hit));
	}

	IEnumerator ShootCoroutine(Trooper enemy, int hit){
		yield return new WaitForSeconds (0.8f);
		animator.SetTrigger ("Shoot");
		yield return new WaitForSeconds (0.5f);
		Vector3 startpos = transform.position + new Vector3 (0, 5, 0);
		Vector3 enemyPos = enemy.transform.position + new Vector3 (0, 5, 0);
		GameObject mybullet = Instantiate (Bullet, startpos, Quaternion.identity);
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
		shooting = false;
		HudController._instance.HitOrMiss (mybullet.transform.position, hit);
		Destroy (mybullet);
		GameHandler._instance.refreshGameStates ();
	}

	public void gotShot(){
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
		myPlayer.createDogTagAt(transform.position + new Vector3 (0, 3f, 0), id);
		HudController._instance.removeHealthBar (id);
		GameHandler._instance.CheckForEnd ();
		Destroy (gameObject);
	}
		
	public void makeInvulnerable(){
		isInvulnerable = true;
	}

	public void makeNotInvulnerable(){
		isInvulnerable = false;
	}

	public void executeAbility(Vector3 target){
		int[] content = new int[4];
		content[0] = id;
		content[1] = (int)target.x;
		content[2] = (int)target.y;
		content[3] = (int)target.z;

		PhotonNetwork.RaiseEvent((byte)20, (object)content, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All, 
			ForwardToWebhook = true
		});
	}
		
	public void removeAbility(int abilityId){
		int[] content = new int[]{id, abilityId};
	

		PhotonNetwork.RaiseEvent((byte)22, (object)content, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All, 
			ForwardToWebhook = true
		});
	}
		
	public void giveAbility(int ability){
		int[] content = new int[]{id, ability};
		content [0] = id;
		content [1] = ability;

		PhotonNetwork.RaiseEvent((byte)21, (object)content, true, new RaiseEventOptions () {
			Receivers = ReceiverGroup.All, 
			ForwardToWebhook = true
		});
	}

	public static void networkGiveAbility(byte id, object content, int senderId){
		if(id == 21){
			int[] contentArray = (int[])content;
			int troopId = contentArray [0];
			int abilityId = contentArray [1];
			Trooper myTroop = Game._instance.GetTroop (troopId);
			Player troopPlayer = myTroop.myPlayer;
			int indexOfAbility = myTroop.indexOfCurrentAbility (abilityId);
			myTroop.clearActiveAbility ();
			//remove if troop already has ability
			if (indexOfAbility != -1) {
				
			} else {
				//else give ability and execute phase 0
				int raceIndex = myTroop.indexOfRaceAbility (abilityId);
				troopPlayer.spendDogTags (myTroop.raceAbilities[raceIndex].GetComponent<Ability> ().price);
				GameObject newAbility = Instantiate (myTroop.raceAbilities [raceIndex], myTroop.currentPosition, myTroop.currentRotation, myTroop.transform);
				newAbility.GetComponent<Ability> ().myTroop = myTroop;
				newAbility.GetComponent<Ability> ().giveControl ();
				myTroop.AddAbility (newAbility);
				myTroop.activeAbility = newAbility;
				newAbility.GetComponent<Ability> ().execute (Vector3.zero);
				if (myTroop.team == PhotonNetwork.player.ID) {
					HudController._instance.refreshAbilityPanel (myTroop);
				}
			}
			HudController._instance.RefreshStore ();
		}
	}

	public static void networkRemoveAbility(byte id, object content, int senderId){
		if (id == 22) {
			int[] contentArray = (int[])content;
			int troopId = contentArray [0];
			int abilityId = contentArray [1];
			Trooper myTroop = Game._instance.GetTroop (troopId);
			Player troopPlayer = myTroop.myPlayer;
			int indexOfAbility = myTroop.indexOfCurrentAbility (abilityId);
			GameObject oldAbility = myTroop.currentAbilities [indexOfAbility];
			myTroop.clearActiveAbility ();
			myTroop.currentAbilities.RemoveAt (indexOfAbility);
			Destroy (oldAbility);
			if (PhotonNetwork.player.ID == senderId) {
				HudController._instance.refreshAbilityPanel (myTroop);
			}
		}
	}

	public static void networkExecuteAbility(byte id, object content, int senderId){
		if (id == 20) {
			Debug.Log (content);
			int[] contentArray = (int[])content;
			int troopId = (int)contentArray [0];
			Vector3 target = new Vector3 ((float)contentArray [1], (float)contentArray [2], (float)contentArray [3]);
			Ability currentAbility = Game._instance.GetTroop (troopId).activeAbility.GetComponent<Ability> ();
			currentAbility.execute (target);
		}
	}

	public int indexOfRaceAbility(int abilityId){
		for(int i=0; i<raceAbilities.Count; ++i){
			if(raceAbilities[i].GetComponent<Ability>().id == abilityId){
				return i;
			}
		}
		return -1;
	}

	public int indexOfCurrentAbility(int abilityId){
		for(int i=0; i<currentAbilities.Count; ++i){
			if(currentAbilities[i].GetComponent<Ability>().id == abilityId){
				return i;
			}
		}
		return -1;
	}

	public void naded(){
		animator.SetTrigger ("Naded");
	}

	public void stab(){
		animator.SetTrigger ("Knife");
	}
		
	public void decreaseHealth(float dec){
		if (!isInvulnerable) {
			HudController._instance.showHealthBar (id);
			health -= dec;
			if (health <= 0) {
				StartCoroutine (die ());
			} else {
				Invoke ("stop", 1f);
			}
		}
	}
		
	public void select(){
		if (myPlayer.getSelected() != null) {
			myPlayer.getSelected().unselect ();
		}
		MessageScript._instance.setText ("Click on map to move trooper to location");
		setOutlineColor (0);
		myPlayer.setSelected (this);
		if (this.frozen == false) {
			HudController._instance.CanAttack (true);
		}
		ShowWalkLimit ();
		HudController._instance.RefreshStore ();
		HudController._instance.refreshAbilityPanel (this);
		}

	public void rotateTo(Vector3 point){
		Vector3 direction = (point - gameObject.transform.position).normalized;
		gameObject.transform.rotation = Quaternion.LookRotation (direction);
	}
		
	public void ShowWalkLimit(){
		if (team == PhotonNetwork.player.ID) {
			GameObject limiter = Instantiate (MoveOutline, initialPosition, Quaternion.identity, transform);
			limit = limiter;
			limit.GetComponent<Projector> ().orthographicSize = moveDistance * 1.9f;
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
		Vector3 v = GetComponent<Rigidbody> ().velocity;
		Vector3 direction = gameObject.transform.eulerAngles + new Vector3 (0, 180, 0);
		Vector3 newPos = direction * distance;
		gameObject.transform.Translate (newPos.x, 0, newPos.z);
	}
		
	public void unselect(){
		if (myPlayer.getSelected() == this) {
			myPlayer.setSelected (null);
		}
		setOutlineColor (4);
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
		body.material = TroopController._instance.TroopMats[team];
		Head.material =TroopController._instance.TroopMats[team];
	}

	public void DidSomething(){
		unselect ();
		resetDistance ();
		select ();
	}

	public void stabTroop(Trooper t){
		rotateTo (t.currentPosition);
		stab ();
		t.decreaseHealth (100f);
	}
		
	void OnCollisionEnter(Collision coll){
		Trooper t = coll.gameObject.GetComponent<Trooper> ();
		if (t != null && t.health > 0) {
			if (t.team != team && team == GameHandler._instance.getPlayersTurn()) {
				covering = false;
				myPiece = null;
				StopAllCoroutines ();
				unselect ();
				setMoveDistance (2f);
				initialPosition = transform.position;
				//StartCoroutine (stabTroop (t));
				stabTroop(t);
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
		
	public void clearActiveAbility(){
		foreach (GameObject g in currentAbilities) {
			g.GetComponent<Ability> ().removeControl ();
		}
		activeAbility = null;
	}

	public void setOutlineColor(int color){
		body.material.SetFloat ("_Outline", 0.15f);
		Head.material.SetFloat ("_Outline", 0.15f);

		switch(color){
			case 0:
				body.material.SetColor ("_OutlineColor", Color.green);
				Head.material.SetColor ("_OutlineColor", Color.green);
				break;
			case 1:
				body.material.SetColor ("_OutlineColor", Color.yellow);
				Head.material.SetColor ("_OutlineColor", Color.yellow);
				break;
			case 2:
				body.material.SetColor ("_OutlineColor", Color.red);
				Head.material.SetColor ("_OutlineColor", Color.red);
				break;
			default:
				body.material.SetColor ("_OutlineColor", Color.clear);
				Head.material.SetColor ("_OutlineColor", Color.clear);
				break;
		}
	}


}
