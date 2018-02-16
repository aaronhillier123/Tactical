using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class FriendsOverlay : MonoBehaviour {


	public Button cancel;
	public Button invite;
	public List<string> ids;
	public GameObject FriendsPanelObject;
	// Use this for initialization
	void Start () {
		cancel.onClick.AddListener (delegate {
			CancelMe ();
		});

		invite.onClick.AddListener (delegate {
			InviteAll (ids);
		});
	}


	public void CancelMe(){
		Destroy (this.gameObject);
	}

	public void InviteAll(List<string> fid){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "InviteFriends",
			FunctionParameter = new {all = fid, room = PhotonNetwork.room.Name},
			GeneratePlayStreamEvent = true,
		}, OnSentInvites, OnInviteError);
		CancelMe ();
	}

	public void OnSentInvites(ExecuteCloudScriptResult result){

	}
	public void OnInviteError(PlayFabError error){

	}
	// Update is called once per frame
	void Update () {
		
	}


}
