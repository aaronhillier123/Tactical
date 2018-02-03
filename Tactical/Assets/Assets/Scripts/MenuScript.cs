using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class MenuScript : MonoBehaviour {

	public byte Version = 1;
	private string roomName = "";
	private List<string> players;

	private string _playFabPlayerIdCache;
		
	private void AuthenticateWithPlayFab()  {
		LogMessage("PlayFab authenticating using Custom ID...");
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = PlayFabSettings.DeviceUniqueIdentifier+"EDITOR"
			}, RequestPhotonToken, OnPlayFabError);
	}

	private void RequestPhotonToken(LoginResult obj) {
		LogMessage("PlayFab authenticated. Requesting photon token...");

		_playFabPlayerIdCache = obj.PlayFabId;

		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
			}, AuthenticateWithPhoton, OnPlayFabError);
	}
		
	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {
		LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");
		var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
		customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service
		customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);
		PhotonNetwork.AuthValues = customAuth;
	}

	private void OnPlayFabError(PlayFabError obj) {
		LogMessage(obj.ErrorMessage);
	}

	public void LogMessage(string message) {
		Debug.Log("PlayFab + Photon Example: " + message);
	}

	// Use this for initialization
	void Start () {
		
	}

	void Awake(){
		DontDestroyOnLoad (gameObject);
		ConnectToLobby ();
		AuthenticateWithPlayFab ();
		PhotonNetwork.autoJoinLobby = true;
	}

	public void ConnectToLobby(){
		PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
		PhotonNetwork.JoinLobby ();
	}


	// Update is called once per frame
	void Update () {

	}

	public virtual void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		//AuthenticateWithPlayFab ();
	}

	public virtual void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList()");


	}

	public void CreateGame(){
		
		if (PhotonNetwork.insideLobby) {
			SceneManager.LoadScene ("GameScene");
			if (PhotonNetwork.insideLobby) {
				PhotonNetwork.JoinOrCreateRoom("TestRoom2", new RoomOptions () {
					MaxPlayers = 4,
					EmptyRoomTtl = 0,
					PlayerTtl = -1,
					IsVisible = true
				}, null);
			}
		}
	}

	public void rejoinGame(){
		if(PhotonNetwork.insideLobby){
			SceneManager.LoadScene ("GameScene");
			PhotonNetwork.ReJoinRoom ("TestRoom2");
		}
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		

	}

	public virtual void OnReceivedRoomListUpdate(){
			/*
		if (PhotonNetwork.insideLobby) {
			RoomInfo[] ro = PhotonNetwork.GetRoomList ();
			foreach (RoomInfo r in ro) {
				Debug.Log (r.Name);
			}
		}
*/
	}

	// the following methods are implemented to give you some context. re-implement them as needed.

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
	}


	public virtual void OnPhotonPlayerConnected(PhotonPlayer otherPlayer){
	}

	public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){
	}

}
