using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour {

	public bool gameStarted = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.inRoom && gameStarted==false){
			var response = PhotonNetwork.RaiseEvent (1, null, true, new RaiseEventOptions(){
				ForwardToWebhook = true,
				Receivers = ReceiverGroup.All,
				CachingOption = EventCaching.AddToRoomCache});
			Debug.Log ("EVENT IS " + response);
			gameStarted = true;
		}
	}
}
