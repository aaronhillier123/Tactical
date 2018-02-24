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
using System.Threading;

public class GameListObject{
	public string GameList;
	public string Message;
}

public class NetGame{
	public List<string> players = new List<string> ();
	public string id = "";
	public bool inGame = false;
}

public class MenuScript : MonoBehaviour {

	public static MenuScript _instance;

	public byte Version = 1;
	private string roomName = "";
	private List<string> players;

	public GameObject FriendsListObject;
	public GameObject OptionsObject;
	public GameObject GameOverPanel;
	public GameObject DarkPanel;
	public bool running = false;
	private GameObject optionsPanel;

	private string _message;
	public List<string> currentGames = new List<string> ();
	public List<string> currentInvites = new List<string> ();
	private string _playFabPlayerIdCache;
	public string GameName = "Tester10";
	public int allowedGames = 10;

	void Start () {

		if (GameObject.FindObjectsOfType<MenuScript> ().Length > 1) {
			Destroy (gameObject);
		}


		_instance = this;
		if (!FB.IsInitialized) {
			FB.Init (OnFacebookInitialized);
		}
		DontDestroyOnLoad (gameObject);
		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);
		}
		PhotonNetwork.autoJoinLobby = false;
	}

	private void reload(){

		if (!FB.IsInitialized) {
			FB.Init (OnFacebookInitialized);
		}

		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);
		}
	}

	private void RequestPhotonToken(LoginResult obj) {
		_playFabPlayerIdCache = obj.PlayFabId;
		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
			{
				PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppID
			}, AuthenticateWithPhoton, OnPlayFabError);
	}
		
	private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {

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
		if(running==false){
			PhotonNetwork.OnEventCall += GameHandler.CreatePlayer; //1
			PhotonNetwork.OnEventCall += Trooper.move; //2
			PhotonNetwork.OnEventCall += GameHandler.EndPlacements;//3
			PhotonNetwork.OnEventCall += Player.attack; //4
			PhotonNetwork.OnEventCall += GameHandler.setTurn; //5
			PhotonNetwork.OnEventCall += Player.throwGrenade; //6
			PhotonNetwork.OnEventCall += Trooper.RaiseInvulnerable; //7
			PhotonNetwork.OnEventCall += Trooper.RaiseNotInvulnerable;//8
			PhotonNetwork.OnEventCall += Game.raiseBarrier; //15
			PhotonNetwork.OnEventCall += GameHandler.SyncGameState;//9
			PhotonNetwork.OnEventCall += Player.airStrike; //12
			PhotonNetwork.OnEventCall += Game.BeginGame;//11
			PhotonNetwork.OnEventCall += Player.NetworkTroopAt;//13
			PhotonNetwork.OnEventCall += GameHandler.EndGame;//14
			running = true;
		}

		PhotonNetwork.JoinLobby ();
	}

	private void OnCustomAuthenticationFailed(string debugMessage){
	}

	public void GetFriendsList(){
		removeOptionsPanel ();
		GetFriendsListRequest request = new GetFriendsListRequest ();
		request.IncludeFacebookFriends = true;
		PlayFabClientAPI.GetFriendsList (request, GotFriendsList, FriendsListError);
	}

	private void GotFriendsList(GetFriendsListResult result){
		List<Friend> friends = result.Friends;
		GameObject g = Instantiate (FriendsListObject,GameObject.Find ("Canvas").transform, false);
		FriendsPanel fp = g.GetComponentInChildren<FriendsPanel> ();
		fp.CreateFriend (friends);
	}

	public void LeaveRoom(){
		PhotonNetwork.networkingPeer.OpLeaveRoom (true);
	}

	public void goToMenu(){
		SceneManager.LoadScene ("MainMenu");
		SceneManager.UnloadSceneAsync ("MyGamesScene");
		reload ();
	}

	public void DoneWithGame(){
		string name = PhotonNetwork.room.Name;
		LeaveRoom ();
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "DoneWithGame",
			FunctionParameter = new {roomName = name},
			GeneratePlayStreamEvent = true,
		}, OnFinishedGame, OnFinishedGameError);
	}

	public void OnFinishedGame(ExecuteCloudScriptResult result){

	}

	public void OnFinishedGameError(PlayFabError error){

	}

	void OnJoinedRoom(){
		Debug.Log ("Player joined room");
		foreach (PhotonPlayer p in PhotonNetwork.playerList) {
			Debug.Log ("Player " + p.ID + " is in room");
			Debug.Log ("Currently player " + GameHandler._instance.getPlayersTurn () + " is moving");
		}
	}

	void OnPhotonPlayerConnected(PhotonPlayer p){
		if (PhotonNetwork.player.ID == GameHandler._instance.getPlayersTurn ()) {
			GameHandler._instance.refreshGameStates ();
		}
	}

	void OnLeftRoom(){
		SceneManager.LoadScene ("MainMenu");
		SceneManager.UnloadSceneAsync ("GameScene");
	}

	private void FriendsListError(PlayFabError obj){
	}

	public void LoadGame(){
		SceneManager.LoadScene ("GameScene");
	}

	public static void OnGotGames(ExecuteCloudScriptResult result){
		List<string> myList = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (result.FunctionResult.ToString());
		MenuScript._instance.currentGames = myList;
	}

	public static void OnGotInvites(ExecuteCloudScriptResult result){

		List<string> myListi = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (result.FunctionResult.ToString());
		MenuScript._instance.currentInvites = myListi;
	}

	public static void OnErrorShared(PlayFabError error){
	}

	private void OnPlayFabError(PlayFabError obj) {
	}
		
	private void OnFacebookInitialized(){
		if (FB.IsLoggedIn) {
			FB.LogOut ();
		}
		FB.LogInWithReadPermissions (null, OnFacebookLoggedIn);
	}

	public void removeOptionsPanel(){
		if(optionsPanel!=null){
			Destroy (optionsPanel.gameObject);
		}
	}

	public void removeDarkPanel(){
		GameObject g = GameObject.Find ("DarkPanel(Clone)");
		if (g != null) {
			Destroy (g);
		}

	}

	public void showDarkPanel(){
		GameObject g = GameObject.Find ("DarkPanel(Clone)");
		if (g == null) {
			g = Instantiate (DarkPanel, GameObject.Find ("Canvas").transform);
		}
	}

	public void showOptionsPanel(){
		if (optionsPanel == null) {
			optionsPanel = Instantiate (OptionsObject, GameObject.Find ("Canvas").transform, false);
			if (GameHandler._instance.getPlayersTurn () > 0) {
				try {
					optionsPanel.transform.Find ("FriendsButton").gameObject.GetComponent<Button> ().interactable = false;
				} catch {

				}
			}
		}
	}

	private void OnFacebookLoggedIn(ILoginResult result){
		if (result == null || string.IsNullOrEmpty (result.Error)) {
			PlayFabClientAPI.LoginWithFacebook (new LoginWithFacebookRequest {
				CreateAccount = true,
				AccessToken = AccessToken.CurrentAccessToken.TokenString
			},
				RequestPhotonToken, OnPlayfabFacebookAuthFailed);
		} else {
		}
	}

	private void OnPlayfabFacebookAuthComplete(LoginResult result){
	}

	private void OnPlayfabFacebookAuthFailed(PlayFabError error){
	}

	public void ConnectToLobby(){
		
	}
		
	public void SendInviteToAll(List<string> fid){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "InviteFriends",
			FunctionParameter = new {all = fid},
			GeneratePlayStreamEvent = true,
		}, OnSentInvites, OnInviteError);

	}
		
	public void OnSentInvites(ExecuteCloudScriptResult result){

	}

	public void OnInviteError(PlayFabError error){

	}

	public static void CloudGetEmptyGames(){

		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "GetEmptyGames",
			FunctionParameter = new { name = "YOUR NAME"},
			GeneratePlayStreamEvent = true,
		}, OnGotEmptyGames, OnEmptyErrorShared);

	}

	private static void OnEmptyErrorShared(PlayFabError obj) {
	}

	public static void OnGotEmptyGames(ExecuteCloudScriptResult result){
		MenuScript.CloudGetMyGames ();
		List<string> allEmpty = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (result.FunctionResult.ToString());
		List<string> allOthers = allEmpty.Except (MenuScript._instance.currentGames).ToList();
		Debug.Log (allEmpty.Count + " games are not full");
		Debug.Log (allOthers.Count + " are nonfull that are not yours");
		foreach (string s in allOthers) {
			Debug.Log (s);
		}
		if (allOthers.Count > 0) {
			MenuScript._instance.JoinGame (allOthers [0]);
		} else {
			MenuScript._instance.CreateGame ();
		}
	}

	public static void CloudGetMyGames(){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "GetMyGames",
			FunctionParameter = new { name = "YOUR NAME"},
			GeneratePlayStreamEvent = true,
		}, OnGotGames, OnErrorShared);
	}

	public static void CloudGetMyInvites(){
		PlayFabClientAPI.ExecuteCloudScript (new ExecuteCloudScriptRequest () {
			FunctionName = "GetMyInvites",
			FunctionParameter = new { name = "YOUR NAME"},
			GeneratePlayStreamEvent = true,
		}, OnGotInvites, OnErrorInvites);
	}

	public static void OnErrorInvites(PlayFabError error){
	}

	public virtual void OnJoinedLobby()
	{
		CloudGetMyGames ();
		CloudGetMyInvites ();
	}
		
	public void CreateGame(){
		
		if (PhotonNetwork.insideLobby) {
			CloudGetMyGames ();
			if (MenuScript._instance.currentGames.Count < allowedGames) {
				Debug.Log ("Current game count: " + currentGames.Count);
				SceneManager.LoadScene ("GameScene");
				PhotonNetwork.CreateRoom (null, new RoomOptions () {
					MaxPlayers = 4,
					EmptyRoomTtl = 1000,
					PlayerTtl = -1,
					IsVisible = true
				}, null);
			} else {
				showDarkPanel ();
			}
		}
	}

	public void JoinGame(){
		if (PhotonNetwork.insideLobby) {
			if (MenuScript._instance.currentGames.Count < allowedGames) {
				CloudGetEmptyGames ();
			} else {
				showDarkPanel ();
			}
		}
	}

	public void JoinGame(string name){
		if (PhotonNetwork.insideLobby) {
			SceneManager.LoadScene ("GameScene");
			PhotonNetwork.JoinRoom (name);
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
			EmptyRoomTtl = 1000,
			PlayerTtl = -1,
			IsVisible = true
		}, null);
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions () {
			MaxPlayers = 4,
			EmptyRoomTtl = 1000,
			PlayerTtl = -1,
			IsVisible = true
		}, null);
	}

	public void loadGameScene(){
		SceneManager.LoadScene ("MyGamesScene");
	}

	public virtual void OnReceivedRoomListUpdate(){
	}

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
	}
		

	public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){
	}

	public void initNetGame(string roomId, bool isGame){
		try{
			NetGame ng = new NetGame ();
			List<string> mems = new List<string> ();
			Debug.Log ("DOING THING");
			PlayFabClientAPI.GetSharedGroupData (new GetSharedGroupDataRequest () {
				SharedGroupId = roomId,
				GetMembers = true
			}, result => {
				Debug.Log("Getting Names");
				string names = "";
				if(result.Data.ContainsKey("Names")){
					names = result.Data["Names"].Value;
				}
				List<string> playerNames = PlayFab.Json.JsonWrapper.DeserializeObject<List<string>> (names);

				ng.players = playerNames;
				ng.id = roomId;
				ng.inGame = isGame;
				MyGames._instance.currentNet = ng;
				MyGames._instance.showGameDetails();
			}, (error) => {
				Debug.Log("error getting members");
			});
		} catch{
		}
	}

}
