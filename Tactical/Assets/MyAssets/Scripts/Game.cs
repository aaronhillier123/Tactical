using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using System.Linq;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Game : MonoBehaviour {

	public static Game _instance;
	public Player myPlayer;
	public float floor = 100f;
	//handles internal game events and lists


	public List<Trooper> allTroopers = new List<Trooper> ();
	public List<DogTag> allDogTags = new List<DogTag> ();


	//for click and drag differential
	private float firstTime;
	private float secondTime;
	private Vector3 startPos = new Vector3();
	private float timeDiff;
	private bool mouseDown = false;
	private bool dragOccuring = false;
	private Vector3 previousMousePosition;

	private bool barrierSelected = false;
	private bool rotatingBarrier = false;
	public bool over = false;
	private BKnob selectedKnob;
	private float clickOrDrag = .4f; 


	void Start () {
		_instance = this;

	}
	
	// Update is called once per frame
	void Update () {
		if(myPlayer==null){
			try{
				myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
			} catch{
			}
		}

		OnDrag ();
		//On Drag or Click
		if (Input.GetMouseButtonDown (0)) {
			startPos = Input.mousePosition;
			foreach (Coroutine c in CameraPan._instance.momentums) {
				if (c != null) {
					CameraPan._instance.StopCoroutine (c);
				}
			}
			CameraPan._instance.momentum = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.collider.CompareTag ("Barrier") && GameHandler._instance.getTurnNumber() == 0) {
					if (hit.transform.parent.parent.gameObject.GetComponent<Barrier> ().team == PhotonNetwork.player.ID) {
						myPlayer.setBarrierSelected(hit.collider.gameObject.GetComponent<BarrierPiece> ().myBarrier);
						barrierSelected = true;
					}
				} else if (hit.collider.CompareTag ("Prelimb")) {
					if (hit.transform.parent.parent.gameObject.GetComponent<Barrier>().team == PhotonNetwork.player.ID) {
						selectedKnob = hit.transform.gameObject.GetComponent<BKnob> ();
						selectedKnob.selected = true;
						rotatingBarrier = true;
					}
				}
					previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
					firstTime = Time.time;
					mouseDown = true;
			}
		}

		if (mouseDown == true) {
			timeDiff = Time.time - firstTime;
		}
		if (timeDiff > 0f) {
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject () || !anyPointerOverObject()) {
				//foreach (Coroutine c in CameraPan._instance.momentums) {
				//	CameraPan._instance.StopCoroutine (c);
				//}				//CameraPan._instance.momentum = Vector3.zero;
				CameraPan._instance.drag ();
				dragOccuring = true;
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			mouseDown = false;
			CameraPan._instance.release ();
			//CameraPan._instance.dragging = false;
			dragOccuring = false;
			barrierSelected = false;
			rotatingBarrier = false;
			if (selectedKnob != null) {
				selectedKnob.selected = false;
				selectedKnob = null;
			}
			if (myPlayer != null) {
				myPlayer.setBarrierSelected (null);
			}
			if (timeDiff < clickOrDrag && over==false && Vector3.Distance(startPos, Input.mousePosition) < 5f) {
				OnClick ();
			}
			timeDiff = 0;
		}
	}
		

	void OnDrag(){
		if (CameraPan._instance.dragging== true) {
			if (barrierSelected == false && rotatingBarrier == false) {
				//drag camera/screen with mouse

				Vector3 camDifference = Camera.main.ScreenToViewportPoint (Input.mousePosition) - previousMousePosition;
				//Camera.main.transform.parent.transform.Translate (camDifference.y * 30f, 0f, camDifference.x * -30f);
				CameraPan._instance.transform.Translate (camDifference.y * 30f, 0f, camDifference.x * -30f);
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);


			} else if (barrierSelected == false) {
			} else {
				//moving barrier on drag
				if (myPlayer.getBarrierSelected().placed == false) {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 1000)) {
						if (hit.collider.CompareTag ("Terrain")) {
							//BarrierPiece myPiece = Game._instance.GetBarrierPiece (myPlayer.barrierSelected.id);
							//myPiece.gameObject.transform.parent.parent.position = hit.point;
							myPlayer.getBarrierSelected().Cylinder.gameObject.transform.position = hit.point;
						}
					}
				}
			}
		}
	}


	private bool anyPointerOverObject(){
		PointerEventData pointerData = new PointerEventData (EventSystem.current);
		pointerData.position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (pointerData, results);
		return results.Count > 0;
	}

	//if click happens
	void OnClick (){
		if (anyPointerOverObject() || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)) {
			
		} else { 
			foreach (Coroutine c in CameraPan._instance.momentums) {
				if (c != null) {
					CameraPan._instance.StopCoroutine (c);
				}
			}
			CameraPan._instance.momentum = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 1000)) {
				if (myPlayer.isTurn ()) {
					//if a trooper is clicked
					if (hit.collider.CompareTag ("Player")) {
						Trooper clickedOn = hit.collider.gameObject.GetComponent<Trooper> ();
						if (myPlayer.team == clickedOn.team) {
							//if it is an ally trooper
							myPlayer.selectTrooper (clickedOn);
						} else {
							//if it is an enemy player
							Debug.Log("enemyTroop clicked");
							if (myPlayer.getSelected() != null) {
								Debug.Log("selected not null");

								if (myPlayer.getSelected().activeAbility!=null) {
									if (myPlayer.getSelected ().activeAbility.GetComponent<Ability> ().passive == false) {
										Debug.Log("active ability");

										//if current player has an active ability
										myPlayer.getSelected ().executeAbility (hit.point);
									} else {
										Debug.Log("passive ability");

										myPlayer.getSelected ().activeAbility.GetComponent<Ability> ().passiveExecute (hit);
									}
								}
							}
						}
					} else {
						//if player clicked on terrain/ground
						if (myPlayer.getSelected() != null) {
							//if player is selected
							if (myPlayer.getSelected ().activeAbility != null) {
								if(myPlayer.getSelected().activeAbility.GetComponent<Ability>().passive==false){//if current player has an active ability
									myPlayer.getSelected ().executeAbility (hit.point);
								} else {
									myPlayer.getSelected ().activeAbility.GetComponent<Ability> ().passiveExecute (hit);
								}
							} else {
								if (!hit.collider.CompareTag ("Background")) {
									Debug.Log ("should move");

									//regular player movement
									if (hit.collider.CompareTag ("Barrier")) {
										myPlayer.getSelected ().takingCover = true;
										myPlayer.getSelected ().RaiseMovement (hit.point, 1, 0);
									} else if (hit.collider.CompareTag ("Terrain")) {
										myPlayer.getSelected ().takingCover = false;
										myPlayer.getSelected ().RaiseMovement (hit.point, 0, 1);
									} else {
										myPlayer.getSelected ().takingCover = false;
										myPlayer.getSelected ().RaiseMovement (hit.point, 0, 0);
									}
								}
							}
						}
					}
				}
			}
		}
	}


	//list of all troopers who are NOT a specifc player's
	public List<Trooper> notMyTroopers(Player p){
		List<Trooper> nmt = new List<Trooper> ();
		foreach(Trooper t in Game._instance.allTroopers){
			if(t.team != p.team){
				nmt.Add(t);
			}
		}
		return nmt;
	}
		
	public int generateNewId(){
		int max = 0;
		foreach (Trooper t in allTroopers) {
			if (t.id > max) {
				max = t.id;
			}
		}
		max += max;
		return max;
	}

	public ControlPoint getConrolPoint(int idd){
		ControlPoint[] allObs = GameObject.FindObjectsOfType (typeof(ControlPoint)) as ControlPoint[];
		foreach(ControlPoint g in allObs){
			if(g.id == idd){
				return g;
			}
		}
		return null;
	}

	public List<ControlPoint> allControlPoints(){
		ControlPoint[] cps = (GameObject.FindObjectsOfType (typeof(ControlPoint)) as ControlPoint[]);
		return cps.ToList();
	}

	public static void BeginGame(byte id, object content, int senderID){

		if (id == 11 && senderID == PhotonNetwork.player.ID) {
			CameraController._instance.normalView ();
			HudController._instance.GameHud.nextTroopPan ();
			HudController._instance.BeginGame ();
		}
	}
		
	public void StartTurn(){
		
		foreach (Trooper t in Game._instance.myPlayer.roster) {
			t.reset ();
		}
		myPlayer.addDogTags (myPlayer.myControlPoints.Count);
		Debug.Log ("added " + myPlayer.myControlPoints.Count + " dog tags");
		MessageScript._instance.setText ("Click on a trooper to select it");
		if (Game._instance.myPlayer.roster.Count == 0) {
			GameHandler._instance.RaiseTurnChange ();
		} else {
			HudController._instance.StartTurn ();
			myPlayer.setTurn (true);
		}
	}

	public void EndTurn(){

		foreach (Trooper t in Game._instance.myPlayer.roster) {
			try{
			t.unselect ();
			}catch{
			}
		}
		
		HudController._instance.EndTurn ();
		myPlayer.setTurn (false);
	}
		
	public void giveAbility(int ability){
		if (myPlayer.getSelected () != null) {
			Debug.Log ("giving ability " + ability);
			myPlayer.getSelected ().giveAbility (ability);
		}
		timeDiff = clickOrDrag;
		HudController._instance.GameHud.Store.Invoke ("removeInfoPanel", .1f);
		//HudController._instance.GameHud.Store.removeInfoPanel ();
		HudController._instance.RefreshStore ();
	}

	public void SendBarriersToNetwork(){
		foreach (Barrier b in BarrierHandler._instance.allBarriers) {
			GameObject g = b.gameObject;
			if (b.team == Game._instance.myPlayer.team) {
				float[] info = new float[11];
				info [0] = b.type;
				info [1] = b.team;
				info [2] = g.transform.position.x;
				info [3] = g.transform.position.y;
				info [4] = g.transform.position.z;
				info [5] = b.Cylinder.transform.position.x;
				info [6] = b.Cylinder.transform.position.y;
				info [7] = b.Cylinder.transform.position.z;
				info [8] = b.Cylinder.transform.rotation.eulerAngles.x;
				info [9] = b.Cylinder.transform.rotation.eulerAngles.y;
				info [10] = b.Cylinder.transform.rotation.eulerAngles.z;
				PhotonNetwork.RaiseEvent (15, (object)info, true, GameHandler._instance.OtherReceivers());
			}
		}
	}

	public static void raiseBarrier(byte id, object content, int senderID){
		if (id == 15) {
				float[] info = (float[])content;
				Vector3 location = new Vector3 (info [2], info [3], info [4]);
				Vector3 Cylinderlocation = new Vector3 (info [5], info [6], info [7]);
				Quaternion angles = Quaternion.Euler (info [8], info [9], info [10]);
				int btype = (int)info [0];
				int bteam = (int)info [1];
				Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
				BarrierHandler._instance.CreateBarrier (btype, location, Cylinderlocation, angles, bteam);
		}
	}
		
	//Return trooper based on ID
	public Trooper GetTroop(int id){
		foreach (Trooper t in allTroopers){
			if(t.id == id){
				return t;
			}
		}
		return null;
	}

	public static Vector3 midPoint(Vector3 start, Vector3 finish){
		return (start + finish) / 2;
	}

	public DogTag GetTag(int id){
		foreach (DogTag dg in allDogTags) {
			if (dg.id == id) {
				return dg;
			}
		}
			return null;
	}

	public float getChanceOfHit(Trooper shooter, Trooper target){
		float distance = Vector3.Distance (shooter.transform.position, target.transform.position);
		float percentOfHit = 100 - ((distance / shooter.getRange ()) * 100);


		Vector3 enemypos = target.transform.position + new Vector3(0f, 3f, 0f);
		Vector3 mypos = shooter.transform.position + new Vector3 (0f, 3f, 0f);

		//under cover may be blocked by barrier
		if (target.getPiece() != null) {
			Vector3 enemyCenter = target.GetComponent<CapsuleCollider> ().bounds.center;
			Vector3 barrierCenter = target.getPiece().GetComponent<BoxCollider> ().bounds.center;
			float DistanceToEnemy = Vector3.Distance (transform.position, enemyCenter);
			float DistanceToBarrier = Vector3.Distance (transform.position, barrierCenter);
			if (DistanceToEnemy > DistanceToBarrier) {
				percentOfHit = percentOfHit / 2;
			}
		}

		//if target is in foxhole
		if (target.inFoxHole) {
			percentOfHit = percentOfHit / 2;
		}

		//determine if enemy is behind terrain or cover
		RaycastHit hitcast;

		if(Physics.Linecast(mypos, enemypos, out hitcast)){
			if(hitcast.collider.CompareTag("Terrain") || hitcast.collider.CompareTag("NaturalCover")){
				percentOfHit = 0;
			}
		}

		if (percentOfHit < 0) {
			percentOfHit = 0;
		}
		return percentOfHit;
	}
}
