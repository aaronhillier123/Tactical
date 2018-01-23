using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	//identification
	public int team;
	public int lookingAt = 0;
	private bool isTurnBool = false;
	//Prefabs needed
	public GameObject TrooperObject;
	public GameObject HealthObject;
	public GameObject ChanceObject;
	public GameObject WaitingScreen;
	public GameObject HudObject;
	public GameObject StartHudOb;


	//state of player's troops
	public List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;
	public Barrier barrierSelected;
	public List<GameObject> barriers = new List<GameObject> ();

	//general player variables
	public static int numberOfTroops = 3;
	public int dogtags = 3;
	public List<ControlPoint> myControlPoints;

	//booleans for player states
	public bool attacking = false;
	public bool ready = false;


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	}

	public void spendDogTags(int amount){
		dogtags -= amount;
		Hud.updateDogTags (dogtags);
	}

	//create a new troop at a certain location
	public void CreateTroopAt(Vector3 location, Quaternion rotation, int troopTeam, int troopId){
		GameObject FirstTroopObject = Instantiate (TrooperObject, location, rotation, transform) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		if (firstTroop != null) {
			firstTroop.team = troopTeam;
			firstTroop.id = troopId;
			firstTroop.initialPosition = firstTroop.gameObject.transform.position;
			Game._instance.allTroopers.Add (firstTroop);
			roster.Add (firstTroop);
		}
	}

	public void addControlPoint(ControlPoint cp){
		myControlPoints.Add(cp);
	}



	public void RaiseAttack(int EnemyId){

		attacking = false;
		HudController._instance.removeChances ();
		float[] targets = new float[3];
		targets [0] = Selected.id;
		targets [1] = EnemyId;
		if (Game._instance.GetTroop (Selected.id).isSniper == false) {
			targets [2] = Random.Range (0, 100);
		} else {
			targets [2] = Random.Range (0, 200);
		}
		object target = (object)targets;
		PhotonNetwork.RaiseEvent (4, target, true, EventHandler.ops);

	}
	//network attack function
	public static void attack(byte id, object content, int senderID){
		if (id == 4) {
			//unpack objects for attacker and target

			float[] contents = (float[])content;
			Trooper myTroop = Game._instance.GetTroop ((int)contents [0]);
			Trooper enemy = Game._instance.GetTroop ((int)contents [1]);
			myTroop.isSniper = false;

			RaycastHit hit;
			Vector3 enemypos = enemy.gameObject.transform.position;
			Vector3 mypos = myTroop.gameObject.transform.position;
			Vector3 enemyhip = new Vector3 (enemypos.x, enemypos.y + 3, enemypos.z);
			Vector3 myhip = new Vector3 (mypos.x, mypos.y + 3, mypos.z);
			Vector3 dir = (enemyhip - myhip);
			if (Physics.Raycast (myhip, dir, out hit, 200f)) {
				
				if (hit.collider.CompareTag ("NaturalCover")) {
					myTroop.rotateTo (enemy.gameObject.transform.position);
					myTroop.animator.SetInteger ("AnimPar", 2);
					myTroop.miss (enemy.gameObject);
				} else {
					//find distance and see if attack is a hit
					float distance = Vector3.Distance (myTroop.gameObject.transform.position, enemy.gameObject.transform.position);
					float random = contents [2];
					myTroop.rotateTo (enemy.gameObject.transform.position);
					myTroop.animator.SetInteger ("AnimPar", 2);
					if (random - distance > 0) {
						myTroop.shoot (enemy.gameObject);
					} else {
						myTroop.miss (enemy.gameObject);
					}
				}
					myTroop.Invoke ("stop", 1f);

					myTroop.unselect ();
					myTroop.freeze ();
					HudController._instance.AttackMode (false);
				}
			}
		}


	public void RaiseGrenade(Vector3 point){

		Selected.resetDistance ();
		float[] contents1 = new float[4];
		contents1 [0] = (float)Selected.id;
		contents1 [1] = point.x;
		contents1 [2] = point.y;
		contents1 [3] = point.z;
		object contents = (object)contents1;
		PhotonNetwork.RaiseEvent ((byte)6, contents, true, EventHandler.ops);
	}
	//network grenade function
	public static void throwGrenade(byte id, object content, int senderID){
		if (id == 6) {
			float[] conList = (float[])content;
			int selectedID = (int)conList [0];
			float newPosx = conList [1];
			float newPosy = conList [2];
			float newPosz = conList [3];
			Vector3 newPos = new Vector3 (newPosx, newPosy, newPosz);
			Trooper myTroop = Game._instance.GetTroop (selectedID);
			myTroop.animator.SetInteger ("AnimPar", 3);
			myTroop.throwGrenade (newPos);
			myTroop.unselect ();
		}
	}
		
	//select a specific trooper



	public void selectTrooper(Trooper a){
		a.select();
	}

	public bool isTurn(){
		return isTurnBool;
	}

	public void setTurn(bool isMyTurn){
		isTurnBool = isMyTurn;
	}

}
