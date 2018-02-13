﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	float deltaWheel;
	float originalZoom;
	float maxZoom;
	float currentZoom;
	// Use this for initialization
	void Start () {
		deltaWheel = 0;
		currentZoom = gameObject.transform.position.y;
		originalZoom = gameObject.transform.position.y;
		maxZoom = 20;
	}
	
	// Update is called once per frame
	void Update () {
		currentZoom = gameObject.transform.position.y;
		deltaWheel = Input.GetAxis ("Mouse ScrollWheel");
		if(deltaWheel < 0f && currentZoom < originalZoom+maxZoom){
			gameObject.transform.Translate(0f, 0f, -.8f);
		}
		if(deltaWheel > 0f && currentZoom > originalZoom-maxZoom){
			gameObject.transform.Translate(0f, 0f, .8f);
		}
	}

	public void resetZoom(){
		deltaWheel = 0;
		currentZoom = gameObject.transform.position.y;
		originalZoom = gameObject.transform.position.y;
		maxZoom = 20;
	}
}