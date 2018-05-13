using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marine : Trooper {


	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		if (moving == false) {
			currentPosition = transform.position;
		} else {
			currentPosition = destinationPosition;
		}
		currentRotation = transform.rotation;
	}
}
