﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public static CameraController _instance;
	public GameObject follow;
	public GameObject cpo;
	public GameObject czo;

	public CameraPan cp;
	public CameraZoom cz;
	// Use this for initialization
	void Start () {
		_instance = this;
		cp = cpo.GetComponent<CameraPan> ();
		cz = czo.GetComponent<CameraZoom> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (follow != null) {
			cp.moveToObject (follow, true);
		} else {
			follow = null;
		}
	}

	public void aerialView(){
		Camera.main.transform.rotation =  Quaternion.Euler (90, 90, 180);
		Camera.main.transform.localPosition = new Vector3 (0, 30, 0);
	}

	public void normalView(){
		Camera.main.transform.rotation =  Quaternion.Euler (135, 90, 180);
		Camera.main.transform.localPosition = new Vector3 (0, 0, 0);
	}

	public void setFollowedObject(GameObject ob, int release){
		cp.StopAllCoroutines ();
		follow = ob;
		if (release == 1) {
			StartCoroutine (WaitToUnfollow (1f));
		}
	}

	public IEnumerator WaitToUnfollow(float time){
		yield return new WaitForSeconds (time);
		CameraController._instance.setFollowedObject (null, 0);
	}


}
