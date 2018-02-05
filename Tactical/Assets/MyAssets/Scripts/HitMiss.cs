using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HitMiss : MonoBehaviour {

	public int hitmis = 0;
	// Use this for initialization
	void Start () {
		StartCoroutine (DeleteMe ());
	}
	
	// Update is called once per frame
	void Update () {
		switch (hitmis) {
		case 0:
			gameObject.GetComponent<Text> ().text = "MISSED";
			gameObject.GetComponent<Text> ().color = Color.red;
			break;
		case 1:
			gameObject.GetComponent<Text> ().text = "HIT";
			gameObject.GetComponent<Text> ().color = Color.green;
			break;
		case 2:
			gameObject.GetComponent<Text> ().text = "BLOCKED";
			gameObject.GetComponent<Text> ().color = Color.yellow;
			break;
		default:
			break;
		}
		transform.Translate (0f, .01f, 0f);
	}

	IEnumerator DeleteMe(){
		yield return new WaitForSeconds (2f);
		Destroy (gameObject);
	}
}
