using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

	public int team;
	public Vector3 destination;
	public GameObject explosion;
	public Vector3 initialPosition;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
		StartCoroutine (initLifeCycle ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IEnumerator initLifeCycle(){
		Vector3 mid = (Game.midPoint (initialPosition, destination)) + new Vector3(0f, 5f, 0f);
		Vector3 fq = (Game.midPoint (initialPosition, mid)) + new Vector3 (0f, 3f, 0f);
		Vector3 lq = (Game.midPoint (mid, destination)) + new Vector3(0f, 3f, 0f);
		while (Vector3.Distance (transform.position, fq) > 1f) {
			transform.position = Vector3.MoveTowards (transform.position, fq, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (transform.position, mid) > 1f) {
			transform.position = Vector3.MoveTowards (transform.position, mid, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (transform.position, lq) > 1f) {
			transform.position = Vector3.MoveTowards (transform.position, lq, 30 * Time.deltaTime);
			yield return null;
		}
		while (Vector3.Distance (transform.position, destination) > 1f) {
			transform.position = Vector3.MoveTowards (transform.position, destination, 30 * Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds (.5f);
		GameObject ex = GameObject.Instantiate (explosion, transform.position, Quaternion.identity);
		Explosion now = ex.GetComponent<Explosion> ();
		if (now != null) {
			now.type = 1;
		}
		Destroy (gameObject);
	}
}
