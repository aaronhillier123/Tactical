using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		foreach (GameObject g in spawns) {
			if (g.GetComponent<SpawnArea> ().team == PhotonNetwork.player.ID) {
				float x = g.transform.position.x;
				float y = transform.position.y;
				float z = g.transform.position.z;
				transform.position = new Vector3 (x, y, z);
			}
		}

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

	public void moveToObject(GameObject t){
		Vector3 pos = t.transform.position;
		Vector3 newPos = new Vector3 (pos.x - 8, gameObject.transform.position.y, pos.z + 5);
		StartCoroutine (moveTo (newPos));
	}

	public IEnumerator moveTo(Vector3 dest){
		while (Vector3.Distance (gameObject.transform.position, dest) > 1) {
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, dest, 1f);
			yield return null;
		}
		gameObject.transform.position = dest;
	}
}
