using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour {

	public bool gameStarted = false;
	public static RaiseEventOptions ops;
	// Use this for initialization
	void Start () {
		ops = RaiseEventOptions.Default;
		ops.Receivers = ReceiverGroup.All;
	}
	
	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.inRoom && gameStarted==false){
			PhotonNetwork.RaiseEvent (1, null, true, ops);
			gameStarted = true;
		}
	}
}
