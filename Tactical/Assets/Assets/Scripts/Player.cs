using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {

	public int team;
	public GameObject TrooperObject;
	private List<Trooper> roster = new List<Trooper>();
	public Trooper Selected;
	public Game game;
	public int numberOfTroops = 2;


	private float firstTime;
	private float secondTime;
	private float timeDiff;
	private bool mouseDown = false;
	private bool dragOccuring = false;


	private Vector3 previousMousePosition;
	// Use this for initialization
	void Start () {
		game = GameObject.FindObjectOfType(typeof(Game)) as Game;
		Vector3 MyLocation = new Vector3 (team * 10f, 0f, 60f);
		for (int id = 0; id < numberOfTroops; ++id) {
			Vector3 TroopLocation = new Vector3 (team * 10f, 0f, (id *10f) + 60f);
			CreateTroopAt (TroopLocation, team, id);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (dragOccuring == true) {
			//adjust camera from drag
			if (previousMousePosition == null) {
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			} else {
				Vector3 camDifference = Camera.main.ScreenToViewportPoint(Input.mousePosition) - previousMousePosition;
				//Debug.Log("Old mousepos was " + previousMousePosition.x ", " + previousMousePosition.y + " and new mousepos is " + 
				Camera.main.transform.parent.transform.Translate (camDifference.y * 20f, 0f, camDifference.x * -20f);
				previousMousePosition = previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			}
		}
			//first click time
			if (Input.GetMouseButtonDown (0)) {
				previousMousePosition = Camera.main.ScreenToViewportPoint (Input.mousePosition);
				firstTime = Time.time;
				mouseDown = true;
			}

			if (mouseDown == true) {
				timeDiff = Time.time - firstTime;
			}


			if (timeDiff > 0f) {
				dragOccuring = true;
			}

			if (Input.GetMouseButtonUp (0)) {
				mouseDown = false;
				dragOccuring = false;
				if (timeDiff < .2f) {
					OnClick ();
				}
			timeDiff = 0;
		}
	}

	void OnClick (){
		if (EventSystem.current.IsPointerOverGameObject (-1)) {    // is the touch on the GUI
			return;
		} else {
			if (game.playersTurn == team) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, 100)) {
					if (hit.collider.CompareTag ("Player")) {
						Trooper clickedOn = hit.collider.gameObject.GetComponent<Trooper> ();
						if (team == clickedOn.team) {
							selectTrooper (hit.collider.gameObject.GetComponent<Trooper> ());
						}
					} else {
						if (Selected != null) {
							Selected.StopAllCoroutines ();
							Selected.StartCoroutine (Selected.moveToPosition (hit.point, 15f));
						}
					}
				}
			}
		}
	}

	void CreateTroopAt(Vector3 location, int troopTeam, int troopId){
		GameObject FirstTroopObject = Instantiate (TrooperObject, transform, true) as GameObject;
		FirstTroopObject.transform.position = location;
		Trooper firstTroop = FirstTroopObject.GetComponent<Trooper> ();
		firstTroop.team = troopTeam;
		firstTroop.id = troopId;
		roster.Add (firstTroop);
	}
		

	void selectTrooper(Trooper a){
		a.select();
	}
}
