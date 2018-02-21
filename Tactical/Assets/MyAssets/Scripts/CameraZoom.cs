using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public static CameraZoom _instance;

	float deltaZoom;
	float originalZoom;
	float maxZoom;
	float currentZoom;
	// Use this for initialization
	void Start () {
		_instance = this;
		deltaZoom = 0;
		currentZoom = gameObject.transform.position.y;
		originalZoom = gameObject.transform.position.y;
		maxZoom = 20;
	}
	
	// Update is called once per frame
	void Update () {
		currentZoom = gameObject.transform.position.y;
	}

	void checkForWheel(){
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			deltaZoom = Input.GetAxis("Mouse ScrollWheel");
			zoom (-0.8f);
		}
	}


	public void zoom(float distance){
		if(distance < 0 && currentZoom < originalZoom+maxZoom){
			gameObject.transform.Translate(0f, 0f, distance);
		}
		if(distance > 0 && currentZoom > originalZoom-maxZoom){
			gameObject.transform.Translate(0f, 0f, distance);
		}
	}

	public void resetZoom(){
		deltaZoom = 0;
		currentZoom = gameObject.transform.position.y;
		originalZoom = gameObject.transform.position.y;
		maxZoom = 20;
	}
}
