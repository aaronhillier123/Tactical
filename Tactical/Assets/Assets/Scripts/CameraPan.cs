using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void moveLeft(){
		gameObject.transform.Translate ( 0f, 0f, -0.25f);
	}

	void moveRight(){
		gameObject.transform.Translate ( 0f, 0f, 0.25f);
	}

	void moveUp(){
		gameObject.transform.Translate ( -0.25f, 0f, 0f);
	}

	void moveDown(){
		gameObject.transform.Translate ( 0.25f, 0f, 0);
	}

	public void moveToPlayer(Trooper t){
		Vector3 troopPos = t.gameObject.transform.position;
		Vector3 newPos = new Vector3 (troopPos.x - 8, gameObject.transform.position.y, troopPos.z-10);
		StartCoroutine (moveTo (newPos));
	}

	public IEnumerator moveTo(Vector3 dest){
		while (Vector3.Distance (gameObject.transform.position, dest) > 1) {
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, dest, 5f);
			yield return null;
		}
		gameObject.transform.position = dest;
	}
}
