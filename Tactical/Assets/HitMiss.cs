using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HitMiss : MonoBehaviour {

	public string hitmis;
	// Use this for initialization
	void Start () {
		StartCoroutine (DeleteMe ());
	}
	
	// Update is called once per frame
	void Update () {
		try{
			gameObject.GetComponent<Text>().text = hitmis;
			if(hitmis == "Missed"){
				gameObject.GetComponent<Text>().color = Color.red;
			} else if (hitmis == "Hit"){
				gameObject.GetComponent<Text>().color = Color.green;
			}
		}
		catch{
		}
		transform.Translate (0f, .01f, 0f);
	}

	IEnumerator DeleteMe(){
		yield return new WaitForSeconds (2f);
		Destroy (gameObject);
	}
}
