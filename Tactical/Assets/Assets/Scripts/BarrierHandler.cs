using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BarrierHandler : MonoBehaviour {

	public static BarrierHandler _instance;
	public Player myPlayer;
	public Material[] BarrierMaterials;
	public GameObject[] BarrierTypes;
	public List<Barrier> allBarriers;
	// Use this for initialization
	void Start () {
		_instance = this;
	}

	void Update () {
		if(myPlayer==null){
			try{
				myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
			} catch{
			}
		}
	}

	public void CreateBarrier(int type){
		myPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
		Camera cam = Camera.main;
		Ray ray = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 300)) {
			if (hit.collider.CompareTag ("Terrain")) {
				GameObject NewBarrier = Instantiate (BarrierTypes [type], hit.point, Quaternion.identity) as GameObject;
				Barrier myNewBarrier = NewBarrier.GetComponent<Barrier> ();
				myNewBarrier.team = myPlayer.team;
				myNewBarrier.id = BarrierHandler._instance.allBarriers.Count + 1;
				myNewBarrier.type = type;
				foreach (GameObject g in myNewBarrier.pieces) {
					g.GetComponent<MeshRenderer> ().material = BarrierMaterials [myNewBarrier.team];
					g.GetComponent<BarrierPiece> ().myBarrier = myNewBarrier;
				}
				BarrierHandler._instance.allBarriers.Add (myNewBarrier);
				EventSystem.current.currentSelectedGameObject.GetComponent<Button> ().interactable = false;
			}
		}
	}

	public void CreateBarrier(int type, Vector3 loc, Vector3 Cylloc, Quaternion rot, int team){
		Player myPlayer = GameHandler._instance.getPlayer (team);
		GameObject NewBarrier = Instantiate (BarrierHandler._instance.BarrierTypes [type], loc, rot) as GameObject;
		Barrier myNewBarrier = NewBarrier.GetComponent<Barrier> ();
		myNewBarrier.Cylinder.transform.position = Cylloc;
		myNewBarrier.team = team;
		myNewBarrier.id = BarrierHandler._instance.allBarriers.Count + 1;
		myNewBarrier.type = type;
		BarrierHandler._instance.allBarriers.Add (myNewBarrier);
		foreach (GameObject g in myNewBarrier.pieces) {
			g.GetComponent<MeshRenderer> ().material = BarrierMaterials [myNewBarrier.team];
			g.GetComponent<BarrierPiece> ().myBarrier = myNewBarrier;
		}
		myNewBarrier.HidePrelimbs ();
	}

	public void RemoveAllPrelimbs(){
		foreach (Barrier b in allBarriers) {
			b.HidePrelimbs ();
		}
	}

	public void PlaceAllBarriers(){
		foreach(Barrier b in allBarriers){
			b.placed = true;
		}
	}

	public BarrierPiece getPiece(int id){
		foreach(Barrier b in allBarriers){
			foreach (GameObject bp in b.pieces) {
				BarrierPiece mybp = bp.GetComponent<BarrierPiece> ();
				if (mybp != null && mybp.id == id) {
					return mybp;
				}
			}
		}
		return null;
	}

}
