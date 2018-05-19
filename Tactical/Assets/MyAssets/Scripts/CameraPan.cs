using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	public static CameraPan _instance;
	public bool dragging = false;
	public Vector3 momentum = new Vector3();
	public float LoseMomRate = 0.95f;
	float startTime = 0;
	Vector3 lastPosition = new Vector3();

	public List<Coroutine> momentums = new List<Coroutine> ();

	float xMax = 290f;
	float xMin = 10f;
	float zMax = 290f;
	float zMin = 10f;

	// Use this for initialization
	void Start () {
		_instance = this;
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		foreach (GameObject g in spawns) {
			if (g.GetComponent<SpawnArea> ().team == PhotonNetwork.player.ID) {
				float x = g.transform.position.x;
				float y = transform.position.y;
				float z = g.transform.position.z;
				transform.position = new Vector3 (x, y, z);
			}
		}
		//InvokeRepeating ("UpdateMomentum", 0.1f, 0.02f);
	}
	
	// Update is called once per frame
	void Update () {
		checkBounds ();
		UpdateMomentum ();
	}

	public void checkBounds(){
		if (transform.position.x > xMax) {
			transform.position = new Vector3 (xMax, transform.position.y, transform.position.z);
		}
		if (transform.position.x < xMin) {
			transform.position = new Vector3 (xMin, transform.position.y, transform.position.z);
		}
		if (transform.position.z > zMax) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, zMax);
		}
		if (transform.position.z < zMin) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, zMin);
		}

	}

	public void UpdateMomentum(){
		if (dragging == true) {
			momentum = transform.position - lastPosition;
			lastPosition = transform.position;
		}
	}

	public IEnumerator UseMomentum(){
		float mag = momentum.magnitude;
		if (mag > 10) {
			mag = 10;
		}
		Vector3 dir = momentum.normalized;
		while (mag > 0) {
			transform.Translate (dir * mag);
			mag *= LoseMomRate;
			yield return null;
		}
	}

	public void release(){
		dragging = false;
		momentums.Add(StartCoroutine (UseMomentum ()));
		momentum = Vector3.zero;
	}

	public void drag(){
		dragging = true;
		CameraPan._instance.StopCoroutine (UseMomentum ());
		startTime = Time.time;
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

	public void moveToObject(GameObject t, bool instant){
		CameraPan._instance.StopAllCoroutines ();
		Vector3 pos = t.transform.position;
		Vector3 newPos = new Vector3 (pos.x + 25, gameObject.transform.position.y, pos.z + 5);
		if (instant == false) {
			StartCoroutine (moveTo (newPos, 1f));
		} else {
			StartCoroutine (moveTo (newPos));
		}

	}

	public IEnumerator moveTo(Vector3 dest, float time){

		float elapsedTime = 0;
		Vector3 initPos = transform.position;
		while (elapsedTime < time) {
			transform.position = Vector3.Lerp (initPos, dest, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		gameObject.transform.position = dest;
		StopAllCoroutines ();
	}

	public IEnumerator moveTo(Vector3 dest){
		while (Vector3.Distance (gameObject.transform.position, dest) > 1) {
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, dest, 2f);
			yield return null;
		}
		gameObject.transform.position = dest;
		StopAllCoroutines ();
	}

}
