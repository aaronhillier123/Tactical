using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;

using JsonObject = PlayFab.Json.JsonObject;

public class GameListObject{
	public string GameList;
	public string Message;
}

public class MenuScript : MonoBehaviour {

	public byte Version = 1;
	private string roomName = "";
	private List<string> players;

	private string _playFabPlayerIdCache;
	public string GameName = "Tester10";
	private void AuthenticateWithPlayFab()  {
		LogMessage("PlayFab authenticating using Custom ID...");
		Debug.Log (PlayFabSettings.DeviceUniqueIdentifier + " is DUID");
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
		Debug.Log (_playFabPlayerIdCache + " is plafab cache id");
		LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");
		var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
		customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service
		customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);
		PhotonNetwork.AuthValues = customAuth;
	}

	public static void CloudGetMyGames(){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "GetMyGames",
			FunctionParameter = new { name = "YOUR NAME"},
			GeneratePlayStreamEvent = true,
		}, OnGotGames, OnErrorShared);
	}

	public static void OnGotGames(ExecuteCloudScriptResult result){
		
		List<string> myList = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (result.FunctionResult.ToString());
		Debug.Log ("GETTING GAMES");
		MyGames._instance.SetGames (myList);
	}

	public static void OnErrorShared(PlayFabError error){
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
			PhotonNetwork.CreateRoom(GameName, new RoomOptions () {
				MaxPlayers = 4,
				EmptyRoomTtl = 100,
				PlayerTtl = -1,
				IsVisible = true
			}, null);
		}
	}

	public void JoinGame(){
		if (PhotonNetwork.insideLobby) {
			SceneManager.LoadScene ("GameScene");
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	void OnPhotonCreateRoomFailed()
	{

	}

	public void rejoinGame(){
		if(PhotonNetwork.insideLobby){
			SceneManager.LoadScene ("GameScene");
			PhotonNetwork.ReJoinRoom (GameName);
		}
	}

	public void rejoinGame(string s){
		if(PhotonNetwork.insideLobby){
			SceneManager.LoadScene ("GameScene");
			PhotonNetwork.ReJoinRoom (s);
		}
	}

	public virtual void OnPhotonJoinRoomFailed(){
		PhotonNetwork.CreateRoom(GameName, new RoomOptions () {
			MaxPlayers = 4,
			EmptyRoomTtl = 100,
			PlayerTtl = -1,
			IsVisible = true
		}, null);
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions () {
			MaxPlayers = 4,
			EmptyRoomTtl = 100,
			PlayerTtl = -1,
			IsVisible = true
		}, null);
	}

	public void loadGameScene(){
		SceneManager.LoadScene ("MyGamesScene");
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
