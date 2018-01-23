using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public static Game _instance;
	public Player myPlayer;
	public float floor = 49f;
	//handles internal game events and lists


	public List<Trooper> allTroopers = new List<Trooper> ();
	public List<DogTag> allDogTags = new List<DogTag> ();


	//for click and drag differential
	private float firstTime;
	private float secondTime;
	private float timeDiff;
	private bool mouseDown = false;
	private bool dragOccuring = false;
	private Vector3 previousMousePosition;

	private bool barrierSelected = false;
	private bool rotatingBarrier = false;
	private BKnob selectedKnob;



	void Start () {
		_instance = this;
		PhotonNetwork.OnEventCall += GameHandler.CreatePlayer; //1
		PhotonNetwork.OnEventCall += Trooper.move; //2
		PhotonNetwork.OnEventCall += GameHandler.EndPlacements;//3
		PhotonNetwork.OnEventCall += Player.attack; //4
		PhotonNetwork.OnEventCall += GameHandler.changeTurn; //5
		PhotonNetwork.OnEventCall += Player.throwGrenade; //6
		PhotonNetwork.OnEventCall += Trooper.makeInvulnerable; //7
		PhotonNetwork.OnEventCall += Trooper.makeNotInvulnerable;//8
		PhotonNetwork.OnEventCall += Game.raiseBarrier; //15

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
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 300)) {
				if (hit.collider.CompareTag ("Barrier") && GameHandler._instance.getTurnNumber() == 0) {
					if (hit.transform.parent.parent.gameObject.GetComponent<Barrier> ().team == PhotonNetwork.player.ID) {
						myPlayer.barrierSelected = hit.collider.gameObject.GetComponent<BarrierPiece> ().myBarrier;
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
				if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
					dragOccuring = true;
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				mouseDown = false;
				dragOccuring = false;
				barrierSelected = false;
				rotatingBarrier = false;
				if (selectedKnob != null) {
					selectedKnob.selected = false;
					selectedKnob = null;
				}
				if (myPlayer != null) {
					myPlayer.barrierSelected = null;
				}
				if (timeDiff < .2f) {
					OnClick ();
				}
				timeDiff = 0;
			}
		}


	void OnDrag(){
		if (dragOccuring == true) {
			if (barrierSelected == false && rotatingBarrier == false) {
				//drag camera/screen with mouse

				Vector3 camDifference = Camera.main.ScreenToViewportPoint (Input.mousePosition) - previousMousePosition;
				Camera.main.transform.parent.transform.Translate (camDifference.y * 30f, 0f, camDifference.x * -30f);
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			} else if (barrierSelected == false) {
			} else {
				//moving barrier on drag
				if (myPlayer.barrierSelected.placed == false) {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 300)) {
						if (hit.collider.CompareTag ("Terrain")) {
							//BarrierPiece myPiece = Game._instance.GetBarrierPiece (myPlayer.barrierSelected.id);
							//myPiece.gameObject.transform.parent.parent.position = hit.point;
							myPlayer.barrierSelected.Cylinder.gameObject.transform.position = hit.point;
						}
					}
				}
			}
		}
	}
	//if click happens
	void OnClick (){
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (-1)) {

		} else { 
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 300)) {
				if (myPlayer.isTurn ()) {
					
					//if player is clicked
					if (hit.collider.CompareTag ("Player")) {
						Trooper clickedOn = hit.collider.gameObject.GetComponent<Trooper> ();
						if (myPlayer.team == clickedOn.team) {
								
							//if it is an ally trooper
							myPlayer.selectTrooper (clickedOn);
						} else {
							//if it is an enemy player
							if (myPlayer.Selected != null) {
										
								if (myPlayer.attacking == true) {
									//if current player is attacking
									myPlayer.RaiseAttack (clickedOn.id);
						
								} else if (myPlayer.Selected.hasGrenade) {
									//if current player is not attacking and player is carrying a grenade
									myPlayer.RaiseGrenade (hit.point);

								} else {
									HudController._instance.showHealthBar (clickedOn.id);
								}
							}
						}
							
					} else if (hit.collider.CompareTag ("Terrain") || hit.collider.CompareTag ("ControlPoint") || hit.collider.CompareTag ("NaturalCover")) {
						//if player clicked on terrain/ground
						if (myPlayer.Selected != null) {
							//if player is selected
							if (myPlayer.Selected.hasGrenade) {
								//if grenade is equipped
								myPlayer.RaiseGrenade(hit.point);
							} else {
								//regular player movement
								myPlayer.Selected.RaiseMovement (hit.point);
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
		
	public ControlPoint getConrolPoint(int idd){
		ControlPoint[] allObs = GameObject.FindObjectsOfType (typeof(ControlPoint)) as ControlPoint[];
		foreach(ControlPoint g in allObs){
			if(g.id == idd){
				return g;
			}
		}
		return null;
	}

	public void BeginGame(){
		Game._instance.SendBarriersToNetwork ();
		BarrierHandler._instance.RemoveAllPrelimbs ();
		Camera.main.transform.Rotate (new Vector3 (-45, 0, 0));
		Vector3 newPos = Camera.main.transform.position;

		Camera.main.transform.position = new Vector3 (newPos.x, 80, newPos.z);
		GameObject.Find ("Main Camera").GetComponent<CameraZoom> ().resetZoom ();
		HudController._instance.GameHud.nextTroopPan ();

		HudController._instance.BeginGame ();
	}

	public void StartTurn(){
		foreach (Trooper t in Game._instance.myPlayer.roster) {
			t.initialPosition = t.transform.position;
			t.maxDistance = 50f;
		}
		HudController._instance.StartTurn ();
		myPlayer.setTurn (true);
	}

	public void EndTurn(){
		foreach (Trooper t in Game._instance.myPlayer.roster) {
			t.unselect ();
		}

		HudController._instance.EndTurn ();
		myPlayer.setTurn (false);
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
				RaiseEventOptions rf = RaiseEventOptions.Default;
				rf.Receivers = ReceiverGroup.Others;
				PhotonNetwork.RaiseEvent (15, (object)info, true, rf);
			}
		}
	}



	public static void raiseBarrier(byte id, object content, int senderID){
		if (id == 15) {
			Debug.Log ("Raising a barrier");
			//if (PhotonNetwork.player.ID != senderID) {
				float[] info = (float[])content;
				Vector3 location = new Vector3 (info [2], info [3], info [4]);
				Vector3 Cylinderlocation = new Vector3 (info [5], info [6], info [7]);
				Quaternion angles = Quaternion.Euler (info [8], info [9], info [10]);
				int btype = (int)info [0];
				int bteam = (int)info [1];
				Player myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
				BarrierHandler._instance.CreateBarrier (btype, location, Cylinderlocation, angles, bteam);
			//}
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

}
