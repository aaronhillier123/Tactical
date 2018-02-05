using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControlScript : MonoBehaviour {

	public byte Version = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void CreateGame(){
		PhotonNetwork.ConnectUsingSettings (Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
	}
}
