using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour {

	public bool gameStarted = false;
	// Use this for initialization
	void Start () {
		//ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.room.CustomProperties;
		//object gs;
		//ht.TryGetValue ((object)"Player" + PhotonNetwork.player.ID.ToString (), out gs);
		//gameStarted = (bool)gs;
	}
	
	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.inRoom && gameStarted==false){
			Player isPlayer = GameHandler._instance.getPlayer (PhotonNetwork.player.ID);
			if (isPlayer == null) {
				var response = PhotonNetwork.RaiseEvent (1, null, true, new RaiseEventOptions () {
					ForwardToWebhook = true,
					Receivers = ReceiverGroup.All,
					CachingOption = EventCaching.AddToRoomCache
				});
				Debug.Log ("EVENT IS " + response);
			}
			gameStarted = true;
		}
	}
}
