using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trooper : MonoBehaviour {

	//miscellaneous
	public GameObject hitmisOb;
	public float health = 100;
	public GameObject limit;

	//animation
	public Animator animator;
	private int animInt;
	public float maxDistance = 50f;
	//Seperate Objects




	//for movement
	public Vector3 initialPosition;

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
		HudController._instance.AttackMode (false);	
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
		GameObject mybullet = Instantiate (TroopController._instance.TroopObjects[0], startpos, Quaternion.identity);
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
		animator.SetInteger ("AnimPar", 6);
		yield return new WaitForSeconds (2f);
		myPlayer.roster.Remove (this);
		Game._instance.allTroopers.Remove (this);
		GameObject newDogTag = Instantiate (TroopController._instance.TroopObjects[4], transform.position + new Vector3 (0, 3f, 0), Quaternion.identity);
		Game._instance.allDogTags.Add(newDogTag.GetComponent<DogTag>());
		HudController._instance.removeHealthBar (id);
		Destroy (gameObject);
	}
		
	IEnumerator missThis(GameObject target){
		yield return new WaitForSeconds (1f);
		Vector3 p = gameObject.transform.position;
		float xoff = Random.Range (-4, 4);
		float yoff = Random.Range (-4, 4);
		float zoff = Random.Range (-4, 4);
		Vector3 startpos = new Vector3 (p.x, p.y + 5, p.z);
		GameObject mybullet = Instantiate (TroopController._instance.TroopObjects[0], startpos, Quaternion.identity);
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
		GameObject myGrenade = Instantiate (TroopController._instance.TroopObjects[1], pos, Quaternion.identity); 
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

	public float distanceTo(Vector3 target){
		return (Vector3.Distance(gameObject.transform.position, target));
	}

	public void select(){
		if (myPlayer.Selected != null) {
			myPlayer.Selected.unselect ();
		}
		myPlayer.Selected = this;
		if (this.frozen == false) {
			transform.Find ("Trooper").GetComponent<SkinnedMeshRenderer> ().material = TroopController._instance.SelectedMats[team];
			HudController._instance.AttackMode (true);
		}
		ShowWalkLimit ();
		HudController._instance.RefreshStore ();
		}

	public void rotateTo(Vector3 point){
		Vector3 direction = (point - gameObject.transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation (direction);
		gameObject.transform.rotation = lookRotation;
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

}
