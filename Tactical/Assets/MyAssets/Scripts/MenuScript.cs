using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using LoginResult = PlayFab.ClientModels.LoginResult;
using JsonObject = PlayFab.Json.JsonObject;
using UnityEngine.UI;
using Friend = PlayFab.ClientModels.FriendInfo;

public class GameListObject{
	public string GameList;
	public string Message;
}

public class MenuScript : MonoBehaviour {

	public byte Version = 1;
	private string roomName = "";
	private List<string> players;

	public GameObject FriendsListObject;

	private string _message;

	private string _playFabPlayerIdCache;
	public string GameName = "Tester10";

	/*
	private void AuthenticateWithPlayFab()  {
		LogMessage("PlayFab authenticating using Custom ID...");
		Debug.Log (PlayFabSettings.DeviceUniqueIdentifier + " is DUID");
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
			{
				CreateAccount = true,
				CustomId = PlayFabSettings.DeviceUniqueIdentifier+"EDITOR"
			}, RequestPhotonToken, OnPlayFabError);
	}
	*/

	private void RequestPhotonToken(LoginResult obj) {
		LogMessage("PlayFab authenticated. Requesting photon token...");

		_playFabPlayerIdCache = obj.PlayFabId;

		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
			}, AuthenticateWithPhoton, OnPlayFabError);
	}
		
	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {

		/*
		Debug.Log ("Authenting with Photon");
		string aToken = AccessToken.CurrentAccessToken.TokenString;
		string facebookid = AccessToken.CurrentAccessToken.UserId;
		PhotonNetwork.AuthValues = new AuthenticationValues ();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Facebook;
		PhotonNetwork.AuthValues.UserId = facebookid;
		PhotonNetwork.AuthValues.AddAuthParameter ("token", aToken);
		PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
		*/

		var customAuth = new AuthenticationValues {
			AuthType = CustomAuthenticationType.Custom
		};

		customAuth.AddAuthParameter ("username", _playFabPlayerIdCache);

		customAuth.AddAuthParameter ("token", obj.PhotonCustomAuthenticationToken);

		PhotonNetwork.AuthValues = customAuth;

		PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
	}

	private void OnConnectedToMaster(){
		foreach (Button b in GameObject.FindObjectsOfType<Button>()) {
			b.interactable = true;
		}
		PhotonNetwork.JoinLobby ();
	}

	private void OnCustomAuthenticationFailed(string debugMessage){
		Debug.LogErrorFormat ("Error with facebook auth: {0} ", debugMessage);
	}

	public static void CloudGetMyGames(){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "GetMyGames",
			FunctionParameter = new { name = "YOUR NAME"},
			GeneratePlayStreamEvent = true,
		}, OnGotGames, OnErrorShared);
	}

	public void GetFriendsList(){
		GetFriendsListRequest request = new GetFriendsListRequest ();
		request.IncludeFacebookFriends = true;
		PlayFabClientAPI.GetFriendsList (request, GotFriendsList, FriendsListError);
	}

	private void GotFriendsList(GetFriendsListResult result){
		
		List<Friend> friends = result.Friends;
		Debug.Log ("HOW MANY FRIENDS?:" + friends.Count);
		GameObject g = Instantiate (FriendsListObject,GameObject.Find ("Canvas").transform, false);
		FriendsPanel fp = g.GetComponentInChildren<FriendsPanel> ();
		fp.CreateFriend (friends);
	}

	private void FriendsListError(PlayFabError obj){
		Debug.Log ("NOPE");
	}

	public static void OnGotGames(ExecuteCloudScriptResult result){
		
		List<string> myList = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (result.FunctionResult.ToString());
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
		SetMessage ("Initializing Facebook...");
		if (!FB.IsInitialized) {
			FB.Init (OnFacebookInitialized);
		}


		DontDestroyOnLoad (gameObject);
		//ConnectToLobby ();
		//AuthenticateWithPlayFab ();
		PhotonNetwork.autoJoinLobby = false;
		
	}

	private void OnFacebookInitialized(){

		SetMessage ("Logging into facebook");

		if (FB.IsLoggedIn) {
			FB.LogOut ();
		}

		FB.LogInWithReadPermissions (null, OnFacebookLoggedIn);

	}

	private void OnFacebookLoggedIn(ILoginResult result){
		if (result == null || string.IsNullOrEmpty (result.Error)) {

			SetMessage ("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into Playfab...");

			PlayFabClientAPI.LoginWithFacebook (new LoginWithFacebookRequest {
				CreateAccount = true,
				AccessToken = AccessToken.CurrentAccessToken.TokenString
			},
				RequestPhotonToken, OnPlayfabFacebookAuthFailed);
		
		} else {
			SetMessage ("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
		}

	}

	private void OnPlayfabFacebookAuthComplete(LoginResult result){
		SetMessage ("Playfab Facebook Auth Complete. Session ticket: " + result.SessionTicket);

	}

	private void OnPlayfabFacebookAuthFailed(PlayFabError error){
		SetMessage ("PlfayFab Facebook Auth Failed: " + error.GenerateErrorReport (), true);
	}

	public void SetMessage(string message, bool error=false)
	{
		_message = message;
		if(error){
			Debug.LogError (_message);
		}else {
			Debug.Log(_message);
		}
	}

	void Awake(){
		
	}

	public void ConnectToLobby(){
		
	}


	// Update is called once per frame
	void Update () {

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
